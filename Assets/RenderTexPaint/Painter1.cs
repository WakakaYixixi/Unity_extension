using UnityEngine;
using System.Collections;

public class Painter1 : MonoBehaviour {

	public Texture pen ;
	public Vector2 size = new Vector2(500f,400f);
	public float penScale=1f;
	public Material penMat;
	public Shader shader;

	private float lerpDamp = 1f;
	private RenderTexture rt;
	private Vector3 _prevMousePosition;

	private float penW,penH;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 60;

		Material mat = new Material(shader);
		CreateQuad(mat);

		rt = new RenderTexture((int)size.x,(int)size.y,16,RenderTextureFormat.ARGB32);
		rt.useMipMap = false;
		rt.antiAliasing=1;
		rt.anisoLevel =1 ;
		Clear(rt);
		mat.SetTexture("_MainTex",rt);

		penW = pen.width;
		penH = pen.height;
	}

	void CreateQuad( Material mat){
		Mesh m = new Mesh();
		m.vertices = new Vector3[]{
			new Vector3(size.x*0.005f ,size.y*0.005f ),
			new Vector3(size.x*0.005f ,-size.y*0.005f ),
			new Vector3(-size.x*0.005f ,-size.y*0.005f),
			new Vector3(-size.x*0.005f ,size.y*0.005f )
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


	bool isDown = false;
	Vector3 screenPos;
	void Update(){

		Vector3 worldPos= SpriteHitPoint2UV(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		screenPos = new Vector3(worldPos.x * size.x, size.y - worldPos.y * size.y,0f);

		if(Input.GetMouseButtonDown(0)){
			if(!isDown){
				isDown = true;
				_prevMousePosition = screenPos ;
			}
		}else if(Input.GetMouseButtonUp(0)){
			isDown = false;
		}

		if(isDown){
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, size.x, size.y, 0);
			RenderTexture.active = rt;
			penMat.color=Color.red;
			LerpDraw(screenPos,_prevMousePosition);
			RenderTexture.active = null;
			GL.PopMatrix();
			_prevMousePosition = screenPos ;
		}
	}

	/// <summary>
	/// Sprite中,hitPoint转uv坐标。hitPoint为世界坐标
	/// </summary>
	/// <returns>The hit point2 U.</returns>
	/// <param name="hitPoint">Hit point.</param>
	public Vector2 SpriteHitPoint2UV( Vector3 hitPoint){
		Vector3 localPos=transform.InverseTransformPoint(hitPoint);
		localPos*=100f;
		localPos.x += size.x*0.5f;
		localPos.y += size.y*0.5f;
		return new Vector2(localPos.x/size.x,localPos.y/size.y);
	}

	void LerpDraw(Vector3 current , Vector3 prev){

		float w = penW*penScale;
		float h = penH*penScale;

		lerpDamp= Mathf.Min(w,h)*0.02f;

		float distance = Vector2.Distance(current, prev);
		for (float i = 0; i < distance; i += lerpDamp)
		{
			float lDelta = i / distance;
			float lDifx = current.x - prev.x;
			float lDify = current.y - prev.y;
			Vector2 pos = new Vector2(prev.x + (lDifx * lDelta), prev.y + (lDify * lDelta));

			Graphics.DrawTexture(new Rect((pos.x-w*0.5f),(pos.y-h*0.5f),w,h),pen,penMat);
		}
	}



	void Clear(RenderTexture destTexture)
	{
		Graphics.SetRenderTarget (destTexture);
		GL.Clear(true,true,new Color(0,0,0,1));
	}



	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
		Gizmos.matrix *= cubeTransform;
		Gizmos.DrawWireCube(Vector3.zero,new Vector3(size.x*0.01f,size.y*0.01f,0.1f));
		Gizmos.matrix = oldGizmosMatrix;
	}

	void OnGUI(){
		if(GUI.Button(new Rect(10,10,100,50),"Clear")){
			Clear(rt);
		}
		penScale = GUI.HorizontalSlider(new Rect(10, 100, 200, 40), penScale, 0.1F, 0.5F);
	}
}