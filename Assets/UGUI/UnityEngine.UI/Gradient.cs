using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	/// <summary>
	/// UI gradient
	/// </summary>
	[AddComponentMenu("UI/Effects/Gradient")]
	public class Gradient : BaseMeshEffect {
		[SerializeField]
		private Color32 topColor = Color.white;
		[SerializeField]
		private Color32 bottomColor = Color.black;

#if UNITY_5_2_1
		public override void ModifyMesh (VertexHelper vh){
			if (!IsActive()) {
				return;
			}
			float bottomY = 0;
			float topY = 0;
			for(int i=0;i<vh.currentVertCount;i++){
				UIVertex v=new UIVertex();
				vh.PopulateUIVertex(ref v,i);
				if(i==0){
					bottomY =v.position.y;
					topY = v.position.y;
				}
				else
				{
					float y = v.position.y;
					if (y > topY) {
						topY = y;
					}
					else if (y < bottomY) {
						bottomY = y;
					}
				}
			}
			float uiElementHeight = topY - bottomY;
			for(int i=0;i<vh.currentVertCount;i++){
				UIVertex v=new UIVertex();
				vh.PopulateUIVertex(ref v,i);
				v.color = Color32.Lerp(bottomColor, topColor, (v.position.y - bottomY) / uiElementHeight);
				vh.SetUIVertex(v,i);
			}
		}
	}
#elif UNITY_5_2

	
	public override void ModifyMesh (Mesh mesh)
	{
		if (!IsActive()) {
			return;
		}
		
		int count = mesh.vertexCount;
		if(count>0){
			
			float bottomY = mesh.vertices[0].y;
			float topY = mesh.vertices[0].y;
			
			for (int i = 1; i < count; i++) {
				float y = mesh.vertices[i].y;
				if (y > topY) {
					topY = y;
				}
				else if (y < bottomY) {
					bottomY = y;
				}
			}
			
			float uiElementHeight = topY - bottomY;
			
			Color32[] cs = mesh.colors32;
			for (int i = 0; i < count; i++) {
				Vector3 uiVertex = mesh.vertices[i];
				cs[i] = Color32.Lerp(bottomColor, topColor, (uiVertex.y - bottomY) / uiElementHeight);
			}
			mesh.colors32 = cs ;
		}
	}


#endif
}