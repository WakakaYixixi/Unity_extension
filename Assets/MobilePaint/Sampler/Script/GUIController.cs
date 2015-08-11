using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour {


    public Texture2D drawImage;

    private Painter _paint;
    private bool _selectedEraser;
    private bool _selectedDrawImg;
	private bool _selectdPenAlpha;
	private bool _isDrawLine;
	private bool _isYellow;

	private 
	// Use this for initialization
	void Start () {
		_paint = GetComponent<Painter> ();
		Application.targetFrameRate = 60;
	}
	
	void OnGUI()
    {
        _selectedDrawImg = GUI.Toggle(new Rect(0, 0, 100, 30), _selectedDrawImg, "Is Draw Img");
        if (!_selectedDrawImg)
        {
            _selectedEraser = GUI.Toggle(new Rect(0, 30, 100, 30), _selectedEraser, "Is Eraser");
            _paint.isEraser = _selectedEraser;

			_selectdPenAlpha = GUI.Toggle(new Rect(120, 30, 100, 30), _selectdPenAlpha, "Pen Alpha");
			_paint.penAlphaEnable = _selectdPenAlpha;

			_isDrawLine = GUI.Toggle(new Rect(120, 0, 100, 30), _isDrawLine, "Is Draw Line");
			if(_isDrawLine){
				_paint.paintType= Painter.PaintType.DrawLine;

				_isYellow = GUI.Toggle(new Rect(240, 0, 100, 30), _isYellow, "Change to Yellow");
				if(_isYellow){
					_paint.paintColor = new Color32(0xff,0xcc,0,0xff);
				}
				else
				{
					_paint.paintColor =new Color32(0xff, 0, 0, 0xff);
				}

			}else{
				_paint.paintType= Painter.PaintType.Scribble;
			}
        }
	}

 
    //test draw img
    void Update()
    {
        if (_selectedDrawImg)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Vector2 point = hit.textureCoord;
                    _paint.DrawTexture(drawImage, point.x, point.y);
                }
            }
        }
    }

}
