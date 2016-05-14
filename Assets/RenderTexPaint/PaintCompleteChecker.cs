using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 涂抹完成判断
/// </summary>
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
	private Vector2 m_lerpSize ;
	private List<string> m_keyList = new List<string>();
	private int m_totalCount;

	/// <summary>
	/// 完成的进度 0-1
	/// </summary>
	/// <value>The progress.</value>
	public float Progress{
		get {return 1f-(float)gridsDic.Count/m_totalCount; }
	}

	void Start(){
		m_painter = GetComponent<RenderTexturePainter>();
		Reset();
	}

	/// <summary>
	/// Reset
	/// </summary>
	public void Reset(){
		if(assetRectDic!=null){
			gridsDic = assetRectDic.ConvertToDictionary();
			if(assetEnableDic!=null){
				enablesDic = assetEnableDic.ConvertToDictionary();
			}
			m_totalCount = gridsDic.Count;
		}

		float w = m_painter.penTex.width*m_painter.brushScale*0.005f;
		float h = m_painter.penTex.height*m_painter.brushScale*0.005f;
		m_lerpSize = new Vector2(w,h); 
	}

	public void ClickDraw(Vector3 screenPos , Camera camera=null){
		if (camera == null) camera = Camera.main;
		Vector3 localPos= transform.InverseTransformPoint(camera.ScreenToWorldPoint(screenPos));

		float w = m_lerpSize.x;
		float h = m_lerpSize.y;
		float lerpDamp = Mathf.Min(w,h);
		Rect brushSize = new Rect((localPos.x-w*0.5f),(localPos.y-h*0.5f),w,h);
		foreach(string key in gridsDic.Keys)
		{
			Rect rect = gridsDic[key];
			if(Vector2.Distance(rect.center,brushSize.center)<lerpDamp*0.75f){
				if(enablesDic[key]){
					enablesDic[key] = false;
					m_keyList.Add(key);
				}
			}
		}
		//移除完成部分
		int count = m_keyList.Count;
		for(int i=0;i<count;++i){
			gridsDic.Remove(m_keyList[i]);
		}
		m_keyList.Clear();
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
			float w = m_lerpSize.x;
			float h = m_lerpSize.y;
	
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
					if(Vector2.Distance(rect.center,brushSize.center)<lerpDamp*0.75f){
						if(enablesDic[key]){
							enablesDic[key] = false;
							m_keyList.Add(key);
						}
					}
				}
			}
			//移除完成部分
			int count = m_keyList.Count;
			for(int i=0;i<count;++i){
				gridsDic.Remove(m_keyList[i]);
			}
			m_keyList.Clear();
		}
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
