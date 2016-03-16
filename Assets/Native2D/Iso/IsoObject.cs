using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IsoObject : MonoBehaviour {

	private Vector3 m_pos3D;
	private int m_nodeX = 1;
	private int m_nodeZ = 1;
	private List<Vector3> m_spanPosArray;
	private bool m_isRotated = false;
	private float m_centerOffsetY = 0;

	protected int m_size;
	protected int m_spanX,m_spanZ;

	public bool isSort = false;


	public int spanX{
		get { return m_spanX; }
	}
	public int spanZ{
		get { return m_spanZ; }
	}
	public int size{
		get { return m_size; }
	}
	public List<Vector3> spanPosArray{
		get { return m_spanPosArray; }
	}
	public float x{
		get { return m_pos3D.x; }
		set {
			m_pos3D.x = value;
			UpdateScreenPos();
			UpdateSpanPos();
		}
	}
	public float y{
		get { return m_pos3D.y; }
		set {
			m_pos3D.y = value;
			UpdateScreenPos();
			UpdateSpanPos();
		}
	}
	public float z{
		get { return m_pos3D.z; }
		set {
			m_pos3D.z = value;
			UpdateScreenPos();
			UpdateSpanPos();
		}
	}
	public Vector3 pos3D{
		get { return m_pos3D; }
		set { m_pos3D=value; }
	}
	public int nodeX{
		get {return m_nodeX; }
	}
	public int nodeZ{
		get {return m_nodeZ; }
	}
	public float centerY{
		get {
			return transform.localPosition.y-m_centerOffsetY;
		}
	}
	public float depth{
		//return (this._pos3D.x + this._pos3D.z) * .866 - this._pos3D.y * .707;
		get { return -centerY; }
	}


	public void Init(int size,int spanX,int spanZ){
		m_size=size;
		m_spanX=spanX;
		m_spanZ=spanZ;
		m_centerOffsetY = size*spanX/2f;
	}

	public virtual void Sort(){
		isSort = true;
	}

	public void RotateX( bool value){
		m_isRotated = value;
		UpdateSpanPos();
	}

	public void UpdateSpanPos(){
		m_spanPosArray.Clear();
		int t1=0;
		int t2=0;
		if(m_isRotated){
			t1 = m_spanZ;
			t2 = m_spanX;
		}else{
			t1 = m_spanX;
			t2 = m_spanZ;
		}
		for(int i = 0 ;  i<t1 ; i++)
		{
			for(int j = 0 ; j<t2 ; j++)
			{
				Vector3 pos = new Vector3( i*m_size+x, y, j*m_size+z );
				m_spanPosArray.Add( pos );
			}
		}
	}

	public void SetNodePosition(int nodeX,int nodeZ)
	{
		m_nodeX = nodeZ;
		m_nodeZ = nodeZ;
		m_pos3D.x = nodeX*m_size;
		m_pos3D.z = nodeZ*m_size;
		UpdateScreenPos();
		UpdateSpanPos();

	}

	public void SetScreenPos(float x,float y){
		this.x = x;
		this.y = y;
		m_pos3D.x = x;
		m_pos3D.y = y;
	}

	public void SetWalkable(bool value,PathGrid grid){
		UpdateSpanPos();
		foreach(Vector3 v in m_spanPosArray){
			grid.SetWalkable( Mathf.FloorToInt(v.x/m_size),Mathf.FloorToInt(v.z/m_size),value);
		}
	}

	public bool GetWalkable( PathGrid grid){
		bool flag = false;
		foreach(Vector3 v in m_spanPosArray){
			int nodeX = Mathf.FloorToInt(v.x/m_size);
			int nodeY = Mathf.FloorToInt(v.z/m_size);
			if(nodeX<0 || nodeX>grid.gridX-1) return false;
			if(nodeY<0 || nodeY>grid.gridZ-1) return false;
			flag = grid.GetNode(nodeX,nodeY).walkable;
			if(!flag) return false;
		}
		return true;
	}

	public bool GetRotatable(PathGrid grid)
	{
		if (m_spanX==m_spanZ ) return true;

		SetWalkable(true,grid);
		m_isRotated = ! m_isRotated;
		UpdateSpanPos();
		bool flag = GetWalkable(grid);
		//还原
		m_isRotated = !m_isRotated;
		SetWalkable(false,grid);
		return flag;
	}

	public void UpdateScreenPos(){
		Vector2 ScPos = IsoUtil.IsoToScreen(m_pos3D.x,m_pos3D.y,m_pos3D.z);
		Vector3 pos = transform.localPosition;
		pos.x = ScPos.x ;
		pos.y = ScPos.y ;
		transform.localPosition = pos;
		UpdateSpanPos();
	}

	protected virtual void OnDestory(){
		m_spanPosArray.Clear();
		m_spanPosArray = null;
	}
}
