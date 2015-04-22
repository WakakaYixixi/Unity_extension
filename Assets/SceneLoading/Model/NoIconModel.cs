using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 内置的一种模式：没有Icon显示的一种模式.
/// </summary>
[RequireComponent(typeof(SceneLoading))]
[RequireComponent(typeof(GUIBaseEffect))]
public class NoIconModel : MonoBehaviour {

	private SceneLoading m_loading;
	private bool m_startFadeout = false; //防止 fadeout两次

	public bool isAutoFadeOut = true;
	public Action onLoadComplete ;

	// Use this for initialization
	void Start () {
		m_loading = GetComponent<SceneLoading> ();
		m_loading.OnLoadProgress = OnLoadProgress;
		m_loading.isAutoLoadScene = true;
		LoadScene (1); //用的时候去掉这行
	}
	
	public void LoadScene(string name ){
		
		m_loading.StartFadeIn (name);
	}
	
	public void LoadScene( int lv){
		m_loading.StartFadeIn (lv);
	}

	
	void OnLoadProgress( float pro){
		if (pro == 1f) {
			//complete
			if(isAutoFadeOut){
				UnityEngine.UI.Image img = gameObject.GetComponent<UnityEngine.UI.Image>();
				if(img){
					Destroy(img);
				}
				m_loading.StartFadeOut();
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
			m_loading.StartFadeOut();
	}
}
