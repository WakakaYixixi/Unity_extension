using UnityEngine;
using System.Collections;

public class SpinObject : MonoBehaviour {

	public float RotationSpeed = 50;

	private float resultX = 0;
	private float resultY = 0;

	public bool isTile=true;

	public float tileMinAngle = 0f;
	public float tileMaxAngle = 90;

	private Vector3 mousePostion;

	void Start(){
		if ( !(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)) {
			RotationSpeed*=10;
		}
	}

	void Update()  
	{  
		if (Input.GetMouseButton(0))  
		{  
			Vector3 currentMousePos = Input.mousePosition;

			float deltaX = Input.GetAxis("Mouse X");
			float deltaY = Input.GetAxis("Mouse Y");
			if (Input.touchCount > 0)
			{
				deltaX = Input.touches[0].deltaPosition.x;
				deltaY = Input.touches[0].deltaPosition.y;
			}

			resultX = -deltaX*RotationSpeed*Time.deltaTime;
			resultY = deltaY*RotationSpeed*Time.deltaTime;
		}  

		if(!isTile){
			if(resultX!=0){
				resultX = Mathf.Lerp(resultX,0,0.25f);
				if(Mathf.Abs(resultX)>0.01f){
					transform.Rotate(Vector3.up*resultX);
				}
			}
		}
		else
		{
			if(resultY!=0){
				resultY = Mathf.Lerp(resultY,0,0.25f);
				if(Mathf.Abs(resultY)>0.01f){
					float rotaX = (transform.right*resultY).x+transform.localEulerAngles.x;
					rotaX = ClampAngle(rotaX,tileMinAngle,tileMaxAngle);
					transform.localRotation = Quaternion.Euler(rotaX,transform.localEulerAngles.y,0);
				}
			}

		}

	} 
	static float ClampAngle (float angle ,float min ,float max ) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
}
