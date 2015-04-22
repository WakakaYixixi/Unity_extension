using UnityEngine;
using System;

public class SceneLoading : MonoBehaviour {

	enum EffectState{ NONE,IN,STAY,OUT}

	private GUIBaseEffect m_effect;
	private AsyncOperation m_async;
	private EffectState m_effectState = EffectState.NONE;
	private string m_nextName;
	private int m_nextLv;

	[HideInInspector]
	public bool isAutoLoadScene = false;
	public Action OnLoadStay;
	public Action<float> OnLoadProgress;

	// Use this for initialization
	void Awake () {
		m_effect = GetComponent<GUIBaseEffect> ();
		DontDestroyOnLoad (gameObject);
		DontDestroyOnLoad (transform.parent.gameObject);
	}
	
	public void StartFadeIn(string sceneName){
		m_nextName = sceneName;
		m_effectState = EffectState.IN;
		if (m_effect) {
			m_effect.OnComplete = OnEffectComplete;
			m_effect.EffectIn();
		} else {
			OnEffectComplete();
		}
	}

	public void StartFadeIn(int lv){
		m_nextLv = lv;
		m_effectState = EffectState.IN;
		if (m_effect) {
			m_effect.OnComplete = OnEffectComplete;
			m_effect.EffectIn();
		} else {
			OnEffectComplete();
		}
	}

	public void StartLoadScene(){
		if(!string.IsNullOrEmpty(m_nextName)){
			m_async = Application.LoadLevelAsync (m_nextName);
		}else if(m_nextLv>-1){
			m_async = Application.LoadLevelAsync (m_nextLv);
		}
	}

	public void StartFadeOut(){
		if (m_effect) {
			m_effect.EffectOut();
		}else {
			OnEffectComplete();
		}
	}


	void Update()
	{
		if (m_effectState == EffectState.STAY) {
			if (m_async!=null) {
				if(m_async.isDone){
					m_effectState = EffectState.OUT;
					Resources.UnloadUnusedAssets();
					if(OnLoadProgress!=null)
					{
						OnLoadProgress(1f);
					}
				}
				else if(OnLoadProgress!=null)
				{
					OnLoadProgress(m_async.progress);
				}
			}
		}
	}

	void OnEffectComplete(){
		if (m_effectState == EffectState.IN) {
			m_effectState = EffectState.STAY;
			if (OnLoadStay != null) {
				OnLoadStay ();
			}
			if (isAutoLoadScene) {
				StartLoadScene ();
			}
		} else if (m_effectState == EffectState.OUT) {
			Destroy (transform.parent.gameObject);
//			Destroy(gameObject);
		}
	}
}
