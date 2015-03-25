using UnityEngine;
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

	public Texture icon;

	// Use this for initialization
	void Start () {
		m_loading = GetComponent<SceneLoading> ();
		m_loading.OnLoadStay = OnLoadStay;
		m_loading.OnLoadProgress = OnLoadProgress;
		m_loading.isAutoLoadScene = false;
		LoadScene (1);
	}

	public void LoadScene(string name ){
		
		m_loading.StartFadeIn (name);
	}

	public void LoadScene( int lv){
		m_loading.StartFadeIn (lv);
	}


	void OnLoadStay(){
		m_isDrawIcon = true;
		StartCoroutine ("ShowIcon");
	}

	IEnumerator ShowIcon(){
		yield return new WaitForEndOfFrame ();

		do {
			m_iconScale = Mathf.Lerp (m_iconScale, 1f, 0.4f);
			yield return null;

		} while(m_iconScale<0.99f);

		m_loading.StartLoadScene ();
	}

	void OnLoadProgress( float pro){
		if (pro == 1f) {
			//complete
			StartCoroutine ("HideIcon");
		}
	}

	IEnumerator HideIcon(){
		yield return new WaitForSeconds (0.5f);
		
		do {
			m_iconScale = Mathf.Lerp (m_iconScale, 0f, 0.4f);
			yield return null;
			
		} while(m_iconScale>0.01f);

		m_isDrawIcon = false;
		m_loading.StartFadeOut ();
	}

	void OnGUI()
	{
		if (m_isDrawIcon && icon) {
			GUI.DrawTexture(new Rect(Screen.width/2-icon.width*m_iconScale/2,Screen.height/2-icon.height*m_iconScale/2,icon.width*m_iconScale,icon.height*m_iconScale),icon);
		}
	}
}
