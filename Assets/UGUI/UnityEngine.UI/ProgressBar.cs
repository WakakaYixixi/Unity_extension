using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
	[ExecuteInEditMode]
	public class ProgressBar : MonoBehaviour {

		public Image bar;

		public bool horizontalOrVertical = true;

		[Range(0f,1f)]
		[SerializeField]
		private float m_progress  = 1f;

		public float progress{
			get{ return m_progress; }
			set {
				if(m_progress!=value){
					m_progress = value;
					m_progress = Mathf.Clamp01(m_progress);
					UpdateBar();
				}
			}
		}

		// Use this for initialization
		void Start () {

		}

		#if UNITY_EDITOR
		void LateUpdate () {
			if(!Application.isPlaying){
				UpdateBar();
			}
		}
		#endif

		void UpdateBar(){
			if(bar)
			{
				Vector2 anchorPos = bar.rectTransform.anchoredPosition;
				if(horizontalOrVertical){
					anchorPos.x = -bar.rectTransform.sizeDelta.x*(1f-m_progress);
				}else{
					anchorPos.y = -bar.rectTransform.sizeDelta.y*(1f-m_progress);
				}
				bar.rectTransform.anchoredPosition = anchorPos;
			}
		}
	}

}