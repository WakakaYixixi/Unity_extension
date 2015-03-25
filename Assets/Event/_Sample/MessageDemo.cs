using UnityEngine;
using System.Collections;

public class MessageDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Messenger.AddListener<bool> ("OK", Callback);
		Messenger.DispatchEvent ("OK",true);
		Messenger.RemoveListener<bool> ("OK", Callback);
		Messenger.DispatchEvent ("OK",true);
	}
	
	void Callback(bool flag){
		print (flag);
	}
}
