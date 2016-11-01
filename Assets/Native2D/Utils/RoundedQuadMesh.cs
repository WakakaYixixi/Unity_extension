using UnityEngine;
using System.Collections;

/// <summary>
/// Rounded corner mesh.
/// </summary>
[ExecuteInEditMode]
public class RoundedQuadMesh : MonoBehaviour
{
	public float RoundEdges = 0.5f;
	public float RoundTopLeft = 0.0f;
	public float RoundTopRight = 0.0f;
	public float RoundBottomLeft = 0.0f;
	public float RoundBottomRight = 0.0f;
	public float Width=1f;
	public float Height=1f;
	[Range(2,32)]
	public int CornerVertexCount = 8;
	public bool CreateUV = true;
	public bool DoubleSided = false;

	private MeshFilter m_MeshFilter;
	private MeshRenderer m_Renderer;
	private Mesh m_Mesh;
	private Vector3[] m_Vertices;
	private Vector3[] m_Normals;
	private Vector2[] m_UV;
	private int[] m_Triangles;
	private Material m_Mat;

	[SerializeField]
	private Texture m_texture;
	public Texture texture{
		get{  return m_texture; }
		set{
			m_texture = value;
			if(m_Mat) m_Mat.mainTexture = m_texture;
		}
	}

	[SerializeField]
	public string m_SortingLayerName = "Default";
	public string sortingLayerName{
		get{  return m_SortingLayerName; }
		set{
			m_SortingLayerName = value;
			if(m_Renderer) m_Renderer.sortingLayerName = value;
		}
	}

	[SerializeField]
	public int m_SortingOrder = 0;
	public int sortingOrder{
		get{  return m_SortingOrder; }
		set{
			m_SortingOrder = value;
			if(m_Renderer) m_Renderer.sortingOrder = value;
		}
	}

	void Start ()
	{
		m_MeshFilter = GetComponent<MeshFilter>();
		m_Renderer = GetComponent<MeshRenderer>();
		if (m_MeshFilter == null)
			m_MeshFilter = gameObject.AddComponent<MeshFilter>();
		if (m_Renderer == null)
			m_Renderer = gameObject.AddComponent<MeshRenderer>();
		m_Mesh = new Mesh();
		m_MeshFilter.sharedMesh = m_Mesh;
		m_Mat = new Material(Shader.Find("Sprites/Default"));
		m_Renderer.sharedMaterial=m_Mat;
		if(m_texture) m_Mat.mainTexture = m_texture;
		sortingLayerName = m_SortingLayerName;
		sortingOrder = m_SortingOrder;
		UpdateMesh();
	}

	public Mesh UpdateMesh()
	{
		if (CornerVertexCount<2)
			CornerVertexCount = 2;
		int sides = DoubleSided ? 2 : 1;
		int vCount = CornerVertexCount * 4 * sides + sides;
		int triCount = (CornerVertexCount * 4) * sides;
		if (m_Vertices == null || m_Vertices.Length != vCount)
		{
			m_Vertices = new Vector3[vCount];
			m_Normals = new Vector3[vCount];
		}
		if (m_Triangles == null || m_Triangles.Length != triCount * 3)
			m_Triangles = new int[triCount * 3];
		if (CreateUV && (m_UV == null || m_UV.Length != vCount))
		{ 
			m_UV = new Vector2[vCount];
		}
		float f = 1f / (CornerVertexCount-1);
		m_Vertices[0] = Vector3.zero;
		int count = CornerVertexCount * 4;
		if (CreateUV)
		{
			m_UV[0] = Vector2.one *0.5f;
			if (DoubleSided)
				m_UV[count + 1] = m_UV[0];
		}

		for (int i = 0; i < CornerVertexCount; i++ )
		{
			float s = Mathf.Sin((float)i * Mathf.PI * 0.5f*f);
			float c = Mathf.Cos((float)i * Mathf.PI * 0.5f*f);
			float tl = Mathf.Clamp01(RoundTopLeft + RoundEdges);
			float tr = Mathf.Clamp01(RoundTopRight + RoundEdges);
			float bl = Mathf.Clamp01(RoundBottomLeft + RoundEdges);
			float br = Mathf.Clamp01(RoundBottomRight + RoundEdges);
			Vector2 v1 = new Vector3(-Width + tl - c * tl, - tl + s * tl+Height);
			Vector2 v2 = new Vector3(Width - tr + s * tr,  - tr + c * tr+Height);
			Vector2 v3 = new Vector3(Width - br + c * br,  br - s * br-Height);
			Vector2 v4 = new Vector3(-Width + bl - s * bl, bl - c * bl-Height);

			m_Vertices[1 + i] = v1;
			m_Vertices[1 + CornerVertexCount + i] = v2;
			m_Vertices[1 + CornerVertexCount * 2 + i] = v3;
			m_Vertices[1 + CornerVertexCount * 3 + i] = v4;
			if (CreateUV)
			{
				m_UV[1 + i] = v1 * 0.5f + Vector2.one * 0.5f;
				m_UV[1 + CornerVertexCount * 1 + i] = v2 * 0.5f + Vector2.one * 0.5f;
				m_UV[1 + CornerVertexCount * 2 + i] = v3 * 0.5f + Vector2.one * 0.5f;
				m_UV[1 + CornerVertexCount * 3 + i] = v4 * 0.5f + Vector2.one * 0.5f;
			}
			if (DoubleSided)
			{
				// The backside vertices are in reverse order
				m_Vertices[1 + CornerVertexCount * 7 + CornerVertexCount - i] = v1 ;
				m_Vertices[1 + CornerVertexCount * 6 + CornerVertexCount - i] = v2 ;
				m_Vertices[1 + CornerVertexCount * 5 + CornerVertexCount - i] = v3 ;
				m_Vertices[1 + CornerVertexCount * 4 + CornerVertexCount - i] = v4 ;
				if (CreateUV)
				{
					m_UV[1 + CornerVertexCount * 7 + CornerVertexCount - i] = v1 * 0.5f + Vector2.one * 0.5f;
					m_UV[1 + CornerVertexCount * 6 + CornerVertexCount - i] = v2 * 0.5f + Vector2.one * 0.5f;
					m_UV[1 + CornerVertexCount * 5 + CornerVertexCount - i] = v3 * 0.5f + Vector2.one * 0.5f;
					m_UV[1 + CornerVertexCount * 4 + CornerVertexCount - i] = v4 * 0.5f + Vector2.one * 0.5f;
				}
			}
		}
		for (int i = 0; i < count + 1;i++ )
		{
			m_Normals[i] = -Vector3.forward;
			if (DoubleSided)
				m_Normals[count + 1 + i] = Vector3.forward;
		}

		for (int i = 0; i < count; i++)
		{
			m_Triangles[i*3    ] = 0;
			m_Triangles[i*3 + 1] = i + 1;
			m_Triangles[i*3 + 2] = i + 2;
			if (DoubleSided)
			{
				m_Triangles[(count + i) * 3] = count+1;
				m_Triangles[(count + i) * 3 + 1] = count+1 +i + 1;
				m_Triangles[(count + i) * 3 + 2] = count+1 +i + 2;
			}
		}
		m_Triangles[count * 3 - 1] = 1;
		if (DoubleSided)
			m_Triangles[m_Triangles.Length - 1] = count + 1 + 1;

		m_Mesh.Clear();
		m_Mesh.vertices = m_Vertices;
		m_Mesh.normals = m_Normals;
		if (CreateUV)
			m_Mesh.uv = m_UV;
		m_Mesh.triangles = m_Triangles;

		return m_Mesh;
	}
	#if UNITY_EDITOR
	void Update ()
	{
		UpdateMesh();
		texture = m_texture;
		sortingLayerName = m_SortingLayerName;
		sortingOrder = m_SortingOrder;
	}
	#endif
}