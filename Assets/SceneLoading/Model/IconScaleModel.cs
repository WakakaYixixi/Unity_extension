using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 内置的一种模式：显示Icon模式，Icon缩放显示.
/// </summary>
[RequireComponent(typeof(SceneLoading))]
[RequireComponent(typeof(GUIBaseEffect))]
public class IconScaleModel: MonoBehaviour {

	private SceneLoading m_loading;
	private bool m_isDrawIcon = false;
	private float m_iconScale=0f;
	private bool m_startFadeout = false;

	public Texture icon;
	public bool isAutoFadeOut = true;//防止 fadeout两次
	public Action onLoadComplete ;

	// Use this for initialization
	void Start () {
		m_loading = GetComponent<SceneLoading> ();
		m_loading.OnLoadStay = OnLoadStay;
		m_loading.OnLoadProgress = OnLoadProgress;
		m_loading.isAutoLoadScene = false;
		LoadScene (1); //用的时候去掉这行
	}

	public void LoadScene(string name ){
		
		m_loading.StartFadeIn (name);
	}

	public void LoadScene( int lv){
		m_loading.StartFadeIn (lv);
	}


	void OnLoadStay(){
		m_isDrawIcon = true;
		StartCoroutine (ShowIcon());
	}

	IEnumerator ShowIcon(){
		yield return new WaitForEndOfFrame ();

		do {
			m_iconScale = Mathf.Lerp (m_iconScale, 1f, 0.4f);
			yield return null;

		} while(m_iconScale<0.99f);
		m_iconScale = 1f;
		m_loading.StartLoadScene ();
	}

	void OnLoadProgress( float pro){
		if (pro == 1f) {
			//complete
			if(isAutoFadeOut){
				StartCoroutine ( HideIcon() );
				m_startFadeout = true;
			}
			if(onLoadComplete!=null){
				onLoadComplete();
			}
		}
	}

	public void StartFadeOut()
	{
		UnityEngine.UI.Image img = gameObject.GetComponent<UnityEngine.UI.Image>();
		if(img){
			Destroy(img);
		}
		if(!m_startFadeout)
			StartCoroutine ( HideIcon() );
	}

	IEnumerator HideIcon(){
		yield return new WaitForSeconds (0.5f);
		
		do {
			m_iconScale = Mathf.Lerp (m_iconScale, 0f, 0.4f);
			yield return null;
			
		} while(m_iconScale>0.01f);

		m_isDrawIcon = false;

		UnityEngine.UI.Image img = gameObject.GetComponent<UnityEngine.UI.Image>();
		if(img){
			Destroy(img);
		}
		m_loading.StartFadeOut ();
	}

	void OnGUI()
	{
		if (m_isDrawIcon && icon) {
			GUI.DrawTexture(new Rect(Screen.width/2-icon.width*m_iconScale/2,Screen.height/2-icon.height*m_iconScale/2,icon.width*m_iconScale,icon.height*m_iconScale),icon);
		}
	}
}
