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

		//load group
		ResManager.Instance.maxCount = 3;
		ResManager.Instance.LoadGroup(new ResManager.Asset[]{
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites),
			new ResManager.Asset("sprites.png",ResManager.AssetType.Sprite),
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites),
			new ResManager.Asset("sprites.png",ResManager.AssetType.Sprite),
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites),
			new ResManager.Asset("sprites.png",ResManager.AssetType.Sprite),
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites),
			new ResManager.Asset("sprites.png",ResManager.AssetType.Sprite),
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites),
			new ResManager.Asset("sprites.png",ResManager.AssetType.Sprite),
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites),
			new ResManager.Asset("sprites.png",ResManager.AssetType.Sprite),
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites),
			new ResManager.Asset("sprites.png",ResManager.AssetType.Sprite),
			new ResManager.Asset("sprites.xml",ResManager.AssetType.Sprites)
		},delegate(ResManager.Asset[] assets) {
			print("Load group complete");
		},delegate(ResManager.Asset[] assets, float progress) {
			print("Load group :"+progress);
		});

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
