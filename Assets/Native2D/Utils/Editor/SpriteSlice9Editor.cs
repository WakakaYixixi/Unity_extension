using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

[CustomEditor(typeof(SpriteSlice9))]
public class SpriteSlice9Editor : Editor {

	SpriteSlice9 instance ;

	string[] sortingLayerNames;
	int selectedOption;

	void OnEnable(){
		instance = target as SpriteSlice9;

		sortingLayerNames = GetSortingLayerNames();
		selectedOption = GetSortingLayerIndex(instance.sortLayerName);
	}

	public override void OnInspectorGUI(){
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("sprite"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_color"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("size"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("pivot"), true);
		serializedObject.ApplyModifiedProperties();

		selectedOption = EditorGUILayout.Popup("Sorting Layer", selectedOption, sortingLayerNames);
		if (sortingLayerNames[selectedOption] != instance.sortLayerName)
		{
			Undo.RecordObject(instance, "Sorting Layer");
			instance.sortLayerName = sortingLayerNames[selectedOption];
			EditorUtility.SetDirty(instance);
		}
		int newSortingLayerOrder = EditorGUILayout.IntField("Order in Layer", instance.sortingOrder);
		if (newSortingLayerOrder != instance.sortingOrder)
		{
			Undo.RecordObject(instance, "Edit Sorting Order");
			instance.sortingOrder = newSortingLayerOrder;
			EditorUtility.SetDirty(instance);
		}
	}

	public string[] GetSortingLayerNames() {
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}
	public int[] GetSortingLayerUniqueIDs()
	{
		System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
		return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
	}
	int GetSortingLayerIndex(string layerName){
		for(int i = 0; i < sortingLayerNames.Length; ++i){  
			if(sortingLayerNames[i] == layerName) return i;  
		}  
		return 0;  
	}
}
