using UnityEngine;
using System.Collections;

public class RenderPainterController : MonoBehaviour {

	public RenderTexturePainter painter;
	private bool m_isEraser = false;

	// Use this for initialization
	void Start () {
		m_isEraser = painter.isEraser;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){
			painter.StartDraw(Input.mousePosition);
		}
		if(Input.GetMouseButtonUp(0)){
			painter.EndDraw();
		}
	}


	void OnGUI(){
		if(GUI.Button(new Rect(10,10,100,30),"Clear")){
			painter.ClearCanvas();
		}

		painter.brushScale = GUI.HorizontalSlider(new Rect(10, 80, 200, 30), painter.brushScale , 0.1F, 5F);
	}
}
