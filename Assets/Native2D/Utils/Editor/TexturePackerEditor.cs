using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Xml;
using System.Collections.Generic;

/// <summary>
/// 解析texturepacker图集 , 转成Sprites
/// Author:bingheliefeng
/// </summary>
public class TexturePackerEditor : Editor {

	[MenuItem("Tools/TexturePacker to Sprites")]
	static void ParseXML(){
		if(Selection.activeObject && Selection.activeObject is TextAsset)
		{
			string xmlPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			string xml = AssetDatabase.LoadAssetAtPath<TextAsset>(xmlPath).text.ToString();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);
			XmlNode root = xmlDoc.SelectSingleNode("TextureAtlas");
			XmlElement rootEle = (XmlElement)root;
			string imagePath = "";
			if(rootEle.HasAttribute("imagePath")){
				imagePath =rootEle.GetAttribute("imagePath");
			}
			else if(rootEle.HasAttribute("name")){
				imagePath =rootEle.GetAttribute("name")+".png";
			}

			imagePath = xmlPath.Substring( 0, xmlPath.LastIndexOf('/') ) +"/"+imagePath;
			Texture2D t = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
			if(t==null) return;

			XmlNodeList nodeList =root.ChildNodes;
			List<SpriteMetaData> metaDatas=new List<SpriteMetaData>();
			//遍历所有子节点
			foreach (XmlNode xn in nodeList)
			{
				XmlElement xe = (XmlElement)xn;
				string name = xe.GetAttribute("name").Replace('/','_');
				float x = float.Parse( xe.GetAttribute("x"));
				float y = float.Parse( xe.GetAttribute("y"));
				float w = float.Parse( xe.GetAttribute("width"));
				float h = float.Parse( xe.GetAttribute("height"));
				float fx = x;
				float fy = y;
				float fw = w;
				float fh = h;
				if(xe.HasAttribute("fx"))
					fx = float.Parse( xe.GetAttribute("frameX"));
				if(xe.HasAttribute("fy"))
					fy = float.Parse( xe.GetAttribute("frameY"));
				if(xe.HasAttribute("fw"))
					fw = float.Parse( xe.GetAttribute("frameWidth"));
				if(xe.HasAttribute("fh"))
					fh = float.Parse( xe.GetAttribute("frameHeight"));

				SpriteMetaData metaData = new SpriteMetaData();
				metaData.name = name;
				metaData.rect = new Rect(x,t.height-h-y,w,h);
				metaDatas.Add(metaData);
			}

			if(metaDatas.Count>0){
				string textureAtlasPath = AssetDatabase.GetAssetPath(t);
				TextureImporter textureImporter = AssetImporter.GetAtPath(textureAtlasPath) as TextureImporter;
				textureImporter.maxTextureSize = 2048;
				textureImporter.spritesheet = metaDatas.ToArray();
				textureImporter.textureType = TextureImporterType.Sprite;
				textureImporter.spriteImportMode = SpriteImportMode.Multiple;
				textureImporter.spritePixelsPerUnit = 100;
				AssetDatabase.ImportAsset(textureAtlasPath, ImportAssetOptions.ForceUpdate);
			}
		}
	}
}
