using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 从png图片中提取两张图片，一张是alpha
/// </summary>
public class SplitAlpha {

	[MenuItem("Tools/Split Alpha/RGB+A可能会合并成一张图")]
	static void SplitUseMerge(){
		Split(true);
	}
	[MenuItem("Tools/Split Alpha/RGB+A两张图")]
	static void SplitNonMerge(){
		Split(false);
	}

	public static void Split(bool merge){
		if(Selection.activeObject && Selection.activeObject is Texture2D)
		{
			string name = (Selection.activeObject as Texture2D).name;
			string folder = AssetDatabase.GetAssetPath(Selection.activeObject);

			List<SpriteMetaData> metaDatas = new List<SpriteMetaData>();
			foreach(Object o in AssetDatabase.LoadAllAssetsAtPath(folder))
			{
				if(o is Sprite)
				{
					Sprite s = o as Sprite;
					SpriteMetaData metaData = new SpriteMetaData();
					metaData.name = s.name;
					metaData.rect = s.rect;
					metaData.pivot = s.pivot;
					metaData.border = s.border;
					metaDatas.Add(metaData);
				}
			}

			folder = Application.dataPath.Substring(0,Application.dataPath.LastIndexOf('/')+1) + folder.Substring(0,folder.LastIndexOf('/')+1);
			Texture2D t = new Texture2D(1,1,TextureFormat.ARGB32,false,true);
			t.filterMode = FilterMode.Trilinear;
			t.LoadImage(File.ReadAllBytes(folder+name+".png"));

			Texture2D jpg = new Texture2D(t.width,t.height,TextureFormat.RGB24,false,true);
			Texture2D alphaT = new Texture2D(t.width,t.height,TextureFormat.RGB24,false,true);

			for(int i=0;i<t.width;i++)
			{
				for(int j=0;j<t.height;j++)
				{
					Color c = t.GetPixel(i,j);
					jpg.SetPixel(i,j,c);
					alphaT.SetPixel(i,j,new Color(c.a,c.a,c.a));
					if(c.a==0){
						if(i>0 && t.GetPixel(i-1,j).a>0) jpg.SetPixel(i,j,t.GetPixel(i-1,j));
						else if(i<t.width-1 && t.GetPixel(i+1,j).a>0) jpg.SetPixel(i,j,t.GetPixel(i+1,j));
						else if(j>0 && t.GetPixel(i,j-1).a>0 ) jpg.SetPixel(i,j,t.GetPixel(i,j-1));
						else if(j<t.height-1 && t.GetPixel(i,j+1).a>0) jpg.SetPixel(i,j,t.GetPixel(i,j+1));
						else if(i>0 && j>0 && t.GetPixel(i-1,j-1).a>0) jpg.SetPixel(i,j,t.GetPixel(i-1,j-1));//左上
						else if(i>0 && j<t.height-1 && t.GetPixel(i-1,j+1).a>0) jpg.SetPixel(i,j,t.GetPixel(i-1,j+1));//左下
						else if(i<t.width-1 && j>0 && t.GetPixel(i+1,j-1).a>0) jpg.SetPixel(i,j,t.GetPixel(i+1,j-1));//右上
						else if(i<t.width-1 && j<t.height-1 && t.GetPixel(i+1,j+1).a>0) jpg.SetPixel(i,j,t.GetPixel(i+1,j+1));//右下
					}
				}
			}

			string atlasPath = "";
			if(merge && t.width>t.height)
			{
				//两张图片合成一张
				Texture2D atlasT = new Texture2D(t.width,t.width,TextureFormat.RGB24,false,true);
				atlasT.SetPixels(0,0,t.width,t.height,jpg.GetPixels());
				atlasT.SetPixels(0,t.height,t.width,t.height,alphaT.GetPixels());
				File.WriteAllBytes(folder+name+"_RGB_A.jpg",atlasT.EncodeToJPG(100));
				atlasPath = folder+name+"_RGB_A.jpg";
			}
			else if(merge && t.width<t.height)
			{
				//两张图片合成一张
				Texture2D atlasT = new Texture2D(t.height,t.height,TextureFormat.RGB24,false,true);
				atlasT.SetPixels(0,0,t.width,t.height,jpg.GetPixels());
				atlasT.SetPixels(t.width,0,t.width,t.height,alphaT.GetPixels());
				File.WriteAllBytes(folder+name+"_RGB_A.jpg",atlasT.EncodeToJPG(100));
				atlasPath = folder+name+"_RGB_A.jpg";

			}else{
				byte[] alphaBytes = alphaT.EncodeToJPG(100);
				File.WriteAllBytes(folder+name+"_A.jpg",alphaBytes);

				byte[] jpgbytes = jpg.EncodeToJPG(100);
				File.WriteAllBytes(folder+name+"_RGB.jpg",jpgbytes);
				atlasPath = folder+name+"_RGB.jpg";
			}
			atlasPath = atlasPath.Substring(atlasPath.IndexOf("/Assets")+1);
			if(metaDatas.Count>0){
				AssetDatabase.Refresh();
				SetSprite(metaDatas,atlasPath);
			}

			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		}
	}

	static void SetSprite(List<SpriteMetaData> metaDatas,string atlasPath){
		TextureImporter textureImporter = AssetImporter.GetAtPath(atlasPath) as TextureImporter;
		textureImporter.maxTextureSize = 2048;
		textureImporter.spritesheet = metaDatas.ToArray();
		textureImporter.textureType = TextureImporterType.Sprite;
		textureImporter.spriteImportMode = SpriteImportMode.Multiple;
		textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
		textureImporter.spritePixelsPerUnit = 100;
		textureImporter.mipmapEnabled=false;
		AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceUpdate);
	}
}
