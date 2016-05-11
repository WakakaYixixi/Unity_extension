using UnityEngine;
using System.Collections;

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

	//画布大小
	public float canvasWidth=512f;
	public float canvasHeight=512f;

	//默认背景颜色
	public Color canvasColor = Color.white;

	public int renderTextureDepth = 16;
	public RenderTextureFormat renderTextureformat=RenderTextureFormat.ARGB32;

	public PaintType paintType = PaintType.Scribble;
	//画笔方式.
	public Texture2D pen, source;

	public Material canvasMat,penMat,sourceMat;

	//是否为擦除.
	public bool isEraser = false;
	//纯色方式.
	public Color paintColor=new Color(1, 0, 0, 1);
	//彩色方式
	public Color[] paintColorful ;

	//速度变化频率，越大变化越慢
	public float colorChangeRate = 1f;
	private int m_colorfulIndex = 1;
	private float m_colorfulTime = 0f;

	//笔刷缩放值
	public float brushScale = 1f;

	//是否自动初始化.
	public bool isAutoInit = false;

	//init show picture
	public bool initShowSource = false;


	private RenderTexture m_rt;
	private bool m_inited = false;
	private Material m_canvasMat,m_penMat,m_sourceMat;

	// Use this for initialization
	void Start () {
		if (isAutoInit) {
			Init(initShowSource);
		}
	}

	public void Init(bool isShowSource=false)
	{
		if(m_inited){
			m_rt = new RenderTexture(canvasWidth,canvasHeight,renderTextureDepth,renderTextureformat);
			m_rt.useMipMap = false;
			m_rt.antiAliasing=1;
			m_rt.anisoLevel =1 ;

			m_canvasMat = (Material) Instantiate(canvasMat);
			CreateQuad(m_canvasMat);
			m_canvasMat.mainTexture = m_canvasMat;


		}
	}
	
	// Update is called once per frame
	void Update () {
	
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
			m_rt.Release();
			ClearCanvas = null;
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
