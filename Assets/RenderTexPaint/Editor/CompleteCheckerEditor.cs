﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(PaintCompleteChecker))]
public class CompleteCheckerEditor : Editor {

	private static bool m_showSource = false;

	public override void OnInspectorGUI(){

		EditorGUILayout.Space();
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("gridDefaultStatus"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("brushSize"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("enableColor"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("disableColor"), true);

		EditorGUILayout.PropertyField(serializedObject.FindProperty("assetRectDic"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("assetEnableDic"), true);

		serializedObject.ApplyModifiedProperties();

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Create Check Grid")){
			CreateGrid();
		}
		bool showSource = GUILayout.Toggle(m_showSource,"Show Source Tex","Button");
		if(m_showSource!=showSource){
			PaintCompleteChecker checker = target as PaintCompleteChecker;
			RenderTexturePainter painter = checker.gameObject.GetComponent<RenderTexturePainter>();
			if(showSource){
				if(painter&&painter.sourceTex){
					SpriteRenderer render = checker.GetComponent<SpriteRenderer>();
					if(!render) render = checker.gameObject.AddComponent<SpriteRenderer>();
					render.sprite = Sprite.Create((Texture2D)painter.sourceTex,new Rect(0,0,painter.sourceTex.width,painter.sourceTex.height),Vector2.one*0.5f);
				}
			}
			else
			{
				SpriteRenderer render = checker.GetComponent<SpriteRenderer>();
				if(render){
					DestroyImmediate(render);
				}
			}
			m_showSource = showSource;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Save Grid")){
			SaveGrid();
		}
		if(GUILayout.Button("Read Grid Data")){
			ReadGrid();
		}
		EditorGUILayout.EndHorizontal();
	}

	void CreateGrid(){
		PaintCompleteChecker checker = target as PaintCompleteChecker;
		RenderTexturePainter painter = checker.gameObject.GetComponent<RenderTexturePainter>();
		if( painter && painter.sourceTex && painter.penTex){
			checker.gridsDic = new Dictionary<string,Rect> ();
			checker.enablesDic = new Dictionary<string,bool> ();

			int gridW = Mathf.FloorToInt(painter.penTex.width*painter.brushScale/4f);
			int gridH = Mathf.FloorToInt(painter.penTex.height*painter.brushScale/4f);
			int canvasW = painter.sourceTex.width;
			int canvasH = painter.sourceTex.height;

			for(int w=-canvasW/2;w<=canvasW/2;w+=gridW)
			{
				for(int h=-canvasH/2;h<=canvasH/2;h+=gridH){
					string key =  w*0.01f+"-"+h*0.01f;
					Rect value = new Rect(w*0.01f,h*0.01f,gridH*0.01f,gridW*0.01f);
					checker.gridsDic[key]=value;
					checker.enablesDic[key] = checker.gridDefaultStatus;
				}
			}
		}
	}

	void SaveGrid(){
		//序列化存储
		PaintCompleteChecker checker = target as PaintCompleteChecker;
		if(checker.gridsDic!=null){
			PaintRectDictionary map1 =new PaintRectDictionary();
			PaintEnableDictionary map2 =new PaintEnableDictionary();

			foreach(string key in checker.gridsDic.Keys){
				if(checker.enablesDic[key])
				{
					map1[key] = checker.gridsDic[key];
					map2[key] = checker.enablesDic[key];
				}
			}
			AssetDatabase.CreateAsset(map1,"Assets/Paint_Rect_"+checker.name+".asset");
			AssetDatabase.CreateAsset(map2,"Assets/Paint_Enable_"+checker.name+".asset");
		}
	}

	void ReadGrid()
	{
		PaintCompleteChecker checker = target as PaintCompleteChecker;
		if(checker.assetRectDic!=null){
			checker.gridsDic = checker.assetRectDic.ConvertToDictionary();
			if(checker.assetEnableDic!=null){
				checker.enablesDic = checker.assetEnableDic.ConvertToDictionary();
			}
		}
	}


	void OnSceneGUI() {
		PaintCompleteChecker checker = target as PaintCompleteChecker;
		Handles.color = Color.blue;
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		Event current = Event.current;
		if (checker.gridsDic!=null && (current.control||current.command||current.alt)){
			switch (current.GetTypeForControl(controlID)) {
			case EventType.MouseDown:
				current.Use();
				break;
			case EventType.MouseMove:
				Vector3 p = HandleUtility.GUIPointToWorldRay(current.mousePosition).origin;
				Vector3 localPos = checker.transform.InverseTransformPoint(p);

				Rect brushSize = new Rect(localPos.x-checker.brushSize/2,localPos.y-checker.brushSize/2,checker.brushSize,checker.brushSize);

				if(current.control||current.command){
					foreach(string key in checker.gridsDic.Keys)
					{
						Rect rect = checker.gridsDic[key];
						if(checker.Intersect(rect,brushSize)){
							checker.enablesDic[key]=true;
						}
					}
				}else if(current.alt){
					foreach(string key in checker.gridsDic.Keys)
					{
						Rect rect = checker.gridsDic[key];
						if(checker.Intersect(rect,brushSize)){
							checker.enablesDic[key]=false;
						}
					}
				}
				Event.current.Use();
				break;
			case EventType.Layout:
				HandleUtility.AddDefaultControl(controlID);
				break;
			}
		}
	}
}
