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
	}
}