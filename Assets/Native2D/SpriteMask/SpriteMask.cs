using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpriteMask : MonoBehaviour {

	public Rect maskSize = new Rect(0,0,1,1);
	public Material[] maskMaterials;

	// Use this for initialization
	void Start () {
		Clip();
	}
	
	void Update () {
		Clip();
	}

	void Clip(){
		if(maskMaterials!=null){
			Vector4 rect  = new Vector4(maskSize.x,maskSize.y,maskSize.width,maskSize.height);
			rect.x+=transform.position.x;
			rect.y+=transform.position.y;
			rect.z+=transform.position.x+maskSize.x;
			rect.w+=transform.position.y+maskSize.y;
			for(int i=0;i<maskMaterials.Length;++i){
				maskMaterials[i].SetVector("_ClipRect",rect);
			}
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;

		Vector3 pos = new Vector3(maskSize.x+transform.position.x,maskSize.y+transform.position.y,transform.position.z);
		Vector3 pos1 = pos+new Vector3(maskSize.width,0,0);
		Gizmos.DrawLine( pos,pos1);
		pos = pos1+new Vector3(0,maskSize.height,0);
		Gizmos.DrawLine( pos,pos1);
		pos1 = pos-new Vector3(maskSize.width,0,0);
		Gizmos.DrawLine( pos,pos1);
		pos=new Vector3(maskSize.x+transform.position.x,maskSize.y+transform.position.y,transform.position.z);
		Gizmos.DrawLine( pos,pos1);
	}
}
