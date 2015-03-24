using UnityEngine;
using System.Collections;

/// <summary>
/// unity2D的锚点.
/// author:zhouzhanglin
/// </summary>
[ExecuteInEditMode]
public class Native2dAnchor : MonoBehaviour {
	
	public enum Position
	{
		CENTER,
		TOP,
		BOTTOM,
		LEFT,
		RIGHT,
		TOP_LEFT
	}
	
	public Position position=Position.CENTER;
	private int m_tick;
	
	void Start()
	{
		#if !UNITY_EDITOR
		SetPosition();
		#endif
	}
	
	#if UNITY_EDITOR
	void Update()
	{
		m_tick++;
		if (m_tick % 5 == 0)
		{
			SetPosition();
		}
	}
	#endif
	
	void SetPosition()
	{
		switch (position)
		{
		case Position.CENTER:
			transform.localPosition = Vector3.zero;
			break;
		case Position.TOP:
			transform.position = Native2dScreenUtil.GetScreenTopCenter();
			break;
		case Position.BOTTOM:
			transform.position = Native2dScreenUtil.GetScreenBottomCenter();
			break;
		case Position.LEFT:
			transform.position = Native2dScreenUtil.GetScreenMiddleLeft();
			break;
		case Position.RIGHT:
			transform.position = Native2dScreenUtil.GetScreenMiddleRight();
			break;
		}
	}
}
