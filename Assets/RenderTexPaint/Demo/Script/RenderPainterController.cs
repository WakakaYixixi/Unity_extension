using UnityEngine;
using System.Collections;

/// <summary>
/// 用来测试RenderTexturePainter和PaintCompleteChecker
/// </summary>
public class RenderPainterController : MonoBehaviour {

	public RenderTexturePainterEx painterEx;
	public RenderTexturePainter painter;
//	public PaintCompleteChecker checker;
	private bool m_isEraser = false;
	private float m_alpha = 1f;
	private bool m_clickDraw = false;

	public Texture[] penTexs;
	private int m_penTexIndex;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate=60;
		if(painter) 
			m_isEraser = painter.isEraser;
		if(painterEx) 
			m_isEraser = painterEx.isEraser;
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0)){
			if(painterEx&&m_clickDraw) {
				painterEx.ClickDraw(Input.mousePosition);
			}
		}

		if(Input.GetMouseButton(0)){
			if(painter) 
				painter.Drawing(Input.mousePosition);
			if(painterEx) {
				if(!m_clickDraw)
					painterEx.Drawing(Input.mousePosition);
			}
//			checker.Drawing(Input.mousePosition);
		}
		if(Input.GetMouseButtonUp(0)){
			if(painter) 
				painter.EndDraw();
			if(painterEx) 
				painterEx.EndDraw();
//			checker.EndDraw();
		}
	}


	void OnGUI(){
		if(painter){
			if(GUI.Button(new Rect(10,10,100,30),"Clear")){
				painter.ClearCanvas();
			}
			m_isEraser = GUI.Toggle(new Rect(120,10,100,30),m_isEraser,"Is Earse");
			if(m_isEraser!=painter.isEraser){
				painter.SetIsEraser(m_isEraser);
			}
			painter.brushScale = GUI.HorizontalSlider(new Rect(10, 80, 200, 30), painter.brushScale , 0.1F, 5F);

		}
		else if(painterEx)
		{

			if(GUI.Button(new Rect(10,10,100,30),"Clear")){
				painterEx.ClearCanvas();
			}
			m_isEraser = GUI.Toggle(new Rect(120,10,100,30),m_isEraser,"Is Earse","Button");
			if(m_isEraser!=painterEx.isEraser){
				painterEx.SetIsEraser(m_isEraser);
			}
			m_clickDraw = GUI.Toggle(new Rect(240,10,100,30),m_clickDraw,"Click Draw","Button");

			GUI.color = Color.white;
			GUI.Label( new Rect(10, 60, 200, 30) ,"Brush Scale :"+painterEx.brushScale.ToString("N2"));
			painterEx.brushScale = GUI.HorizontalSlider(new Rect(10, 80, 200, 30), painterEx.brushScale , 0.1F, 5F);

			GUI.color = Color.white;
			GUI.Label( new Rect(10, 100, 200, 30) ,"Canvas Alpha :"+m_alpha.ToString("N2"));
			m_alpha = GUI.HorizontalSlider(new Rect(10, 120, 200, 30), m_alpha , 0F, 1F);
			painterEx.SetCanvasAlpha(m_alpha);

			if(penTexs.Length>1 && GUI.Button(new Rect(220,60,150,30),"Change PenTexture")){
				++m_penTexIndex;
				if(m_penTexIndex>=penTexs.Length) m_penTexIndex = 0;
				painterEx.penTex = penTexs[m_penTexIndex];
			}
		}
	}
}
