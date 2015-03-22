using UnityEngine;
using System.Collections;

/// <summary>
/// 拖放3D物体，使用两种方式，一种用系统OnMouseDown,OnMouseDrag,OnMouseUp事件，另一种用射线检测。注意：使用的是主相机(mainCamera)
/// send Message:OnDragAndDropDown,OnDragAndDropMove,OnDragAndDropRelease
/// author:zhouzhanglin
/// 2015/3/12
/// </summary>
[RequireComponent(typeof(Collider))]
public class DragAndDrop3D : MonoBehaviour {

	public enum DragBackEffect
	{
		None,Immediately,TweenPosition,TweenScale
	}

	private Vector3 m_cachePosition ;
	private Vector3 m_cacheScale;
	private Vector3 m_dragOffset;
	private Vector3 m_screenPosition;
	private bool m_haveRigidbody;
	private bool m_defaultIsKinematic;
	private Transform m_trans;
	private Collider m_collider;
	private bool m_isDown;
	private Vector3 m_currentPosition ;

	[Tooltip("是否发送鼠标事件，OnDragAndDropDown,OnDragAndDropMove,OnDragAndDropRelease.")]
	public bool isSendMouseEvent = false;

	[Tooltip("是否使用射线检测.如果是，则设置rayCastMasks中的参数.")]
	public bool isUseRaycast = false ;

	[Tooltip("射线的检测距离，只用于射线检测时.")]
	public float raycastDistance = 100f;

	[Tooltip("射线检测的Layer")]
	public LayerMask[] rayCastMasks ;

	[Tooltip("在拖动时是否固定在拖动物的原点.")]
	public bool isDragOriginPoint = false ;

	[Tooltip("如果isDragOriginPoint为true,则可以设置拖动时的偏移值.")]
	public Vector3 dragOffset ;

	[Tooltip("拖动时的缓动参数.")]
	public float dragMoveDamp = 0.5f;

	[Tooltip("移动时在哪个面上移动，如果为null，则在拖动物的Z轴面移动.")]
	public GameObject mousePickLayer = null;

	[Tooltip("drop容器所在的层.")]
	public LayerMask[] dropLayerMasks ;

	[Tooltip("drop发生时发送的事件，drop和当前拖动对象都会发送.")]
	public string dropMedthod = "OnDrop";

	[Tooltip("如果没有检测到可drop的容器，是否返回原来的位置.")]
	public bool isDropFailBack = true;

	[Tooltip("返回原来位置时的效果.")]
	public DragBackEffect dragBackEffect = DragBackEffect.None;

	[Tooltip("返回原来位置时的速度.对TweenPosition和TweenScale有用.")]
	public float backEffectSpeed = 10f;


	#region MonoBehaviour内置方法.
	void Start () {
		m_trans = transform;
		m_collider = GetComponent<Collider> ();
		if (mousePickLayer) {
			mousePickLayer.SetActive(false);
		}
	}
	void Update () {
		if (isUseRaycast && Input.touchCount < 2) {
			if(Input.GetMouseButtonDown(0)){
				if(!m_isDown){
					int mask = GetLayerMask (rayCastMasks);
					RaycastHit hit;
					if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,raycastDistance,mask)){
						if(hit.collider.gameObject==gameObject){
							m_isDown=true;
							OnMouseDownHandler();
						}
					}
				}
			}
			else if(m_isDown && Input.GetMouseButtonUp(0)){
				m_isDown = false;
				OnMouseUpHandler();
			}
			else if(m_isDown){
				OnMouseDragHandler();
			}
		}
	}
	void OnMouseDown(){
		if (!isUseRaycast&&Input.touchCount < 2) {
			OnMouseDownHandler();
		}
	}
	void OnMouseDrag(){
		if (!isUseRaycast&&Input.touchCount < 2) {
			OnMouseDragHandler ();
		}
	}
	void OnMouseUp(){
		if (!isUseRaycast&&Input.touchCount < 2) {
			OnMouseUpHandler();
		}
	}
#endregion


	private void OnMouseDownHandler(){
		StopAllCoroutines ();
		m_cachePosition = m_trans.position;
		m_cacheScale = m_trans.localScale;
		m_currentPosition = m_cachePosition;
		m_dragOffset = Vector3.zero;

		Rigidbody rb = GetComponent<Rigidbody> ();
		if (rb) {
			m_haveRigidbody = true;
			m_defaultIsKinematic = rb.isKinematic;
			rb.isKinematic = true;
		} else {
			m_haveRigidbody = false;
		}
		m_screenPosition = Camera.main.WorldToScreenPoint(m_trans.position);
		if (!isDragOriginPoint) {
			m_dragOffset = m_trans.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, m_screenPosition.z));
		}
		if (isUseRaycast && mousePickLayer) {
			mousePickLayer.SetActive (true);
		}
		if (isSendMouseEvent) {
			gameObject.SendMessage("OnDragAndDropDown",SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnMouseDragHandler(){
		if (isUseRaycast && mousePickLayer) {
			RaycastHit hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,raycastDistance,1<<mousePickLayer.layer)){
				if(hit.collider.gameObject==mousePickLayer){
					m_currentPosition = hit.point;
				}
			}
		} else {
			Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_screenPosition.z);
			m_currentPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) ;
		}

		if (!isDragOriginPoint) {
			m_currentPosition += m_dragOffset;
		} else {
			m_currentPosition += dragOffset;
		}
		if (dragMoveDamp > 0f) {
			m_trans.position = Vector3.Lerp(m_trans.position,m_currentPosition,dragMoveDamp);
		} else {
			m_trans.position = m_currentPosition;
		}
		if (isSendMouseEvent) {
			gameObject.SendMessage("OnDragAndDropMove",SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnMouseUpHandler(){
		if (mousePickLayer) {
			mousePickLayer.SetActive (false);
		}
		if (m_haveRigidbody) {
			Rigidbody rb = GetComponent<Rigidbody> ();
			if (rb) {
				rb.isKinematic = m_defaultIsKinematic;
			}
		}
		//check drop
		int mask = GetLayerMask (dropLayerMasks);
		RaycastHit hit;
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,raycastDistance,mask)){
			if(hit.collider.gameObject!=gameObject){ //Exclude myself
				hit.collider.SendMessage(dropMedthod,SendMessageOptions.DontRequireReceiver);
				gameObject.SendMessage(dropMedthod,gameObject,SendMessageOptions.DontRequireReceiver);
			}else{
				BackPosition();
			}
		}
		else
		{
			BackPosition ();
		}
		if (isSendMouseEvent) {
			gameObject.SendMessage("OnDragAndDropRelease",SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary>
	/// 根据LayerMask数组来获取 射线检测的所有层.
	/// </summary>
	/// <returns>The layer mask.</returns>
	/// <param name="masks">Masks.</param>
	private int GetLayerMask(LayerMask[] masks){
		int mask = Physics.AllLayers ;
		if(masks!=null && masks.Length>0){
			mask = masks[0].value ;
			for(int i=1;i<masks.Length;i++){
				mask = mask|masks[i].value;
			}
		}
		return mask;
	}


	/// <summary>
	/// 返回到原来的位置.
	/// </summary>
	public void BackPosition(){
		if (isDropFailBack) {
			switch(dragBackEffect){
			case DragBackEffect.TweenPosition:
				StartCoroutine("BackTween");
				break;
			case DragBackEffect.Immediately:
				m_trans.position = m_cachePosition ;
				m_collider.enabled = true;
				break;
			case DragBackEffect.TweenScale:
				m_trans.position = m_cachePosition ;
				m_trans.localScale = Vector3.zero;
				StartCoroutine("ScaleTween");
				break;
			default :
				m_collider.enabled = true;
				break;
			}
		}
	}
	private IEnumerator BackTween(){
		m_collider.enabled = false;//Prevent dragging
		while (Vector3.Distance(m_trans.position,m_cachePosition)>0.01f) {
			m_trans.position = Vector3.Lerp(m_trans.position,m_cachePosition,backEffectSpeed*Time.fixedDeltaTime);
			yield return new WaitForFixedUpdate();
		}
		m_trans.position = m_cachePosition ;
		m_collider.enabled = true;
	}
	private IEnumerator ScaleTween(){
		m_collider.enabled = false;//Prevent dragging
		while (Vector3.Distance(m_trans.localScale,m_cacheScale)>0.01f) {
			m_trans.localScale = Vector3.Lerp(m_trans.localScale,m_cacheScale,backEffectSpeed*Time.fixedDeltaTime);
			yield return new WaitForFixedUpdate();
		}
		m_trans.localScale = m_cacheScale ;
		m_collider.enabled = true;
	}

}
