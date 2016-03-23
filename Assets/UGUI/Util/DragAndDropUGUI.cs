using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDropUGUI: MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler{

	private Vector3 m_dragPos;
	private bool m_isDown = false;
	private Vector3 m_worldPos;

	[Tooltip("拖动的对象，默认为自己.")]
	public RectTransform dragTarget;

	[Tooltip("在拖动时是否固定在拖动物的原点.")]
	public bool isDragOriginPoint = false;

	[Tooltip("当isDragOriginPoint为true时，拖动时的偏移值.")]
	public Vector2 dragOffset;

	[Tooltip("拖动时的缓动参数.")]
	public float dragMoveDamp = 0.5f;

	[Tooltip("触发坐标，默认为当前对象")]
	public Transform triggerPos ;

	//要发送的事件名字
	[Header("Event")]
	public bool sendHoverEvent = false;
	public string onHoverMethodName = "OnHover";
	public string onDropMethodName = "OnDrop";

	// Use this for initialization
	void Start () {
		if(!dragTarget){
			dragTarget =  transform as RectTransform;;
		}
		if(!triggerPos){
			triggerPos = transform;
		}
	}
	
	public void OnBeginDrag(PointerEventData eventData)
	{
		m_dragPos = dragTarget.localPosition;
		m_worldPos = dragTarget.position;
		m_isDown = true ;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.dragging)
		{
			if(isDragOriginPoint){
				RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTarget,eventData.position,Camera.main,out m_worldPos);
				m_worldPos += (Vector3)dragOffset;
			}else{
				m_dragPos += (Vector3)eventData.delta;
			}
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		m_isDown = false ;
	}

	void Update(){
		if(m_isDown){
			if(isDragOriginPoint){
				dragTarget.position = Vector3.Lerp(dragTarget.position,m_worldPos,dragMoveDamp);
			}else{
				dragTarget.localPosition = Vector3.Lerp(dragTarget.localPosition,m_dragPos,dragMoveDamp);
			}
		}
	}
}
