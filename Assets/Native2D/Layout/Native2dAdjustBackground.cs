using UnityEngine;
using System.Collections;

/// <summary>
/// 背景铺满屏幕.自动缩放到合适的大小.
/// author:zhouzhanglin
/// </summary>
[ExecuteInEditMode]
public class Native2dAdjustBackground : MonoBehaviour
{
	private int m_tick = 0;
	
	// Use this for initialization
	void Start () {
		#if !UNITY_EDITOR
		SetScale();
		#endif
	}
	
	#if UNITY_EDITOR
	void Update()
	{
		m_tick++;
		if ( m_tick % 5 == 0)
		{
			SetScale();
		}
	}
	#endif
	
	void SetScale()
	{
		SpriteRenderer render2D = GetComponent<SpriteRenderer>();
		
		Vector3 size = render2D.bounds.size;
		size.x /= transform.localScale.x;
		size.y /= transform.localScale.y;
		
		float screenWidth = Native2dScreenUtil.GetScreenWidth();
		float screenHeight = Native2dScreenUtil.GetScreenHeight();
		Transform mtran = transform;
		
		if (size.x / size.y > screenWidth / screenHeight)
		{
			float scale = screenHeight / size.y;
			Vector3 localScale = mtran.localScale;
			localScale.x = scale;
			localScale.y = scale;
			mtran.localScale = localScale;
		}
		else if (size.x / size.y <= screenWidth / screenHeight)
		{
			float scale = screenWidth / size.x;
			Vector3 localScale = mtran.localScale;
			localScale.x = scale;
			localScale.y = scale;
			mtran.localScale = localScale;
		}
	}
}
