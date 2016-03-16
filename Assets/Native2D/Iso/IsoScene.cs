using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IsoScene : IsoObject {

	private List<IsoObject> m_sprites;

	private PathGrid m_gridData;
	public PathGrid gridData{
		get {
			return m_gridData;
		}
		set {
			m_gridData=value;
		}
	}

	public void Init(int size,int gridX, int gridZ){

	}

	public void AddIsoObject(IsoObject obj,bool isSort = true){

	}

	public void RemoveIsoObject(IsoObject obj){

	}

	public void SortIsoObject(IsoObject obj){

	}

	public void SortAll(){

	}

	public IsoObject GetIsoObjectByNodePos(int nodeX,int nodeZ){

	}

	public List<IsoObject> GetIsoChildren(){
		return m_sprites;
	}

	public void ClearScene(){

	}

	// Update is called once per frame
	protected virtual void Update () {
		
	}
}
