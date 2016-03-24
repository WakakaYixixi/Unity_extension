using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

/// <summary>
/// 用于拖动UGUI控件
/// </summary>
public class UGUIDrag: MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler{

	public enum DragBackEffect{
		None,Immediately, TweenPosition, TweenScale , ScaleDestroy , FadeOutDestroy , Destroy
	}

	private bool m_isDown = false;
	private Vector3 m_cachePosition;
	private Vector3 m_cacheScale;
	private Vector3 m_worldPos;
	private Vector3 m_touchDownTargetOffset ;
	private Transform m_parent;

	[Tooltip("拖动的对象，默认为自己.")]
	public RectTransform dragTarget;

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

	[Tooltip("Drag时的变化的大小.")]
	public float dragChangeScale = 1f;

	[Tooltip("拖动时的缓动参数.")]
	[Range(0f,1f)]
	public float dragMoveDamp = 0.5f;

	[Tooltip("拖动时的所在的父窗器，用于拖动时在UI最上层，如果不填，则在当前层.")]
	public string dragingParent = "Canvas";

	[Tooltip("触发坐标，默认为当前对象")]
	public Transform triggerPos ;

	//要发送的事件名字
	[Header("Event")]
	public bool sendHoverEvent = false;
	public string onHoverMethodName = "OnHover";
	public string onDropMethodName = "OnDrop";

	[Header("Back Effect")]
	[Tooltip("释放时，是否自动返回")]
	public bool releaseAutoBack = false;
	[Tooltip("返回时的效果")]
	public DragBackEffect backEffect = DragBackEffect.None;
	[Tooltip("效果时间")]
	public float backDuring = 0.25f;
	[Tooltip("Tween 的效果")]
	public Ease tweenEase = Ease.Linear;

	public event Action<UGUIDrag> OnMouseDownAction = null ;
	public event Action<UGUIDrag> OnMouseDragAction = null ;
	public event Action<UGUIDrag> OnMouseUpAction = null ;
	public event Action<UGUIDrag> OnTweenBackAction = null ;

	// Use this for initialization
	void Start () {
		if(!dragTarget){
			dragTarget =  transform as RectTransform;;
		}
		if(!triggerPos){
			triggerPos = dragTarget;
		}
		if (!rayCastCamera)
		{
			rayCastCamera = Camera.main;
		}
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		m_cachePosition = dragTarget.position;
		m_cacheScale = dragTarget.localScale;
		if(dragChangeScale!=0f){
			dragTarget.DOScale(m_cacheScale*dragChangeScale,0.25f);
		}

		dragTarget.position += new Vector3(0,0,dragOffsetZ);
		m_worldPos = dragTarget.position;
		Vector3 touchDownMousePos;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTarget,eventData.position,rayCastCamera,out touchDownMousePos);
		m_touchDownTargetOffset = m_worldPos-touchDownMousePos;

		m_isDown = true ;
		m_parent = dragTarget.parent;
		if(!string.IsNullOrEmpty(dragingParent)){
			GameObject go = GameObject.Find(dragingParent);
			if(go){
				dragTarget.SetParent(go.transform);
			}
		}

		if(OnMouseDownAction!=null){
			OnMouseDownAction(this);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (eventData.dragging)
		{
			RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTarget,eventData.position,rayCastCamera,out m_worldPos);
			if(!isDragOriginPoint){
				m_worldPos+=m_touchDownTargetOffset;
			}
			m_worldPos += (Vector3)dragOffset;
			if(sendHoverEvent && !string.IsNullOrEmpty(onHoverMethodName)){
				Collider2D[] cols = Physics2D.OverlapPointAll(triggerPos.position,rayCastMask,-100f,100f);
				if(cols.Length>0){
					foreach(Collider2D col in cols){
						if(col.gameObject!=gameObject)
							col.SendMessage(onHoverMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
					}
					gameObject.SendMessage(onHoverMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		if(OnMouseDragAction!=null){
			OnMouseDragAction(this);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if(dragChangeScale!=0f){
			dragTarget.DOScale(m_cacheScale,0.25f);
		}

		m_isDown = false ;
		if(!string.IsNullOrEmpty(onDropMethodName)){
			Collider2D[] cols = Physics2D.OverlapPointAll(triggerPos.position,rayCastMask,-100f,100f);
			if(cols.Length>0){
				foreach(Collider2D col in cols){
					if(col.gameObject!=gameObject)
						col.SendMessage(onDropMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
				}
				gameObject.SendMessage(onDropMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
			}
		}
		if(releaseAutoBack){
			BackPosition();
		}else{
			dragTarget.position -= new Vector3(0,0,dragOffsetZ);
			dragTarget.SetParent(m_parent);
		}

		if(OnMouseUpAction!=null){
			OnMouseUpAction(this);
		}
	}

	void Update(){
		if(m_isDown){
			dragTarget.position = Vector3.Lerp(dragTarget.position,m_worldPos,dragMoveDamp);
		}
	}

	/// <summary>
	/// 返回到最初位置
	/// </summary>
	public void BackPosition(){
		switch(backEffect)
		{
		case DragBackEffect.Immediately:
			dragTarget.SetParent(m_parent);
			dragTarget.position=m_cachePosition;
			break;
		case DragBackEffect.Destroy:
			Destroy(dragTarget.gameObject);
			break;
		case DragBackEffect.TweenPosition:
			this.enabled = false;
			dragTarget.DOMove(m_cachePosition,backDuring).SetEase(tweenEase).OnComplete(()=>{
				dragTarget.SetParent(m_parent);
				this.enabled = true;
				if(OnTweenBackAction!=null){
					OnTweenBackAction(this);
				}
			});
			break;
		case DragBackEffect.TweenScale:
			this.enabled = false;
			dragTarget.SetParent(m_parent);
			dragTarget.position=m_cachePosition;
			dragTarget.localScale = Vector3.zero;
			dragTarget.DOScale(m_cacheScale,backDuring).SetEase(tweenEase).OnComplete(()=>{
				this.enabled = true;
				if(OnTweenBackAction!=null){
					OnTweenBackAction(this);
				}
			});
			break;
		case DragBackEffect.ScaleDestroy:
			this.enabled = false;
			dragTarget.DOScale(Vector3.zero,backDuring).SetEase(tweenEase).OnComplete(()=>{
				Destroy(dragTarget.gameObject);
				if(OnTweenBackAction!=null){
					OnTweenBackAction(this);
				}
			});
			break;
		case DragBackEffect.FadeOutDestroy:
			this.enabled = false;
			CanvasGroup group = dragTarget.gameObject.AddComponent<CanvasGroup>();
			group.DOFade(0f,backDuring).SetEase(tweenEase).OnComplete(()=>{
				Destroy(dragTarget.gameObject);
				if(OnTweenBackAction!=null){
					OnTweenBackAction(this);
				}
			});
			break;
		}
	}

}
