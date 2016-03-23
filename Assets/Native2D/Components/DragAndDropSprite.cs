using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Drag and drop sprite.
/// Author :zhouzhanglin
/// </summary>
public class DragAndDropSprite : MonoBehaviour {

	private Vector3 m_cachePosition;
	private Vector3 m_cacheScale;
	private Vector3 m_dragOffset;
	private Vector3 m_screenPosition;
	private Vector3 m_currentPosition;
	private bool m_isDown;
	private Transform m_trans;
	private SpriteRenderer m_spriteRender;
	private string m_sortLayerName;

	[Tooltip("拖动的对象，默认为自己.")]
	public Transform dragTarget = null;

	[Tooltip("如果为null，则使用mainCamera.")]
	public Camera rayCastCamera = null;

	[Tooltip("射线检测的Layer")]
	public LayerMask rayCastMask;

	[Header("Drag Setting")]
	[Tooltip("在拖动时是否固定在拖动物的原点.")]
	public bool isDragOriginPoint = false;

	[Tooltip("当isDragOriginPoint为true时，拖动时的偏移值.")]
	public Vector2 dragOffset;

	[Tooltip("主要用于影响层级显示.")]
	public float dragOffsetZ=0f;

	[Tooltip("拖动时的缓动参数.")]
	public float dragMoveDamp = 0.5f;

	[Tooltip("拖动的时候在哪个层.没有设置的话为当前Sort Layer")]
	public string dragSortLayerName;

	[Tooltip("触发坐标，默认为当前对象")]
	public Transform triggerPos ;

	//要发送的事件名字
	[Header("Event")]
	public bool sendHoverEvent = false;
	public string onHoverMethodName = "OnHover";
	public string onDropMethodName = "OnDrop";

	public event Action<DragAndDropSprite> OnMouseDownAction = null ;
	public event Action<DragAndDropSprite> OnMouseDragAction = null ;
	public event Action<DragAndDropSprite> OnMouseUpAction = null ;

	// Use this for initialization
	void Start () {
		if (dragTarget){
			m_trans = dragTarget;
		}else{
			m_trans = transform;
			dragTarget = m_trans;
		}
		if(!triggerPos){
			triggerPos = m_trans;
		}
		m_spriteRender = m_trans.GetComponent<SpriteRenderer>();
		m_sortLayerName = m_spriteRender.sortingLayerName;
		if(string.IsNullOrEmpty(dragSortLayerName)){
			dragSortLayerName = m_sortLayerName;
		}

		if (!rayCastCamera)
		{
			rayCastCamera = Camera.main;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!this.isActiveAndEnabled) return;
		if (Input.touchCount < 2)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (!m_isDown)
				{
					RaycastHit2D hit = Physics2D.Raycast(rayCastCamera.ScreenToWorldPoint(Input.mousePosition),Vector2.zero, 0, rayCastMask);
					if (hit && hit.collider.gameObject == dragTarget.gameObject)
					{
						m_isDown = true;
						OnMouseDownHandler();
					}
				}
			}
			else if (m_isDown && Input.GetMouseButton(0))
			{
				OnMouseDragHandler();
			}
			else if (m_isDown && Input.GetMouseButtonUp(0))
			{
				m_isDown = false;
				OnMouseUpHandler();
			}
		}
	}

	void OnMouseDownHandler(){
		m_trans.position+=new Vector3(0,0,dragOffsetZ);

		m_cachePosition = m_trans.position;
		m_cacheScale = m_trans.localScale;
		m_currentPosition = m_cachePosition;
		m_dragOffset = Vector3.zero;
		m_spriteRender.sortingLayerName=dragSortLayerName;

		StopAllCoroutines();
		m_screenPosition = rayCastCamera.WorldToScreenPoint(m_trans.position);
		if (!isDragOriginPoint)
		{
			m_dragOffset = m_trans.position - rayCastCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_screenPosition.z));
		}
		if (OnMouseDownAction!=null)
		{
			OnMouseDownAction(this);
		}
	}

	void OnMouseDragHandler(){
		m_screenPosition = rayCastCamera.WorldToScreenPoint(m_trans.position);
		Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_screenPosition.z);
		m_currentPosition = rayCastCamera.ScreenToWorldPoint(curScreenSpace);
		if (!isDragOriginPoint)
		{
			m_currentPosition += m_dragOffset;
		}
		else
		{
			m_currentPosition += (Vector3)dragOffset;
		}
		m_trans.position = Vector3.Lerp(m_trans.position, m_currentPosition, dragMoveDamp);
		if(sendHoverEvent){
			Collider2D[] cols = Physics2D.OverlapPointAll(triggerPos.position,rayCastMask,-100f,100f);
			if(cols.Length>0){
				foreach(Collider2D col in cols){
					if(col.gameObject!=gameObject)
						col.SendMessage(onHoverMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
				}
				gameObject.SendMessage(onHoverMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
			}
		}
		if (OnMouseDragAction!=null)
		{
			OnMouseDragAction(this);
		}
	}

	void OnMouseUpHandler(){
		m_trans.position -=new Vector3(0,0,dragOffsetZ);

		m_spriteRender.sortingLayerName=m_sortLayerName;
		Collider2D[] cols = Physics2D.OverlapPointAll(triggerPos.position,rayCastMask,-100f,100f);
		if(cols.Length>0){
			foreach(Collider2D col in cols){
				if(col.gameObject!=gameObject)
					col.SendMessage(onDropMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
			}
			gameObject.SendMessage(onDropMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
		}
		if (OnMouseUpAction!=null)
		{
			OnMouseUpAction(this);
		}
	}
}
