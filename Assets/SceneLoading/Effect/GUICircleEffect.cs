using UnityEngine;
using System;

[RequireComponent(typeof(SceneLoading))]
public class GUICircleEffect :GUIBaseEffect
{
	private Texture2D m_tex;
	private int m_radius;
	private bool m_isOver=true;
	private bool m_in = true;
	private int m_maxRadius;//用于out.
	private Color m_transparentColor;
	
	public Color color = Color.white;
	public int speed = 5;
	
	// Use this for initialization
	void Awake () {
		m_transparentColor = color;
		m_transparentColor.a = 0f;
		m_tex = new Texture2D(Screen.width / 2, Screen.height / 2, TextureFormat.ARGB32, false);
		m_tex.wrapMode = TextureWrapMode.Clamp;
	}
	
	/// <summary>
	/// 圆从大到小.
	/// </summary>
	public override void EffectIn()
	{
		m_in = true;
		for (int i = 0; i <= m_tex.width; i++)
		{
			for (int j = 0; j <= m_tex.height; j++)
			{
				m_tex.SetPixel(i, j, m_transparentColor);
			}
		}
		m_tex.Apply();
		m_radius = (int)Mathf.Sqrt(m_tex.width / 2 * m_tex.width / 2 + m_tex.height / 2 * m_tex.height / 2);
		m_isOver = false;
	}
	
	/// <summary>
	/// 圆从小到大.
	/// </summary>
	public override void EffectOut()
	{
		m_in = false;
		for (int i = 0; i <= m_tex.width; i++)
		{
			for (int j = 0; j <= m_tex.height; j++)
			{
				m_tex.SetPixel(i, j, color);
			}
		}
		m_tex.Apply();
		m_maxRadius = (int)Mathf.Sqrt(m_tex.width / 2 * m_tex.width / 2 + m_tex.height / 2 * m_tex.height / 2);
		m_radius = 0;
		m_isOver = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_isOver){
			if (m_in)
			{
				if (m_radius > 0)
				{
					SetIn();
				}
				else
				{
					m_radius = 0;
					m_isOver = true;
					SetIn();
					if (OnComplete != null)
					{
						OnComplete();
					}
				}
			}
			else
			{
				if (m_radius < m_maxRadius)
				{
					SetOut();
				}
				else
				{
					m_radius = m_maxRadius;
					m_isOver = true;
					SetOut();
					if (OnComplete != null)
					{
						OnComplete();
					}
				}
			}
		}
	}
	
	void SetIn()
	{
		Vector2 center = new Vector2(m_tex.width / 2, m_tex.height / 2);
		int pc = speed - 1;
		int pi = (int)center.x - m_radius - pc;
		int pj = (int)center.y - m_radius - pc;
		int mi = m_tex.width - pi + pc + 1;
		int mj = m_tex.height - pj + pc + 1;
		for (int i = pi; i <mi ; i++)
		{
			for (int j = pj; j < mj; j++)
			{
				float distance = Vector2.Distance(new Vector2(i, j), center);
				if (distance >= m_radius)
				{
					m_tex.SetPixel(i, j, color);
				}
			}
		}
		m_tex.Apply();
		m_radius -= speed;
	}
	
	void SetOut()
	{
		Vector2 center = new Vector2(m_tex.width / 2, m_tex.height / 2);
		int pc = speed - 1;
		int pi = (int)center.x - m_radius - pc;
		int pj = (int)center.y - m_radius - pc;
		int mi = m_tex.width - pi + pc ;
		int mj = m_tex.height - pj + pc;
		for (int i = pi; i <= mi; i++)
		{
			for (int j = pj; j <= mj; j++)
			{
				float distance = Vector2.Distance(new Vector2(i, j), center);
				if (distance < m_radius)
				{
					m_tex.SetPixel(i, j, m_transparentColor);
				}
			}
		}
		m_tex.Apply();
		m_radius += speed;
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(-5,-5, Screen.width+10, Screen.height+10), m_tex, ScaleMode.ScaleAndCrop);
	}
}
