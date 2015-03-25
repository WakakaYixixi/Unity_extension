using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DragAndDrop3D))]
public class DragAndDrop3DEditor : Editor {

	private int m_rayCastMasksLength=0;
	private LayerMask[] m_rayCastMasks = new LayerMask[0];

	private int m_dropLayerMaskLength=0;
	private LayerMask[] m_dropLayerMask = new LayerMask[0];

	public override void OnInspectorGUI(){

		DragAndDrop3D source = (DragAndDrop3D)target;
		EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal();
		source.dragTarget = (Transform)EditorGUILayout.ObjectField(new GUIContent("Drag Target", "拖动的对象，默认为自己."), source.dragTarget, typeof(Transform),true);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		source.isDragDisableCollider = EditorGUILayout.Toggle(new GUIContent("Is Drag Disable Collider", "Drag时是否禁用此对象的collider组件."), source.isDragDisableCollider);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		source.rayCastCamera = (Camera)EditorGUILayout.ObjectField(new GUIContent("Raycast Camera", "如果为null，则使用mainCamera."), source.rayCastCamera,typeof(Camera),true);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		source.isUseRaycast = EditorGUILayout.Toggle(new GUIContent("Is Use Raycast", "是否使用射线检测.如果是，则设置rayCastMasks中的参数."), source.isUseRaycast);
		EditorGUILayout.EndHorizontal();

		if (source.isUseRaycast) {
			EditorGUILayout.BeginHorizontal();
			source.raycastDistance = EditorGUILayout.FloatField(new GUIContent("Raycast Distance", "射线的检测距离，只用于射线检测时."), source.raycastDistance);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			m_rayCastMasksLength = EditorGUILayout.IntField(new GUIContent("Raycast Masks Length", "Drag Object射线检测的Layer"), m_rayCastMasksLength);
			EditorGUILayout.EndHorizontal();

			if(m_rayCastMasksLength!=source.rayCastMasks.Length){
				source.rayCastMasks = new LayerMask[m_rayCastMasksLength];
				for(int i = 0 ;i<source.rayCastMasks.Length && i<m_rayCastMasks.Length; i++){
					source.rayCastMasks[i] = m_rayCastMasks[i];
				}
				m_rayCastMasks = new LayerMask[m_rayCastMasksLength];
			}
			EditorGUILayout.BeginVertical();
			for(int i = 0; i<m_rayCastMasksLength;i++){
				EditorGUILayout.BeginHorizontal();
				source.rayCastMasks[i] = EditorGUILayout.LayerField(new GUIContent("        Layer "+i, ""), source.rayCastMasks[i]);
				EditorGUILayout.EndHorizontal();
				m_rayCastMasks[i] = source.rayCastMasks[i];
			}
			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.BeginHorizontal();
		source.isDragOriginPoint = EditorGUILayout.Toggle(new GUIContent("Is Drag Origin Point", "在拖动时是否固定在拖动物的原点."), source.isDragOriginPoint);
		EditorGUILayout.EndHorizontal();

		if (source.isDragOriginPoint) {
			EditorGUILayout.BeginHorizontal();
			source.dragOffset = EditorGUILayout.Vector3Field(new GUIContent("Drag Offset", "拖动时的偏移值."), source.dragOffset);
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.BeginHorizontal();
		source.dragMoveDamp = EditorGUILayout.FloatField(new GUIContent("Drag Move Damp", "拖动时的缓动参数."), source.dragMoveDamp);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		source.mousePickLayer = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Mouse Pick Layer", "移动时在哪个面上移动，如果为null，则在拖动物的Z轴面移动."), source.mousePickLayer,typeof(GameObject),true);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		m_dropLayerMaskLength = EditorGUILayout.IntField(new GUIContent("Drop LayerMask Length", "drop容器所在的层."), m_dropLayerMaskLength);
		EditorGUILayout.EndHorizontal();

		if(m_dropLayerMaskLength!=source.dropLayerMasks.Length){
			source.dropLayerMasks = new LayerMask[m_dropLayerMaskLength];
			for(int i = 0 ;i<source.dropLayerMasks.Length && i<m_dropLayerMask.Length; i++){
				source.dropLayerMasks[i] = m_dropLayerMask[i];
			}
			m_dropLayerMask = new LayerMask[m_dropLayerMaskLength];
		}
		EditorGUILayout.BeginVertical();
		for(int i = 0; i<m_dropLayerMaskLength;i++){
			EditorGUILayout.BeginHorizontal();
			source.dropLayerMasks[i] = EditorGUILayout.LayerField(new GUIContent("        Layer "+i, ""), source.dropLayerMasks[i]);
			EditorGUILayout.EndHorizontal();
			m_dropLayerMask[i] = source.dropLayerMasks[i];
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginHorizontal();
		source.dropMedthod = EditorGUILayout.TextField(new GUIContent("Drop Medthod", "drop发生时发送的事件，drop和当前拖动对象都会发送."), source.dropMedthod);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		source.isDropFailBack = EditorGUILayout.Toggle(new GUIContent("Is Drop Fail Back", "如果没有检测到可drop的容器，是否返回原来的位置."), source.isDropFailBack);
		EditorGUILayout.EndHorizontal();

		if (source.isDropFailBack) {
			EditorGUILayout.BeginHorizontal();
			source.dragBackEffect = (DragAndDrop3D.DragBackEffect)EditorGUILayout.EnumPopup(new GUIContent("Drag Back Effect", "返回原来位置时的效果."), source.dragBackEffect);
			EditorGUILayout.EndHorizontal();

			if(source.dragBackEffect== DragAndDrop3D.DragBackEffect.TweenPosition||source.dragBackEffect== DragAndDrop3D.DragBackEffect.TweenScale){
				EditorGUILayout.BeginHorizontal();
				source.backEffectSpeed = EditorGUILayout.FloatField(new GUIContent("Back Effect Speed", "返回原来位置时的速度.对TweenPosition和TweenScale有用."), source.backEffectSpeed);
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.EndVertical();
	}
}
