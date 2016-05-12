using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

/// <summary>
/// 通过Render texture 实现画笔.
/// </summary>
public class RenderTexturePainter : MonoBehaviour {

	public enum PaintType
	{
		Scribble,//对图片进行涂和擦除.
		DrawLine,//画线.
//		DrawColorfulLine,//画彩色线
	}

	[Header("Canvas Setting")]
	//画布大小
	public bool userSourceTexSize=true;
	public int canvasWidth=512;
	public int canvasHeight=512;

	//默认背景颜色
	public Color canvasColor = new Color(1,1,1,0);


	[Header("RenderTexture Setting")]
	public int renderTextureDepth = 16;
	public RenderTextureFormat renderTextureformat=RenderTextureFormat.ARGB32;

	[Header("Painter Setting")]
	public PaintType paintType = PaintType.Scribble;
	//画笔方式.
	public Texture penTex, sourceTex;

	public Shader paintShader;

	//是否为擦除.
	public bool isEraser = false;
	//纯色方式.
	public Color penColor=new Color(1, 0, 0, 1);

	//笔刷缩放值
	public float brushScale = 1f;


//	[Header(" Colorfull paint Setting")]
//	//彩色方式
//	public Color[] paintColorful ;
//
//	//速度变化频率，越大变化越慢
//	public float colorChangeRate = 1f;
//	private int m_colorfulIndex = 1;
//	private float m_colorfulTime = 0f;


	[Header("Auto Setting")]
	//是否自动初始化.
	public bool isAutoInit = false;
	public bool isAutoDestroy = true;

	//init show picture
	public bool initShowSource = false;




	private RenderTexture m_rt;
	public RenderTexture renderTexture{ get{ return m_rt; } }

	private bool m_inited = false;
	private bool m_isDown = false;
	private Vector3 m_prevMousePosition;
	private Material m_penMat,m_sourceMat,m_canvasMat;
	private Vector2 m_sourceTexScale;

	public Material penMat{ get{ return m_penMat; } }
	public Material sourceMat{ get{ return m_sourceMat; } }
	public Material canvasMat{ get{ return m_canvasMat; } }

	// Use this for initialization
	void Start () {
		if (isAutoInit) {
			Init(initShowSource);
		}
	}

	public void Init(bool isShowSource=false)
	{
		if(!m_inited){
			m_inited = true;

			if(userSourceTexSize&&sourceTex){
				canvasWidth = sourceTex.width;
				canvasHeight = sourceTex.height;
			}

			m_rt = new RenderTexture(canvasWidth,canvasHeight,renderTextureDepth,renderTextureformat);
			m_rt.useMipMap = false;


			//canvas
			m_canvasMat = CreateMat(paintShader,canvasColor,BlendMode.SrcAlpha,BlendMode.OneMinusSrcAlpha);
			CreateQuad(m_canvasMat);
			m_canvasMat.mainTexture = m_rt;


			if(isEraser)
			{
				m_penMat = CreateMat(paintShader,penColor,BlendMode.Zero,BlendMode.OneMinusSrcAlpha,penColor.a);
				m_sourceMat = CreateMat(paintShader,Color.white,BlendMode.DstAlpha,BlendMode.Zero);
			}
			else
			{
				if(paintType== PaintType.Scribble){

					canvasColor = new Color(1,1,1,0);
					m_canvasMat.SetColor("_Color",canvasColor);
					m_penMat = CreateMat(paintShader,penColor,BlendMode.One,BlendMode.OneMinusSrcAlpha);
					m_penMat.SetFloat("_Cutoff",0.99f);
					m_sourceMat = CreateMat(paintShader,Color.white,BlendMode.DstAlpha,BlendMode.OneMinusDstAlpha);

				}else if(paintType== PaintType.DrawLine){

					m_penMat = CreateMat(paintShader,penColor,BlendMode.SrcAlpha,BlendMode.One,penColor.a);
					m_canvasMat.color=penColor;
					m_sourceMat = CreateMat(paintShader, Color.white,BlendMode.SrcAlpha,BlendMode.OneMinusSrcAlpha);
				}
			}

			ClearCanvas();
			if(isShowSource){
				ShowSourceTexture();
			}
		}
	}

	/// <summary>
	/// 设置是否为擦除
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void SetIsEraser( bool value){
		if(isEraser!=value){
			isEraser = value;
			if(isEraser){
				m_penMat.SetFloat("_BlendSrc",(int)BlendMode.Zero);
				m_penMat.SetFloat("_BlendDst",(int)BlendMode.OneMinusSrcAlpha);
			}else{
				if(paintType== PaintType.Scribble){
					m_penMat.SetFloat("_BlendSrc",(int)BlendMode.One);
					m_penMat.SetFloat("_BlendDst",(int)BlendMode.OneMinusSrcAlpha);
				}else{
					m_penMat.SetFloat("_BlendSrc",(int)BlendMode.SrcAlpha);
					m_penMat.SetFloat("_BlendDst",(int)BlendMode.One);
					m_canvasMat.color=penColor;
				}
			}
		}
	}

	/// <summary>
	/// 设置画笔颜色
	/// </summary>
	/// <param name="c">C.</param>
	public void SetPenColor( Color c){
		penColor = c;
		if(paintType== PaintType.DrawLine){
			m_canvasMat.color=penColor;
		}
	}

	/// <summary>
	/// 设置画布颜色
	/// </summary>
	/// <param name="c">C.</param>
	public void SetPaintCanvasColor(Color c){
		m_canvasMat.color = c;
		m_canvasMat.SetFloat("_Alpha",c.a);
	}

	/// <summary>
	/// 显示原图
	/// </summary>
	public void ShowSourceTexture(){
		if(sourceTex){
			m_canvasMat.SetColor("_Color",new Color(1,1,1,1));
			RenderTexture.active = m_rt;
			Graphics.Blit(sourceTex,m_rt);
			RenderTexture.active = null;
		}
	}

	Material CreateMat(Shader shader ,Color c, BlendMode src , BlendMode dst , float alpha=1f){
		Material m = new Material(shader);
		m.SetFloat("_BlendSrc",(int)src);
		m.SetFloat("_BlendDst",(int)dst);
		m.SetColor("_Color",c);
		m.SetFloat("_Alpha",alpha);
		return m;
	}

	/// <summary>
	/// Draw一次
	/// </summary>
	/// <param name="screenPos">Screen position.</param>
	/// <param name="camera">Camera.</param>
	public void ClickDraw(Vector3 screenPos , Camera camera=null){
		if (camera == null) camera = Camera.main;
		Vector3 worldPos= SpriteHitPoint2UV(camera.ScreenToWorldPoint(screenPos));
		screenPos = new Vector3(worldPos.x * canvasWidth, canvasHeight - worldPos.y * canvasHeight,0f);

		GL.PushMatrix();
		GL.LoadPixelMatrix(0, canvasWidth, canvasHeight, 0);
		RenderTexture.active = m_rt;
		float w = penTex.width*brushScale;
		float h = penTex.height*brushScale;
		Graphics.DrawTexture(new Rect((screenPos.x-w*0.5f),(screenPos.y-h*0.5f),w,h),penTex,m_penMat);
		if(paintType == PaintType.Scribble){
			Graphics.Blit(sourceTex,m_rt,m_sourceMat,0);
		}
		RenderTexture.active = null;
		GL.PopMatrix();
	}

	/// <summary>
	/// 移动动时draw
	/// </summary>
	/// <param name="screenPos">Screen position.</param>
	/// <param name="camera">Camera.</param>
	public void Drawing(Vector3 screenPos , Camera camera=null){
		if (camera == null) camera = Camera.main;
		Vector3 worldPos= SpriteHitPoint2UV(camera.ScreenToWorldPoint(screenPos));
		screenPos = new Vector3(worldPos.x * canvasWidth, canvasHeight - worldPos.y * canvasHeight,0f);

		if(!m_isDown){
			m_isDown = true;
			m_prevMousePosition = screenPos;
		}

		if(m_isDown){
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, canvasWidth, canvasHeight, 0);
			RenderTexture.active = m_rt;
			LerpDraw(screenPos,m_prevMousePosition);
			if(!isEraser && paintType == PaintType.Scribble){
				Graphics.Blit(sourceTex,m_rt,m_sourceMat,0);
			}
			RenderTexture.active = null;
			GL.PopMatrix();
			m_prevMousePosition = screenPos;
		}
	}

	/// <summary>
	/// 结束draw
	/// </summary>
	public void EndDraw(){
		m_isDown = false;
	}

	/// <summary>
	/// Sprite中,hitPoint转uv坐标。hitPoint为世界坐标
	/// </summary>
	/// <returns>The hit point2 U.</returns>
	/// <param name="hitPoint">Hit point.</param>
	Vector2 SpriteHitPoint2UV( Vector3 hitPoint){
		Vector3 localPos=transform.InverseTransformPoint(hitPoint);
		localPos*=100f;
		localPos.x += canvasWidth*0.5f;
		localPos.y += canvasHeight*0.5f;
		return new Vector2(localPos.x/canvasWidth,localPos.y/canvasHeight);
	}

	void LerpDraw(Vector3 current , Vector3 prev){
		float w = penTex.width*brushScale;
		float h = penTex.height*brushScale;
		float distance = Vector2.Distance(current, prev);
		if(distance>0f){
			float lerpDamp = Mathf.Min(w,h)*0.02f;
			for (float i = 0; i < distance; i += lerpDamp)
			{
				float lDelta = i / distance;
				float lDifx = current.x - prev.x;
				float lDify = current.y - prev.y;
				Vector2 pos = new Vector2(prev.x + (lDifx * lDelta), prev.y + (lDify * lDelta));
				Graphics.DrawTexture(new Rect((pos.x-w*0.5f),(pos.y-h*0.5f),w,h),penTex,m_penMat);
			}
		}
	}


	public void ClearCanvas()
	{
		if(m_rt){
			Graphics.SetRenderTarget (m_rt);
			GL.Clear(true,true,canvasColor);
		}
	}


	public void Dispose(){
		if(m_rt){
			ClearCanvas();
			RenderTexture.active = null;
			m_rt.Release();
			m_rt = null;
		}

		if(m_canvasMat){
			Destroy(m_canvasMat);
		}
		if(m_penMat){
			Destroy(m_penMat);
		}
		if(m_sourceMat){
			Destroy(m_sourceMat);
		}
		m_sourceMat = null;
	}

	void OnDestroy(){
		if(isAutoDestroy){
			Dispose();
		}
	}


	void CreateQuad( Material mat){
		Mesh m = new Mesh();
		m.vertices = new Vector3[]{
			new Vector3(canvasWidth*0.005f ,canvasHeight*0.005f ),
			new Vector3(canvasWidth*0.005f ,-canvasHeight*0.005f ),
			new Vector3(-canvasWidth*0.005f ,-canvasHeight*0.005f),
			new Vector3(-canvasWidth*0.005f ,canvasHeight*0.005f )
		};
		m.uv=new Vector2[]{
			new Vector2(1,1),
			new Vector2(1,0),
			new Vector2(0,0),
			new Vector2(0,1)
		};
		m.triangles=new int[]{0,1,2,2,3,0};
		m.RecalculateBounds();
		m.RecalculateNormals();

		MeshFilter meshFilter= gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = m;

		MeshRenderer rend = gameObject.AddComponent<MeshRenderer>();
		rend.material = mat;
	}



	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
		Gizmos.matrix *= cubeTransform;
		Gizmos.DrawWireCube(Vector3.zero,new Vector3(canvasWidth*0.01f,canvasHeight*0.01f,0.1f));
		Gizmos.matrix = oldGizmosMatrix;
	}
}
