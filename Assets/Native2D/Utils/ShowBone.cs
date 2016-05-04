using UnityEngine;
using System.Collections;

/// <summary>
/// 显示骨骼
/// author:zhouzhanglin
/// </summary>
public class ShowBone : MonoBehaviour {
	#if UNITY_EDITOR
	public Color color = new Color(1f,1f,0f,0.5f);

	void OnDrawGizmos() {
		Gizmos.color = color;
		DrawArrow(transform);
		DrawSphere(transform);
	}

	void DrawArrow( Transform parent){
		for(int i=0;i<parent.childCount;++i){
			Transform child = parent.GetChild(i);

			Vector3 v = Quaternion.AngleAxis(45, Vector3.forward) * ((child.position - parent.position) / 5 );
			Gizmos.DrawLine(parent.position, parent.position + v);
			Gizmos.DrawLine(parent.position + v, child.position);

			v = Quaternion.AngleAxis(-45, Vector3.forward) * ((child.position - parent.position) / 5);
			Gizmos.DrawLine(parent.position, parent.position + v);
			Gizmos.DrawLine(parent.position + v, child.position);

			Gizmos.DrawLine(parent.position,child.position);
			DrawArrow(child);
		}
	}

	void DrawSphere( Transform parent){
		for(int i=0;i<parent.childCount;++i){
			Transform child = parent.GetChild(i);
			Gizmos.DrawSphere(parent.position,0.1f);
			DrawSphere(child);
		}
	}
	#endif
}
