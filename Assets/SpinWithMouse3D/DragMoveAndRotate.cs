using UnityEngine;
using System.Collections;

/// <summary>
/// 拖动并旋转物体，用力的方式.
/// author:zhouzhanglin
/// </summary>
public class DragMoveAndRotate : MonoBehaviour {

	private Vector3 m_pos;
	private Rigidbody m_rb;
	private Transform m_trans;

	[Tooltip("拖动时的缓冲，跟Rigidbody的Drag属性也有关系.")]
	public float dragDamp = 0.1f;

	[Tooltip("旋转的速度 ，跟Rigidbody的Angular Drag属性也有关系.")]
	public float rotateSpeed = 1f;

	// Use this for initialization
	void Start () {
		m_trans = transform;
		m_rb = GetComponent<Rigidbody> ();
	}

	void OnMouseDown(){
		m_pos = Input.mousePosition;
	}
	
	void OnMouseDrag(){
		Vector3 curr = Input.mousePosition;
		Vector3 delta = curr - m_pos;
		
		Vector3 screenPos = Camera.main.WorldToScreenPoint (m_trans.position);
		float torqueForce = screenPos.y < curr.y ? delta.x : -delta.x;
		m_rb.AddRelativeTorque(Vector3.up * Time.deltaTime * torqueForce*rotateSpeed);
		
		Vector3 dragForce = new Vector3 (Input.mousePosition.x-screenPos.x , 0f, Input.mousePosition.y - screenPos.y);
		m_rb.AddForce ( dragForce*dragDamp*Time.deltaTime, ForceMode.VelocityChange);
		
		m_pos = curr;
	}
}
