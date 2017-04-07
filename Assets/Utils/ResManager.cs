using UnityEngine;
using System.Collections;

/// <summary>
/// 图片资源加载
/// </summary>
public class ResManager : MonoBehaviour {

	private static ResManager m_instance;
	public static ResManager Instance{
		get{
			if(m_instance==null){
				GameObject go = new GameObject();
				return go.AddComponent<ResManager>();
			}
			return m_instance;
		}
	}
	void Awake(){
		if(m_instance!=null){
			Destroy(gameObject);
			return;
		}
		name = "[ResManager]";
		m_instance = this;
		DontDestroyOnLoad(gameObject);
	}


}
