using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

/// <summary>
/// 用于拖动UGUI控件
/// </summary>
public class UGUIDrag: MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler,IPointerDownHandler,IPointerUpHandler{
	
	public enum DragBackEffect{
		None,Immediately, TweenPosition, TweenScale , ScaleDestroy , FadeOutDestroy , Destroy
	}
	public enum TriggerType{
		Point,Circle,Range
	}

	private Vector3 m_cachePosition;
	private Vector3 m_cacheScale;
	private Vector3 m_cacheRotation;

	private Vector3 m_defaultPosition; //局部坐标
	private Vector3 m_defaultScale;
	private Vector3 m_defaultRotation;

	private float m_dragMoveDamp;
	private Vector3 m_worldPos;
	private Vector3 m_touchDownTargetOffset ;
	private Transform m_parent;
	private bool m_isDown = false;
	private bool m_isDragging = false;
	public bool isDragging{
		get { return m_isDragging && m_isDown; }
	}
	private bool m_canDrag = true;
	public bool canDrag{
		get { return m_canDrag; }
		set { 
			m_canDrag = value; 
			if(!value) {
				m_isDragging = false;
				m_isDown = false;
			}

		}
	}
	public Vector3 dragTargetWorldPos{
		get { return m_worldPos; }
		set { m_worldPos = value; }
	}

	[Tooltip("拖动的对象，默认为自己.")]
	public RectTransform dragTarget;

	[Tooltip("如果为null，则使用mainCamera.")]
	public Camera rayCastCamera = null;

	[Tooltip("射线检测的Layer")]
	public LayerMask rayCastMask;

	[Header("Drag Setting")]
	[Tooltip("在拖动时是否固定在拖动物的原点.")]
	public bool isDragOriginPoint = false;

	[Tooltip("当isDragOriginPoint为true时，拖动时的偏移值.单位像素")]
	public Vector2 dragOffset;

	[Tooltip("主要用于影响层级显示.单位米")]
	public float dragOffsetZ=0f;

	[Tooltip("Drag时的变化的大小.")]
	public float dragChangeScale = 1f;

	[Tooltip("Drag时角度的变化值")]
	public float dragChangeRotate = 0f;

	[Tooltip("拖动时的所在的父窗器，用于拖动时在UI最上层，如果不填，则在当前层.")]
	public string dragingParent = "Canvas";

	[Tooltip("当按下时就开始执行拖动.")]
	public bool dragOnPointDown = true;

	[Tooltip("触发的原点，默认为当前对象")]
	public Transform triggerPos ;

	[Tooltip("触发的类型")]
	public TriggerType triggerType=TriggerType.Point;

	[Tooltip("当触发类型为圆时,设置半径")]
	public float triggerRadius=1f;

	[Tooltip("当触发类型为范围时,设置宽高")]
	public Vector2 triggerRange = Vector2.one;

	//要发送的事件名字
	[Header("Event")]
	public bool sendHoverEvent = false;
	public string onHoverMethodName = "OnHover";
	public string onHoverOutMethodName = "OnHoverOut";
	public string onDropMethodName = "OnDrop";

	[Header("Back Effect")]
	[Tooltip("释放时，是否自动返回")]
	public bool releaseAutoBack = false;
	[Tooltip("返回时的效果")]
	public DragBackEffect backEffect = DragBackEffect.None;
	[Tooltip("效果时间")]
	public float backDuring = 0.5f;
	[Tooltip("Tween 的效果")]
	public Ease tweenEase = Ease.Linear;

	public event Action<UGUIDrag,PointerEventData> OnPrevBeginDragAction = null ;
	public event Action<UGUIDrag,PointerEventData> OnBeginDragAction = null ;
	public event Action<UGUIDrag,PointerEventData> OnDragAction = null ;
	public event Action<UGUIDrag> OnDragTargetMoveAction = null ;
	public event Action<UGUIDrag,PointerEventData> OnEndDragAction = null ;
	public event Action<UGUIDrag> OnTweenOverAction = null ;
	public delegate bool DragValidCheck(PointerEventData eventData);
	public event DragValidCheck DragValidCheckEvent;

	void OnEnable(){
		m_isDown = false;
		m_isDragging = false;
	}

	void OnDisable(){
		m_isDown = false;
		m_isDragging = false;
	}

	// Use this for initialization
	void Start () {
		if(!dragTarget){
			dragTarget =  transform as RectTransform;;
		}

		m_defaultScale = dragTarget.localScale;
		m_defaultRotation = dragTarget.localEulerAngles;
		m_defaultPosition = dragTarget.localPosition;

		if(!triggerPos){
			triggerPos = dragTarget;
		}
		if (!rayCastCamera)
		{
			rayCastCamera = Camera.main;
		}
	}

	public void SetDefaultPosition(){
		if(dragTarget) dragTarget.localPosition = m_defaultPosition;
	}
	public void SetDefaultRotation(){
		if(dragTarget) dragTarget.localEulerAngles = m_defaultRotation;
	}
	public void SetDefaultScale(){
		if(dragTarget) dragTarget.localScale = m_defaultScale;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if(dragOnPointDown){
			OnBeginDrag (eventData);
			eventData.dragging = true;
			OnDrag (eventData);
		}
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		OnEndDrag (eventData);
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		if(!this.enabled || m_isDown) return;

		if(DragValidCheckEvent!=null) {
			if(!DragValidCheckEvent(eventData)){
				m_canDrag = false;
				return;
			}
		}
		if(OnPrevBeginDragAction!=null){
			OnPrevBeginDragAction(this,eventData);
		}

		m_dragMoveDamp = 0.3f;
		m_isDown = true;
		m_canDrag = true;
		dragTarget.DOKill();

		this.m_isDragging = true;
		this.GetComponent<Graphic>().raycastTarget = false;
		m_cachePosition = dragTarget.localPosition;
		m_cacheScale = dragTarget.localScale;
		m_cacheRotation = dragTarget.localEulerAngles;
		if(dragChangeScale!=0f){
			dragTarget.DOScale(m_cacheScale*dragChangeScale,0.4f);
		}
		if(dragChangeRotate!=0f){
			dragTarget.DOLocalRotate(m_cacheRotation +new Vector3(0f,0f,dragChangeRotate),0.4f,RotateMode.Fast);
		}

		dragTarget.position += new Vector3(0,0,dragOffsetZ);
		m_worldPos = dragTarget.position;
		Vector3 touchDownMousePos;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTarget,eventData.position,rayCastCamera,out touchDownMousePos);
		m_touchDownTargetOffset = m_worldPos-touchDownMousePos;
		if(!isDragOriginPoint){
			m_worldPos+=m_touchDownTargetOffset;
		}
		m_worldPos += (Vector3)dragOffset*0.01f;

		m_parent = dragTarget.parent;
		if(!string.IsNullOrEmpty(dragingParent)){
			GameObject go = GameObject.Find(dragingParent);
			if(go){
				dragTarget.SetParent(go.transform);
			}
		}

		if(OnBeginDragAction!=null){
			OnBeginDragAction(this,eventData);
		}
	}

	void OnApplicationFocus(bool flag){
		if(!flag && m_canDrag && m_isDragging){
			OnEndDrag(null);
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		if(!this.enabled || !this.m_canDrag || !m_isDown) return;
		m_isDragging = false;
		m_isDown = false;

		DOTween.Kill(dragTarget);
		if(dragChangeScale!=0f){
			dragTarget.DOScale(m_cacheScale,0.25f);
		}
		if(dragChangeRotate!=0f){
			dragTarget.DOLocalRotate(m_cacheRotation,0.25f,RotateMode.Fast);
		}

		if(!string.IsNullOrEmpty(onDropMethodName)){
			Collider2D[] cols = null;
			if(triggerType== TriggerType.Point){
				cols = Physics2D.OverlapPointAll(triggerPos.position,rayCastMask,-100f,100f);
			}else if(triggerType== TriggerType.Circle){
				cols = Physics2D.OverlapCircleAll(triggerPos.position,triggerRadius,rayCastMask,-100f,100f);
			}else if(triggerType== TriggerType.Range){
				Vector2 pa = new Vector2(-triggerRange.x,triggerRange.y);
				Vector2 pb = new Vector2(triggerRange.x,-triggerRange.y);
				Matrix4x4 mat = Matrix4x4.TRS(triggerPos.position,triggerPos.rotation,Vector3.one);
				pa = mat.MultiplyPoint(pa);
				pb = mat.MultiplyPoint(pb);
				cols = Physics2D.OverlapAreaAll(pa,pb,rayCastMask,-100f,100f);
			}
			if(cols!=null && cols.Length>0){
				foreach(Collider2D col in cols){
					if(col.gameObject!=gameObject)
						col.SendMessage(onDropMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
				}
				gameObject.SendMessage(onDropMethodName, cols , SendMessageOptions.DontRequireReceiver);
			}
		}
		if(releaseAutoBack){
			BackPosition();
		}else{
			dragTarget.position -= new Vector3(0,0,dragOffsetZ);
			dragTarget.SetParent(m_parent);
			this.m_canDrag = true;
		}

		if(OnEndDragAction!=null){
			OnEndDragAction(this,eventData);
		}
		this.GetComponent<Graphic>().raycastTarget = true;
	}


	public void OnDrag(PointerEventData eventData)
	{
		if(!this.enabled  || !m_canDrag || !m_isDown)  return;

		if(!string.IsNullOrEmpty(dragingParent) && !dragTarget.parent.name.Equals(dragingParent)){
			GameObject go = GameObject.Find(dragingParent);
			if(go){
				dragTarget.SetParent(go.transform);
			}
		}

		if (eventData.dragging)
		{
			m_isDragging = true;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(dragTarget,eventData.position,rayCastCamera,out m_worldPos);
			if(!isDragOriginPoint){
				m_worldPos+=m_touchDownTargetOffset;
			}
			m_worldPos += (Vector3)dragOffset*0.01f;
			if(sendHoverEvent){
				Collider2D[] cols = null;
				if(triggerType== TriggerType.Point){
					cols = Physics2D.OverlapPointAll(triggerPos.position,rayCastMask,-100f,100f);
				}else if(triggerType== TriggerType.Circle){
					cols = Physics2D.OverlapCircleAll(triggerPos.position,triggerRadius,rayCastMask,-100f,100f);
				}else if(triggerType== TriggerType.Range){
					Vector2 pa = new Vector2(-triggerRange.x,triggerRange.y);
					Vector2 pb = new Vector2(triggerRange.x,-triggerRange.y);
					Matrix4x4 mat = Matrix4x4.TRS(triggerPos.position,triggerPos.rotation,Vector3.one);
					pa = mat.MultiplyPoint(pa);
					pb = mat.MultiplyPoint(pb);
					cols = Physics2D.OverlapAreaAll(pa,pb,rayCastMask,-100f,100f);
				}
				if(cols!=null && cols.Length>0){
					foreach(Collider2D col in cols){
						if(col.gameObject!=gameObject)
							col.SendMessage(onHoverMethodName, dragTarget.gameObject , SendMessageOptions.DontRequireReceiver);
					}
					gameObject.SendMessage(onHoverMethodName, cols , SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					gameObject.SendMessage(onHoverOutMethodName,SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		if(OnDragAction!=null){
			OnDragAction(this,eventData);
		}
	}


	void Update(){
		if(!this.enabled) return;
		if(m_canDrag && m_isDragging){
			if(m_dragMoveDamp<1f) m_dragMoveDamp+=0.01f;
			dragTarget.position = Vector3.Lerp(dragTarget.position,m_worldPos,m_dragMoveDamp);
			if(Vector2.Distance((Vector2)dragTarget.position,(Vector2)m_worldPos)>0.001f){
				if(OnDragTargetMoveAction!=null){
					OnDragTargetMoveAction(this);
				}
			}
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
			dragTarget.localPosition=m_cachePosition;
			dragTarget.localScale = m_cacheScale;
			dragTarget.localEulerAngles = m_cacheRotation;
			this.m_canDrag = true;
			break;
		case DragBackEffect.Destroy:
			Destroy(dragTarget.gameObject);
			break;
		case DragBackEffect.TweenPosition:
			this.enabled = false;
			this.m_canDrag = false;
			dragTarget.SetParent(m_parent);
			dragTarget.DOLocalRotate(m_cacheRotation,backDuring).SetEase(tweenEase);
			dragTarget.DOScale(m_cacheScale,backDuring).SetEase(tweenEase);
			dragTarget.DOLocalMove(m_cachePosition,backDuring).SetEase(tweenEase).OnComplete(()=>{
				this.enabled = true;
				this.m_canDrag = true;
				if(OnTweenOverAction!=null){
					OnTweenOverAction(this);
				}
			});
			break;
		case DragBackEffect.TweenScale:
			this.enabled = false;
			this.m_canDrag = false;
			dragTarget.SetParent(m_parent);
			dragTarget.localPosition=m_cachePosition;
			dragTarget.localScale = Vector3.zero;
			dragTarget.localEulerAngles = m_cacheRotation;
			dragTarget.DOScale(m_cacheScale,backDuring).SetEase(tweenEase).OnComplete(()=>{
				this.enabled = true;
				this.m_canDrag = true;
				if(OnTweenOverAction!=null){
					OnTweenOverAction(this);
				}
			});
			break;
		case DragBackEffect.ScaleDestroy:
			this.enabled = false;
			this.m_canDrag = false;
			dragTarget.DOScale(Vector3.zero,backDuring).SetEase(tweenEase).OnComplete(()=>{
				Destroy(dragTarget.gameObject);
				if(OnTweenOverAction!=null){
					OnTweenOverAction(this);
				}
			});
			break;
		case DragBackEffect.FadeOutDestroy:
			this.enabled = false;
			this.m_canDrag = false;
			CanvasGroup group = dragTarget.gameObject.AddComponent<CanvasGroup>();
			group.blocksRaycasts = false;
			group.DOFade(0f,backDuring).SetEase(tweenEase).OnComplete(()=>{
				Destroy(dragTarget.gameObject);
				if(OnTweenOverAction!=null){
					OnTweenOverAction(this);
				}
			});
			break;
		default:
			dragTarget.SetParent(m_parent);
			dragTarget.localPosition=m_cachePosition;
			dragTarget.localScale = m_cacheScale;
			dragTarget.localEulerAngles = m_cacheRotation;
			this.m_canDrag = true;
			break;
		}
	}

	#if UNITY_EDITOR
	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Transform origin = triggerPos == null ? transform : triggerPos;
		if(triggerType == TriggerType.Point)
		{
			Gizmos.DrawSphere(origin.position,0.2f);
		}
		else if(triggerType == TriggerType.Circle)
		{
			Gizmos.DrawWireSphere(origin.position,triggerRadius);
		}
		else if(triggerType== TriggerType.Range)
		{
			Matrix4x4 mat = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(origin.position,origin.rotation,Vector3.one);
			Gizmos.DrawWireCube(Vector3.zero,(Vector3)triggerRange*2f);
			Gizmos.matrix = mat;
		}
	}
	#endif 
}
