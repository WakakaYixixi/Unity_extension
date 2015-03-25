using UnityEngine;
using System.Collections;

/// <summary>
/// 内置的一种模式：没有Icon显示的一种模式.
/// </summary>
[RequireComponent(typeof(SceneLoading))]
[RequireComponent(typeof(GUIBaseEffect))]
public class NoIconModel : MonoBehaviour {

	private SceneLoading m_loading;

	// Use this for initialization
	void Start () {
		m_loading = GetComponent<SceneLoading> ();
		m_loading.OnLoadProgress = OnLoadProgress;
		m_loading.isAutoLoadScene = true;
		LoadScene (1);
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
			m_loading.StartFadeOut();
		}
	}
}
