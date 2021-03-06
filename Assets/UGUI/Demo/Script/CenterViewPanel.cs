﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CenterViewPanel : MonoBehaviour {

    /// <summary>
    /// 初始化显示的页码.
    /// </summary>
    public int initPageIndex = 0;

    private CenterView _centerView;

	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForEndOfFrame();
		_centerView = GetComponentInChildren<CenterView>();
		_centerView.Init();
		_centerView.GotoPage(initPageIndex);
	}

    public void OnSelectItem()
    {
        print("Center View 选择了：" + _centerView.CenterItem.name);
    }
}
