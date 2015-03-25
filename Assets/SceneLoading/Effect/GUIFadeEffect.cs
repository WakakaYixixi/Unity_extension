using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SceneLoading))]
public class GUIFadeEffect : GUIBaseEffect {
	
	public Color color = Color.white;
	public float speed = 2f;

	private float m_alpha = 0f;
	private Texture2D m_tex;

	void Awake(){
		m_tex = new Texture2D(4,4, TextureFormat.ARGB32, false);
		m_tex.wrapMode = TextureWrapMode.Clamp;
		for (int i = 0; i <= m_tex.width; i++)
		{
			for (int j = 0; j <= m_tex.height; j++)
			{
				m_tex.SetPixel(i, j, color);
			}
		}
		m_tex.Apply ();
	}

	public override void EffectIn()
	{
		StartCoroutine ("SetIn");
	}

	public override void EffectOut()
	{
		StartCoroutine ("SetOut");
	}

	IEnumerator SetIn(){
		yield return new WaitForEndOfFrame ();

		do{
			m_alpha = Mathf.Lerp(m_alpha,1f,speed*Time.deltaTime);
			yield return null;

		}while(m_alpha<0.99f);

		m_alpha = 1f;
		if (OnComplete != null) {
			OnComplete();
		}
	}

	IEnumerator SetOut(){
		yield return new WaitForSeconds (0.25f);

		do{
			m_alpha = Mathf.Lerp(m_alpha,0f,speed*Time.deltaTime);
			yield return null;
			
		}while(m_alpha>0.01f);
		
		m_alpha = 0f;
		if (OnComplete != null) {
			OnComplete();
		}
	}

	
	void OnGUI()
	{
		Color c = GUI.color;
		c.a = m_alpha;
		GUI.color = c;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), m_tex);
	}
}
