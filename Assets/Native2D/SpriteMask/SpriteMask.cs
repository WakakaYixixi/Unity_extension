﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpriteMask : MonoBehaviour {

	public Rect maskSize = new Rect(0,0,1,1);
	public Material[] maskMaterials;

	//如果是Texture 遮罩，小心贴图边界的处理,图片的边界最好有1-2像素是透明的,mask图片不要设置mipmap
	public bool isTextureMask = false;

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
			if(isTextureMask){
				rect.z+=maskSize.x;
				rect.w+=maskSize.y;
			}
			else{
				rect.z+=transform.position.x+maskSize.x;
				rect.w+=transform.position.y+maskSize.y;
			}
			for(int i=0;i<maskMaterials.Length;++i){
				if(maskMaterials[i])
					maskMaterials[i].SetVector("_ClipRect",rect);
			}
		}
	}

	void OnDrawGizmos(){
		if(!isTextureMask){
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
		}else{
			Gizmos.color = Color.green;

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
}
