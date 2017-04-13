using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// 图片资源加载 (图集支持Starling/Sparrow格式)
/// author: zhouzhanglin
/*
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
 */
/// </summary>
public class ResManager : MonoBehaviour
{

	#if UNITY_EDITOR
	public static string streamingAssetPath = "file://" + Application.streamingAssetsPath + "/";
	#elif UNITY_ANDROID
	public static string streamingAssetPath = Application.streamingAssetsPath + "/";
	#else
	public static string streamingAssetPath = "file://" + Application.streamingAssetsPath + "/";
	#endif

	private static ResManager m_instance;

	public static ResManager Instance {
		get {
			if (m_instance == null) {
				GameObject go = new GameObject ();
				return go.AddComponent<ResManager> ();
			}
			return m_instance;
		}
	}

	void Awake ()
	{
		if (m_instance != null) {
			Destroy (gameObject);
			return;
		}
		name = "[ResManager]";
		m_instance = this;
		DontDestroyOnLoad (gameObject);
	}

	/// <summary>
	/// 返回的资源类型
	/// </summary>
	public enum AssetType
	{
		Sprite,
		Texture2D,
		Sprites
	}

	/// <summary>
	/// 资源的路径
	/// </summary>
	public enum AssetPath
	{
		StreamingAssets,
		Resources
	}

	[System.Serializable]
	public class Asset
	{
		public string url;
		public AssetType type;
		public AssetPath path;
		[HideInInspector]
		public Dictionary<string,Sprite> sprites;
		[HideInInspector]
		public Sprite sprite;
		[HideInInspector]
		public Texture2D texture;

		//for sprite
		public SpriteMeshType meshType = SpriteMeshType.FullRect;
		//for texture2d
		public bool textureReadonly = true;
		//for texture2d
		public TextureWrapMode warpMode = TextureWrapMode.Clamp;
		
		
		public bool cached = false;

		public Asset (){}

		public Asset (string url, AssetType type, AssetPath path=AssetPath.StreamingAssets, bool cached = false)
		{
			this.url = url;
			this.type = type;
			this.path = AssetPath.StreamingAssets;
			this.cached = cached;
		}
	}

	public int maxCount = int.MaxValue;
	public bool debug = true;
	private static Dictionary<string,Asset> loadedKV = new Dictionary<string, Asset> ();
	private int m_CurrentCount;

	/// <summary>
	/// Loads the group.
	/// </summary>
	/// <param name="assetGroup">Asset group.</param>
	/// <param name="onLoaded">On loaded.</param>
	/// <param name="onProgress">On progress.</param>
	public void LoadGroup(Asset[] assetGroup , System.Action<Asset[]> onLoaded , System.Action<Asset[],float> onProgress = null){
		int count = assetGroup.Length;
		for(int i=0;i<assetGroup.Length;++i){
			LoadAsset(assetGroup[i],delegate(Asset asset) {
				count--;

				if(onProgress!=null){
					onProgress(assetGroup,1f-(float)count/assetGroup.Length);
				}

				if(count==0){
					if(onLoaded!=null)
						onLoaded(assetGroup);
				}
			});
		}
	}

	/// <summary>
	/// Loads the asset.
	/// </summary>
	/// <param name="asset">Asset.</param>
	/// <param name="onLoaded">On loaded call back.</param>
	public void LoadAsset (Asset asset, System.Action<Asset> onLoaded)
	{
		if (loadedKV.ContainsKey (asset.url)) {
			if (onLoaded != null)
				onLoaded (loadedKV [asset.url]);
		} else {
			StartCoroutine (LoadingAsset (asset, onLoaded));
		}
	}

	/// <summary>
	/// Gets the asset.
	/// </summary>
	/// <returns>The asset.</returns>
	/// <param name="url">URL.</param>
	public Asset GetAsset (string url)
	{
		if (loadedKV.ContainsKey (url))
			return loadedKV [url];
		return null;
	}



	/// <summary>
	/// Disposes the asset.
	/// </summary>
	/// <param name="url">URL.</param>
	public void DisposeAsset (string url)
	{
		if (loadedKV.ContainsKey (url)) {
			DisposeAsset (loadedKV [url]);
		}
	}

	/// <summary>
	/// Disposes the asset.
	/// </summary>
	/// <param name="asset">Asset.</param>
	public void DisposeAsset (Asset asset)
	{
		if (asset.path == AssetPath.StreamingAssets) {
			
			if (asset.sprite)
				DestroyImmediate (asset.sprite, false);
			if (asset.sprites != null) {
				foreach (Sprite s in asset.sprites.Values) {
					DestroyImmediate (s, false);
				}
				asset.sprites = null;
			}
			if (asset.texture) {
				DestroyImmediate (asset.texture, false);
			}

		} else if (asset.path == AssetPath.Resources) {
			
			if (asset.sprite)
				Resources.UnloadAsset (asset.sprite);
			if (asset.texture)
				Resources.UnloadAsset (asset.texture);
			if (asset.sprites != null) {
				foreach (Sprite s in asset.sprites.Values) {
					DestroyImmediate (s, false);
				}
				asset.sprites = null;
			}
		}
		if (loadedKV.ContainsKey (asset.url)) {
			loadedKV.Remove (asset.url);
		}
	}

	/// <summary>
	/// Disposes all.
	/// </summary>
	/// <param name="containCache">If set to <c>true</c> contain cache.</param>
	public void DisposeAll (bool containCache = false)
	{
		StopAllCoroutines();
		m_CurrentCount = 0;
		List<Asset> assets = new List<Asset> ();
		foreach (Asset asset in loadedKV.Values) {
			if (containCache) {
				assets.Add (asset);
			} else if (!asset.cached) {
				assets.Add (asset);
			}
		}
		for (int i = 0; i < assets.Count; ++i) {
			DisposeAsset (assets [i]);
		}
	}



	IEnumerator LoadingAsset (Asset asset, System.Action<Asset> onLoaded)
	{
		while(m_CurrentCount>=maxCount) {
			yield return 0;
		}
		++m_CurrentCount;

		if (asset.path == AssetPath.StreamingAssets) {
			WWW www = new WWW (streamingAssetPath + asset.url);
			while (!www.isDone) {
				yield return 0;
			}
			if (www.error == null || www.error.Length == 0) {

				if (asset.type == AssetType.Sprite) {
					
					asset.texture = asset.textureReadonly ? www.textureNonReadable : www.texture;
					asset.texture.wrapMode = asset.warpMode;
					asset.sprite = Sprite.Create (asset.texture, new Rect (0f, 0f, asset.texture.width, asset.texture.height), Vector2.one * 0.5f, 100, 1, asset.meshType);
					asset.sprite.name = asset.texture.name;				
					loadedKV [asset.url] = asset;
					if (onLoaded != null)
						onLoaded (asset);

				} else if (asset.type == AssetType.Texture2D) {
					
					asset.texture = asset.textureReadonly ? www.textureNonReadable : www.texture;
					asset.texture.wrapMode = asset.warpMode;
					loadedKV [asset.url] = asset;
					if (onLoaded != null)
						onLoaded (asset);

				} else if (asset.type == AssetType.Sprites && asset.url.LastIndexOf (".xml") > -1) {

					StartCoroutine (LoadingSpritesByXML (www.text, asset, onLoaded));

				}

			} else if (debug) {
				print (www.error);
			}
			www.Dispose ();
		} else if (asset.path == AssetPath.Resources) {
			
			if (asset.type == AssetType.Sprite) {

				ResourceRequest rr = Resources.LoadAsync<Sprite> (asset.url);
				yield return rr;
				if (rr.isDone) {
					asset.sprite = rr.asset as Sprite;
					if (asset.sprite) {
						asset.texture = asset.sprite.texture;
						loadedKV [asset.url] = asset;
						if (onLoaded != null)
							onLoaded (asset);
					}
				}


			} else if (asset.type == AssetType.Texture2D) {

				ResourceRequest rr = Resources.LoadAsync<Texture2D> (asset.url);
				yield return rr;
				if (rr.isDone) {
					asset.texture = rr.asset as Texture2D;
					loadedKV [asset.url] = asset;
					if (onLoaded != null)
						onLoaded (asset);
				}

			} else if (asset.type == AssetType.Sprites) {

				Sprite[] sprites = Resources.LoadAll<Sprite> (asset.url);
				if (sprites != null && sprites.Length > 0) {
					asset.sprites = new Dictionary<string, Sprite> ();
					foreach (Sprite s in sprites) {
						asset.sprites [s.name] = s;
					}
					loadedKV [asset.url] = asset;
					if (onLoaded != null)
						onLoaded (asset);
				} else if (debug) {
					print ("ResManager>>>>> Load error :" + asset.url);
				}
			}
		}
		--m_CurrentCount;
	}

	IEnumerator LoadingSpritesByXML (string config, Asset asset, System.Action<Asset> onLoaded)
	{
		string atlasPath = asset.url.Substring (0, asset.url.LastIndexOf (".xml")) + ".png";
		WWW www = new WWW (streamingAssetPath + atlasPath);
		while (!www.isDone) {
			yield return 0;
		}
		if (www.error == null || www.error.Length == 0) {

			asset.texture = asset.textureReadonly ? www.textureNonReadable : www.texture;
			
			XmlDocument xmlDoc = new XmlDocument ();
			xmlDoc.LoadXml (config);
			XmlNode root = xmlDoc.SelectSingleNode ("TextureAtlas");
			XmlNodeList nodeList = root.ChildNodes;
			
			asset.sprites = new Dictionary<string, Sprite> ();
			//遍历所有子节点
			foreach (XmlNode xn in nodeList) {
				if (!(xn is XmlElement))
					continue;
				XmlElement xe = (XmlElement)xn;
				string frameName = xe.GetAttribute ("name").Replace ('/', '_');
				float x = float.Parse (xe.GetAttribute ("x"));
				float y = float.Parse (xe.GetAttribute ("y"));
				float w = float.Parse (xe.GetAttribute ("width"));
				float h = float.Parse (xe.GetAttribute ("height"));
//				float fx = x;
//				float fy = y;
//				float fw = w;
//				float fh = h;
//				if(xe.HasAttribute("fx"))
//					fx = float.Parse( xe.GetAttribute("frameX"));
//				if(xe.HasAttribute("fy"))
//					fy = float.Parse( xe.GetAttribute("frameY"));
//				if(xe.HasAttribute("fw"))
//					fw = float.Parse( xe.GetAttribute("frameWidth"));
//				if(xe.HasAttribute("fh"))
//					fh = float.Parse( xe.GetAttribute("frameHeight"));
				Sprite s = Sprite.Create (asset.texture, new Rect (x, asset.texture.height - h - y, w, h), Vector2.one * 0.5f, 100, 1, asset.meshType);
				s.name = frameName;
				asset.sprites [frameName] = s;
			}
			loadedKV [asset.url] = asset;
			if (onLoaded != null)
				onLoaded (asset);
			
		} else if (debug) {
			print (www.error);
		}
		www.Dispose ();
	}
}
