using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// 缩放图片，在手机上才能测试
/// </summary>
public class MultiScale: MonoBehaviour,IPointerUpHandler,IPointerDownHandler,IDragHandler {


	/// <summary>
	/// 自定义touch
	/// </summary>
	class MyTouch{
		public int fingerId = -1;
		public Vector2 localStartPos;//图片局部坐标
		public Vector2 screenPreviousPos;//屏幕坐标
	}


	//要缩放的对象
	public RectTransform target;

	//最大缩放值和最小缩放值
	public float minScale = 1f;
	public float maxScale = 10f;

	//当前的touch
	private List<MyTouch> touches = new List<MyTouch>();
	private RectTransform m_rectTrans;

	void Awake(){
		m_rectTrans = GetComponent<RectTransform>();
		if(target==null){
			target = m_rectTrans;
		}
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		foreach(Touch touch in Input.touches)
		{
			MyTouch mt = CheckTouch(touch);
			if(mt==null && touch.phase == TouchPhase.Began )
			{
				Vector2 localPoint;
				if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rectTrans,eventData.position,eventData.enterEventCamera,out localPoint))
				{
					//在图片上的位置
					localPoint += new Vector2(m_rectTrans.sizeDelta.x * m_rectTrans.pivot.x,m_rectTrans.sizeDelta.y * m_rectTrans.pivot.y);
					//添加一个MyTouch
					MyTouch mytouch = new MyTouch();
					mytouch.localStartPos = localPoint;
					mytouch.screenPreviousPos = touch.position;
					mytouch.fingerId = touch.fingerId;
					touches.Add(mytouch);
				}
			}
		}
	}


	public void OnPointerUp (PointerEventData eventData)
	{
		foreach(Touch touch in Input.touches)
		{
			if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled  )
			{
				RemoveTouch(touch);
			}
		}
	}

	/// <summary>
	/// 缩放时
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnDrag (PointerEventData eventData)
	{
		if(Input.touchCount==2)
		{
			Touch t1 = Input.touches[0];
			Touch t2 = Input.touches[1];
			MyTouch mt1 = CheckTouch(t1);
			MyTouch mt2 = CheckTouch(t2);
			if( mt1!=null && mt2!=null )
			{
				Vector2 middle = (mt1.localStartPos+mt2.localStartPos)/2f;
				float scale = (t2.position-t1.position).sqrMagnitude/(mt2.screenPreviousPos-mt1.screenPreviousPos).sqrMagnitude;
				ScaleMap(ref middle , scale );
				//缓存屏幕坐标点
				mt1.screenPreviousPos = t1.position;
				mt2.screenPreviousPos = t2.position;
			}
		}
	}



//	void Update()
//	{
//		if (Input.GetAxis("Mouse ScrollWheel") != 0)  
//		{  
//			float delta = 1+Input.GetAxis("Mouse ScrollWheel");
//			Vector2 localPoint;
//			if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_rectTrans,Input.mousePosition,Camera.main,out localPoint))
//			{
//				localPoint += new Vector2(m_rectTrans.sizeDelta.x * m_rectTrans.pivot.x,m_rectTrans.sizeDelta.y * m_rectTrans.pivot.y);
//				ScaleMap(ref localPoint,delta);
//			}
//		}  
//	}

	/// <summary>
	/// 缩放地图
	/// </summary>
	/// <param name="middle">中间点.</param>
	/// <param name="scale">缩放值.</param>
	private void ScaleMap( ref Vector2 middle,float scale)
	{
		Vector2 pivot = new Vector2(middle.x/m_rectTrans.sizeDelta.x,middle.y/m_rectTrans.sizeDelta.y);
		target.pivot = pivot;
		//缩放大小限制
		if(target.localScale.x*scale>minScale && target.localScale.x*scale<maxScale){
			target.localScale *= scale;
		}
	}

	/// <summary>
	/// 根据fingerid从数组中移除MyTouch
	/// </summary>
	/// <param name="t">T.</param>
	private void RemoveTouch( Touch t){
		List<MyTouch> removed = new List<MyTouch>();
		foreach(MyTouch touch in touches){
			if(touch.fingerId == t.fingerId){
				removed.Add(touch);
			}
		}
		foreach(MyTouch touch in removed){
			touches.Remove(touch);
		}
	}

	/// <summary>
	/// 判断数组中是否已经有此fingerId的Touch，如果有返回该MyTouch
	/// </summary>
	/// <returns>The touch.</returns>
	/// <param name="touch">Touch.</param>
	private MyTouch CheckTouch(Touch touch){
		foreach(MyTouch current in touches){
			if(current.fingerId==touch.fingerId) {
				return current;
			}
		}
		return null;
	}
}
