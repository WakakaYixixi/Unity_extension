using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RenderTexturePainter))]
public class PaintCompleteChecker : MonoBehaviour {
	//网格初始值
	public bool gridDefaultStatus=false;

	//笔刷大小
	[Range(0.1f,1f)]
	public float brushSize = 0.2f;

	public Color enableColor = Color.blue;
	public Color disableColor = Color.yellow;

	public Dictionary<string,Rect> gridsDic;
	public Dictionary<string,bool> enablesDic;

	[Header("Asset File")]
	//拖文件到这上面
	public PaintRectDictionary assetRectDic;
	public PaintEnableDictionary assetEnableDic;

	void Start(){
		
	}

	void OnDrawGizmos(){
		if(gridsDic!=null && enablesDic!=null){
			
			Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
			Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
			Gizmos.matrix *= cubeTransform;

			foreach(string key in gridsDic.Keys)
			{
				Rect rect = gridsDic[key];
				if(enablesDic[key]){
					Gizmos.color = enableColor;
				}
				else{
					Gizmos.color = disableColor;
				}
				Vector3 center = new Vector3(rect.x+rect.width*0.5f,rect.y+rect.height*0.5f);
				Vector3 size = new Vector3(rect.width,rect.height,0.1f);

				Gizmos.DrawWireCube(center,size);
			}

			Gizmos.matrix = oldGizmosMatrix;
		}
	}
}
