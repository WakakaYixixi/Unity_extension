using UnityEngine;
using System.Collections;

public class ResManagerTest : MonoBehaviour {
	
	SpriteRenderer sr = null;

	void Start () {
		sr = GetComponent<SpriteRenderer>();
		ResManager.Instance.LoadAsset(
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites)
			,delegate(ResManager.Asset asset) {
				sr.sprite  = asset.sprites["a_body.png"];
				StartCoroutine(ChangeImg());
			}
		);

	}

	IEnumerator ChangeImg(){
		yield return new WaitForSeconds(2f);
		//换图片
		sr.sprite = ResManager.Instance.GetAsset("sprites.xml").sprites["a_head.png"];

		yield return new WaitForSeconds(2f);
		//dispose
		ResManager.Instance.DisposeAsset("sprites.xml");
	}

}
