using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RenderTexturePainter))]
public class RenderPainterController : MonoBehaviour {

	private RenderTexturePainter m_painter;
	private bool m_isEraser = false;

	// Use this for initialization
	void Start () {
		m_painter=GetComponent<RenderTexturePainter>();
		m_isEraser = m_painter.isEraser;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){
			m_painter.StartDraw(Input.mousePosition);
		}
		if(Input.GetMouseButtonUp(0)){
			m_painter.EndDraw();
		}
	}


	void OnGUI(){
		if(GUI.Button(new Rect(10,10,100,30),"Clear")){
			m_painter.ClearCanvas();
		}
		m_isEraser = GUI.Toggle(new Rect(120,10,100,30),m_isEraser,"Is Earse");
		if(m_isEraser!=m_painter.isEraser){
			m_painter.SetIsEraser(m_isEraser);
		}

		m_painter.brushScale = GUI.HorizontalSlider(new Rect(10, 80, 200, 30), m_painter.brushScale , 0.1F, 5F);
	}
}
