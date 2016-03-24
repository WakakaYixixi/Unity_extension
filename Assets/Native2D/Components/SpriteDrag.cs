using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

/// <summary>
/// Drag and drop sprite.
/// Author :zhouzhanglin
/// </summary>
public class SpriteDrag : MonoBehaviour {

	public enum DragBackEffect{
		None,Immediately, TweenPosition, TweenScale , ScaleDestroy , FadeOutDestroy , Destroy
	}

	private Vector3 m_cachePosition;
	private Vector3 m_cacheScale;
	private Vector3 m_dragOffset;
	private Vector3 m_screenPosition;
	private Vector3 m_currentPosition;
	private bool m_isDown;
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

	[Tooltip("Drag时的变化的大小.")]
	public float dragChangeScale = 1f;

	[Tooltip("拖动时的缓动参数.")]
	[Range(0f,1f)]
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

	[Header("Back Effect")]
	[Tooltip("释放时，是否自动返回")]
	public bool releaseAutoBack = false;
	[Tooltip("返回时的效果")]
	public DragBackEffect backEffect = DragBackEffect.None;
	[Tooltip("效果时间")]
	public float backDuring = 0.25f;
	[Tooltip("Tween 的效果")]
	public Ease tweenEase = Ease.Linear;


	public event Action<SpriteDrag> OnMouseDownAction = null ;
	public event Action<SpriteDrag> OnMouseDragAction = null ;
	public event Action<SpriteDrag> OnMouseUpAction = null ;

	// Use this for initialization
	void Start () {
		if (!dragTarget){
			dragTarget = transform;
		}
		if(!triggerPos){
			triggerPos = dragTarget;
		}
		if (!rayCastCamera)
		{
			rayCastCamera = Camera.main;
		}

		SpriteRenderer spriteRender = dragTarget.GetComponentInChildren<SpriteRenderer>();
		m_sortLayerName = spriteRender.sortingLayerName;
		if(string.IsNullOrEmpty(dragSortLayerName)){
			dragSortLayerName = m_sortLayerName;
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
		m_cachePosition = dragTarget.position;
		m_cacheScale = dragTarget.localScale;

		dragTarget.position+=new Vector3(0,0,dragOffsetZ);
		m_currentPosition = m_cachePosition;
		m_dragOffset = Vector3.zero;

		foreach(SpriteRenderer render in GetComponentsInChildren<SpriteRenderer>()){
			render.sortingLayerName=dragSortLayerName;
		}

		if(dragChangeScale!=0f){
			dragTarget.DOScale(m_cacheScale*dragChangeScale,0.25f);
		}

		m_screenPosition = rayCastCamera.WorldToScreenPoint(dragTarget.position);
		if (!isDragOriginPoint)
		{
			m_dragOffset = dragTarget.position - rayCastCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_screenPosition.z));
		}
		if (OnMouseDownAction!=null)
		{
			OnMouseDownAction(this);
		}
	}

	void OnMouseDragHandler(){
		m_screenPosition = rayCastCamera.WorldToScreenPoint(dragTarget.position);
		Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_screenPosition.z);
		m_currentPosition = rayCastCamera.ScreenToWorldPoint(curScreenSpace);
		if (!isDragOriginPoint){
			m_currentPosition += m_dragOffset;
		}else{
			m_currentPosition += (Vector3)dragOffset;
		}
		dragTarget.position = Vector3.Lerp(dragTarget.position, m_currentPosition, dragMoveDamp);
		if(sendHoverEvent&& !string.IsNullOrEmpty(onHoverMethodName)){
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
		if(dragChangeScale!=0f){
			dragTarget.DOScale(m_cacheScale,0.25f);
		}

		if(releaseAutoBack){
			BackPosition();
		}else{
			dragTarget.position -=new Vector3(0,0,dragOffsetZ);
			foreach(SpriteRenderer render in GetComponentsInChildren<SpriteRenderer>()){
				render.sortingLayerName=m_sortLayerName;
			}
		}

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

		if (OnMouseUpAction!=null)
		{
			OnMouseUpAction(this);
		}
	}

	/// <summary>
	/// 返回原来位置
	/// </summary>
	public void BackPosition(){
		switch(backEffect)
		{
		case DragBackEffect.Immediately:
			foreach(SpriteRenderer render in GetComponentsInChildren<SpriteRenderer>()){
				render.sortingLayerName=m_sortLayerName;
			}
			dragTarget.position=m_cachePosition;
			break;
		case DragBackEffect.Destroy:
			Destroy(dragTarget.gameObject);
			break;
		case DragBackEffect.TweenPosition:
			this.enabled = false;
			dragTarget.DOMove(m_cachePosition,backDuring).SetEase(tweenEase).OnComplete(()=>{
				foreach(SpriteRenderer render in GetComponentsInChildren<SpriteRenderer>()){
					render.sortingLayerName=m_sortLayerName;
				}
				this.enabled = true;
			});
			break;
		case DragBackEffect.TweenScale:
			this.enabled = false;
			foreach(SpriteRenderer render in GetComponentsInChildren<SpriteRenderer>()){
				render.sortingLayerName=m_sortLayerName;
			}
			dragTarget.position=m_cachePosition;
			dragTarget.localScale = Vector3.zero;
			dragTarget.DOScale(m_cacheScale,backDuring).SetEase(tweenEase).OnComplete(()=>{
				this.enabled = true;
			});
			break;
		case DragBackEffect.ScaleDestroy:
			this.enabled = false;
			dragTarget.DOScale(Vector3.zero,backDuring).SetEase(tweenEase).OnComplete(()=>{
				Destroy(dragTarget.gameObject);
			});
			break;
		case DragBackEffect.FadeOutDestroy:
			this.enabled = false;
			CanvasGroup group = dragTarget.gameObject.AddComponent<CanvasGroup>();
			group.DOFade(0f,backDuring).SetEase(tweenEase).OnComplete(()=>{
				Destroy(dragTarget.gameObject);
			});
			break;
		}
	}
}
