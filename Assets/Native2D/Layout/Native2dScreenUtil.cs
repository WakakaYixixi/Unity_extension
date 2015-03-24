using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// untiy3d Native2d 屏幕坐标工具类.
/// 这里面的Camer.main是Native2d中的相机.
/// author:zhouzhanglin
/// </summary>
public class Native2dScreenUtil
{
	
	/// <summary>
	/// Native2D中屏幕的宽.
	/// </summary>
	/// <returns>The screen width.</returns>
	public static float GetScreenWidth(){
		return -Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width / 2f, 0, 0)).x;
	}
	
	/// <summary>
	/// Native2D中屏幕的高.
	/// </summary>
	/// <returns>The screen height.</returns>
	public static float GetScreenHeight(){
		return -Camera.main.ScreenToWorldPoint(new Vector3(0, -Screen.height / 2f, 0)).y;
	}
	
	public static Vector2 GetScreenTopLeft(){
		return new Vector2( -GetScreenWidth()/2,GetScreenHeight()/2);
	}
	public static Vector2 GetScreenTopCenter(){
		return new Vector2( 0,GetScreenHeight()/2);
	}
	public static Vector2 GetScreenTopRight(){
		return new Vector2( GetScreenWidth()/2,GetScreenHeight()/2);
	}
	public static Vector2 GetScreenBottomLeft(){
		return new Vector2( -GetScreenWidth()/2, -GetScreenHeight()/2);
	}
	public static Vector2 GetScreenBottomCenter(){
		return new Vector2( 0,-GetScreenHeight()/2);
	}
	public static Vector2 GetScreenBottomRight(){
		return new Vector2( GetScreenWidth()/2,-GetScreenHeight()/2);
	}
	public static Vector2 GetScreenMiddleLeft(){
		return new Vector2( -GetScreenWidth()/2,0);
	}
	public static Vector2 GetScreenMiddleRight(){
		return new Vector2( GetScreenWidth()/2,0);
	}
}

