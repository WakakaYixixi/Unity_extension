using UnityEngine;
using System.Collections;

public class MapLayer : MonoBehaviour {

	public Vector2 size;//地图的大小

	[Header("Drag Setting")]
	public bool dragEnable = true;//是否可拖动
	public float mapMoveSpeed = 1f; //移动地图时的速度
	public bool freezeX=false; //X方向上是否不准移动
	public bool freezeY=false; //Y方向上是否不准移动

	[Header("Scale Setting")]
	public bool multiScaleEnable = true;//是否支持多点缩放
	public float minScale=1f;//最小scale
	public float maxScale=1f;//最大scale

	[Header("Init Center Position")]
	public bool center = false;
	public float centerOffsetX = 0f;
	public float centeroffsetY = 0f;

	private SpriteMapViewport m_viewPort;
	private Vector3 m_prevPos ;
	private Vector3 m_endPos;
	private Matrix2D m_matrix;
	private Vector3 m_initPos;
	private bool m_reset = false;
	private bool m_isAutoMoved = false;//是否在自动移动中.

	void Awake(){
		m_viewPort = GetComponentInParent<SpriteMapViewport>();
	}

	// Use this for initialization
	void Start () {
		m_endPos = transform.localPosition;
		m_matrix = new Matrix2D();
		m_initPos = transform.localPosition;

		if(center){
			MovePointToCenter(size*0.5f,centerOffsetX,centeroffsetY);
			m_isAutoMoved = false;
			transform.localPosition = m_endPos;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(m_isAutoMoved){
			transform.localPosition = Vector3.Lerp(transform.localPosition,m_endPos,mapMoveSpeed*5f*Time.deltaTime);
			if(Vector3.Distance(transform.localPosition,m_endPos)<0.01f){
				m_isAutoMoved = false;
			}
			return;
		}

		if(!InputUtil.CheckMouseOnUGUI()){
			if(Input.touchCount<2)
			{
				if(dragEnable){
					if(Input.GetMouseButtonDown(0)){
						OnTouchDown();
					}
					if(Input.GetMouseButton(0)){
						OnTouchMove();
					}
				}
			}
			else if(multiScaleEnable)
			{
				m_reset = true;
				Touch t1 = Input.touches[0];
				Touch t2 = Input.touches[1];
				Vector2 t1PrevPos = t1.deltaPosition+t1.position;
				Vector2 t2PrevPos = t2.deltaPosition+t2.position;
				float delta = Vector2.Distance(t1PrevPos,t2PrevPos)/Vector2.Distance(t1.position,t2.position);
				delta*=delta;

				Vector3 localPos = m_viewPort.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				FixScaleSize(delta,localPos.x,localPos.y);
			}

			if(dragEnable && Input.GetAxis("Mouse ScrollWheel") != 0)
			{  
				float delta = 1+Input.GetAxis("Mouse ScrollWheel");
				Vector3 localPos = m_viewPort.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				FixScaleSize(delta,localPos.x,localPos.y);
			}
		}

		float speed = mapMoveSpeed;
		if(Application.platform== RuntimePlatform.Android || Application.platform== RuntimePlatform.IPhonePlayer){
			if(Input.touchCount==0){
				speed = mapMoveSpeed*0.5f;
				m_endPos = Vector3.Lerp(m_endPos,transform.localPosition,speed*10f*Time.deltaTime);
			}
		}

		Vector3 resultPos = Vector3.Lerp(transform.localPosition,m_endPos,speed*Time.deltaTime);
		if (resultPos.x>0) resultPos.x=0;
		else if(resultPos.x<-size.x*transform.localScale.x+m_viewPort.viewPort.width/transform.root.localScale.x)
			resultPos.x = -size.x*transform.localScale.x+m_viewPort.viewPort.width/transform.root.localScale.x;
		
		if (resultPos.y>0) resultPos.y=0;
		else if(resultPos.y<-size.y*transform.localScale.y+m_viewPort.viewPort.height/transform.root.localScale.y)
			resultPos.y = -size.y*transform.localScale.y+m_viewPort.viewPort.height/transform.root.localScale.y;

		if(freezeY){
			resultPos.y = m_initPos.y;
		}
		if(freezeX){
			resultPos.x = m_initPos.x;
		}
		transform.localPosition = resultPos;
	}

	void OnTouchDown(){
		m_prevPos = Input.mousePosition;
		m_reset = false;
	}

	void OnTouchMove(){
		if(m_reset){
			m_prevPos = Input.mousePosition;
			return;
		}
		Vector3 delta = Input.mousePosition-m_prevPos;
		m_endPos = transform.localPosition + delta;
		m_prevPos = Input.mousePosition;
	}

	void FixScaleSize(float sizeDiff,float middleX,float middleY){
		float resultSc = transform.localScale.x*sizeDiff;
		while(resultSc<minScale || resultSc>maxScale){
			sizeDiff/=sizeDiff;
			resultSc = transform.localScale.x*sizeDiff;
		}
		ScaleMap(sizeDiff,middleX,middleY);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		Gizmos.DrawLine(new Vector3(transform.position.x,transform.position.y,0),
			new Vector3(transform.position.x,size.y*transform.localScale.y*transform.root.localScale.y+transform.position.y,0));
		
		Gizmos.DrawLine(new Vector3(transform.position.x,size.y*transform.localScale.y*transform.root.localScale.y+transform.position.y,0),
			new Vector3(transform.position.x+size.x*transform.localScale.x*transform.root.localScale.x,size.y*transform.localScale.y*transform.root.localScale.y+transform.position.y,0));
		
		Gizmos.DrawLine(new Vector3(transform.position.x+size.x*transform.localScale.x*transform.root.localScale.x,size.y*transform.localScale.y*transform.root.localScale.y+transform.position.y,0),
			new Vector3(transform.position.x+size.x*transform.localScale.x*transform.root.localScale.x,transform.position.y,0));
		
		Gizmos.DrawLine(new Vector3(transform.position.x+size.x*transform.localScale.x*transform.root.localScale.x,transform.position.y,0),
			new Vector3(transform.position.x,transform.position.y,0));
	}




	#region public function

	/// <summary>
	/// Middle坐标需要是Viewport层的局部坐标
	/// </summary>
	/// <param name="scale">要缩放的大小.</param>
	/// <param name="middleX">Middle x.</param>
	/// <param name="middleY">Middle y.</param>
	public void ScaleMap(float scale,float middleX,float middleY){
		m_matrix.Identity();
		m_matrix.Scale(transform.localScale.x,transform.localScale.y);
		m_matrix.Translate(transform.localPosition.x,transform.localPosition.y);
		m_matrix.tx -= middleX;
		m_matrix.ty -= middleY;

		m_matrix.Scale(scale,scale);

		m_matrix.tx += middleX;
		m_matrix.ty += middleY;

		transform.localScale = new Vector3(m_matrix.a,m_matrix.d,1f);
		transform.localPosition = new Vector3(m_matrix.tx,m_matrix.ty,transform.localPosition.z);

		m_endPos.x = m_matrix.tx;
		m_endPos.y = m_matrix.ty;
	}


	/// <summary>
	/// 把一个点移动到viewport中间 , point是MapLayer层的局部坐标
	/// </summary>
	/// <param name="middleX">Middle x.</param>
	/// <param name="middleY">Middle y.</param>
	public void MovePointToCenter(Vector2 point , float offsetX= 0f,float offsetY=0f){
		//viewport 的中点
		Vector2 viewportCenter = new Vector2(m_viewPort.viewPort.width*0.5f/transform.root.localScale.x,m_viewPort.viewPort.height*0.5f/transform.root.localScale.y);
		point.x = transform.localPosition.x+point.x*transform.localScale.x+offsetX;
		point.y = transform.localPosition.y+point.y*transform.localScale.y+offsetY;
		m_endPos.x = transform.localPosition.x+viewportCenter.x-point.x;
		m_endPos.y = transform.localPosition.y+viewportCenter.y-point.y;

		m_isAutoMoved = true;

		if (m_endPos.x>0) m_endPos.x=0;
		else if(m_endPos.x<-size.x*transform.localScale.x+m_viewPort.viewPort.width)
			m_endPos.x = -size.x*transform.localScale.x+m_viewPort.viewPort.width;

		if (m_endPos.y>0) m_endPos.y=0;
		else if(m_endPos.y<-size.y*transform.localScale.y+m_viewPort.viewPort.height)
			m_endPos.y = -size.y*transform.localScale.y+m_viewPort.viewPort.height;

		if(freezeY){
			m_endPos.y = m_initPos.y;
		}
		if(freezeX){
			m_endPos.x = m_initPos.x;
		}
	}

	/// <summary>
	/// 移动到某一个点, point是MapLayer层的局部坐标
	/// </summary>
	/// <param name="point">Point.</param>
	public void MoveTo(Vector2 point)
	{
		point.x = transform.localPosition.x+point.x*transform.localScale.x;
		point.y = transform.localPosition.y+point.y*transform.localScale.y;
		m_endPos.x = transform.localPosition.x-point.x;
		m_endPos.y = transform.localPosition.y-point.y;

		m_isAutoMoved = true;

		if (m_endPos.x>0) m_endPos.x=0;
		else if(m_endPos.x<-size.x*transform.localScale.x+m_viewPort.viewPort.width)
			m_endPos.x = -size.x*transform.localScale.x+m_viewPort.viewPort.width;

		if (m_endPos.y>0) m_endPos.y=0;
		else if(m_endPos.y<-size.y*transform.localScale.y+m_viewPort.viewPort.height)
			m_endPos.y = -size.y*transform.localScale.y+m_viewPort.viewPort.height;

		if(freezeY){
			m_endPos.y = m_initPos.y;
		}
		if(freezeX){
			m_endPos.x = m_initPos.x;
		}

	}

	#endregion
}
