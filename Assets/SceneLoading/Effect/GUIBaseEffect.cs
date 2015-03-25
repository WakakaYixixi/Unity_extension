using UnityEngine;
using System.Collections;
using System;

public abstract class GUIBaseEffect : MonoBehaviour {
	
	public Action OnComplete = null;
	public abstract void EffectIn();
	public abstract void EffectOut();
}
