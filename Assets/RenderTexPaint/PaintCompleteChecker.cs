using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RenderTexturePainter))]
public class PaintCompleteChecker : MonoBehaviour {
	//网格初始值
	public bool gridDefaultStatus=false;

	//笔刷大小
	[Range(0.1f,1f)]
	public float brushSize = 0.2f;

	public Color enableColor = Color.blue;
	public Color disableColor = Color.yellow;

	public Dictionary<string,Rect> gridsDic;
	public Dictionary<string,bool> enablesDic;

	[Header("Asset File")]
	//拖文件到这上面
	public PaintRectDictionary assetRectDic;
	public PaintEnableDictionary assetEnableDic;

	private RenderTexturePainter m_painter;
	public RenderTexturePainter painter{
		get { return m_painter; }
	}

	private bool m_isDown;
	private Vector3 m_prevMousePosition;

	void Start(){
		m_painter = GetComponent<RenderTexturePainter>();
		if(assetRectDic!=null){
			gridsDic = assetRectDic.ConvertToDictionary();
			if(assetEnableDic!=null){
				enablesDic = assetEnableDic.ConvertToDictionary();
			}
		}
	}

	public void ClickDraw(Vector3 screenPos , Camera camera=null){
		if (camera == null) camera = Camera.main;
		Vector3 localPos= transform.InverseTransformPoint(camera.ScreenToWorldPoint(screenPos));
		foreach(string key in gridsDic.Keys)
		{
			Rect rect = gridsDic[key];
			if(rect.Contains(localPos)){
				if(enablesDic[key]){
					enablesDic[key] = false;
					//数量+1
				}
			}
		}
	}

	/// <summary>
	/// 移动动时draw
	/// </summary>
	/// <param name="screenPos">Screen position.</param>
	/// <param name="camera">Camera.</param>
	public void Drawing(Vector3 screenPos , Camera camera=null){
		if (camera == null) camera = Camera.main;
		Vector3 localPos= transform.InverseTransformPoint(camera.ScreenToWorldPoint(screenPos));

		if(!m_isDown){
			m_isDown = true;
			m_prevMousePosition = localPos;
		}

		if(m_isDown){
			LerpDraw(localPos,m_prevMousePosition);
			m_prevMousePosition = localPos;
		}
	}

	/// <summary>
	/// 结束draw
	/// </summary>
	public void EndDraw(){
		m_isDown = false;
	}


	void LerpDraw(Vector3 current , Vector3 prev){
		float distance = Vector2.Distance(current, prev);
		if(distance>0f){
			float w = m_painter.penTex.width*m_painter.brushScale*0.002f;
			float h = m_painter.penTex.height*m_painter.brushScale*0.002f;
	
			float lerpDamp = Mathf.Min(w,h);
			Vector2 pos;
			for (float i = 0; i < distance; i += lerpDamp)
			{
				float lDelta = i / distance;
				float lDifx = current.x - prev.x;
				float lDify = current.y - prev.y;
				pos.x = prev.x + (lDifx * lDelta);
				pos.y = prev.y + (lDify * lDelta);

				Rect brushSize = new Rect((pos.x-w*0.5f),(pos.y-h*0.5f),w,h);
				foreach(string key in gridsDic.Keys)
				{
					Rect rect = gridsDic[key];
					if(Intersect(brushSize,rect)){
						if(enablesDic[key]){
							enablesDic[key] = false;
							//数量+1

						}
					}
				}
			}
		}
	}

	public bool Intersect( Rect a, Rect b ) {
		FlipNegative( ref a );
		FlipNegative( ref b );
		bool c1 = a.xMin < b.xMax;
		bool c2 = a.xMax > b.xMin;
		bool c3 = a.yMin < b.yMax;
		bool c4 = a.yMax > b.yMin;
		return c1 && c2 && c3 && c4;
	}

	void FlipNegative(ref Rect r) {
		if( r.width < 0 ) 
			r.x -= ( r.width *= -1 );
		if( r.height < 0 )
			r.y -= ( r.height *= -1 );
	}




	void OnDrawGizmos(){
		if(gridsDic!=null && enablesDic!=null){

			Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
			Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
			Gizmos.matrix *= cubeTransform;

			foreach(string key in gridsDic.Keys)
			{
				Rect rect = gridsDic[key];
				if(enablesDic[key]){
					Gizmos.color = enableColor;
				}
				else{
					Gizmos.color = disableColor;
				}
				Vector3 center = new Vector3(rect.x+rect.width*0.5f,rect.y+rect.height*0.5f);
				Vector3 size = new Vector3(rect.width,rect.height,0.1f);

				Gizmos.DrawWireCube(center,size);
			}

			Gizmos.matrix = oldGizmosMatrix;
		}
	}
}
