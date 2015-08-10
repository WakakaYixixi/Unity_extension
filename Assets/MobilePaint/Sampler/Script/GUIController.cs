using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour {


    public Texture2D drawImage;

    private Painter _paint;
    private bool _selectedEraser;
    private bool _selectedDrawImg;

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
