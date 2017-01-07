using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sprite mask.
/// Note: The mask's global position must be zero If mask is rotated.
/// </summary>
[ExecuteInEditMode]
public class SpriteMask : MonoBehaviour {

	public Rect maskSize = new Rect(0,0,1,1);
	public Material[] maskMaterials;

	//如果是Texture 遮罩，小心贴图边界的处理,图片的边界最好有1-2像素是透明的,mask图片不要设置mipmap
	public bool isTextureMask = false;
	private Vector4 m_rect;

	// Use this for initialization
	void Start () {
		Clip();
	}

	void LateUpdate () {
		Clip();
	}

	void Clip(){
		if(maskMaterials!=null){
			if(isTextureMask){
				m_rect.x = maskSize.x*transform.lossyScale.x+transform.position.x;
				m_rect.y = maskSize.y*transform.lossyScale.y+transform.position.y;
				m_rect.z = maskSize.width*transform.lossyScale.x;
				m_rect.w = maskSize.height*transform.lossyScale.y;
			}
			else{
				Vector3 pos = transform.position;
				Matrix4x4 matrix = Matrix4x4.TRS(pos ,Quaternion.identity, transform.lossyScale);
				pos = new Vector3(maskSize.x,maskSize.y,0f);
				pos = matrix.MultiplyPoint3x4(pos);
				m_rect.x = pos.x;
				m_rect.y = pos.y;
				pos = new Vector3(maskSize.x+maskSize.width,maskSize.y+maskSize.height,0f);
				pos = matrix.MultiplyPoint3x4(pos);
				m_rect.z = pos.x;
				m_rect.w = pos.y;
			}
			Matrix4x4 rotateMatrix = Matrix4x4.TRS(Vector3.zero,Quaternion.Euler(0,0,-transform.rotation.eulerAngles.z),Vector3.one);
			for(int i=0;i<maskMaterials.Length;++i){
				if(maskMaterials[i]){
					maskMaterials[i].SetMatrix("_RotateMatrix",rotateMatrix);
					maskMaterials[i].SetVector("_ClipRect",m_rect);
				}
			}
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		Vector3 pos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		Matrix4x4 cubeTransform = Matrix4x4.TRS(pos, transform.rotation, transform.lossyScale);
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
		Gizmos.matrix *= cubeTransform;
		Gizmos.DrawWireCube(new Vector3(maskSize.x,maskSize.y,0f)+new Vector3(maskSize.width*0.5f,maskSize.height*0.5f,0f),new Vector3(maskSize.width,maskSize.height,0.1f));
		Gizmos.matrix = oldGizmosMatrix;
	}
}
