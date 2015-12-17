using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ActiveManager : MonoBehaviour {
	private float angle;
	private float speed = 0.5f;
	public Transform TranslateLeft;
	public Transform TranslateRight;
	public Transform ScaleLeft;
	public Transform ScaleRight;
	public Transform RightPlane;
	public Transform LeftView;
	public Transform RightView;
	public Transform VirtualObjectPos;
	void Start () {
	
	}

	//public float CurrentZoom;
	
	// Update is called once per frame
	void Update () {


		if (Input.GetKey(KeyCode.JoystickButton9) ) 
		{
			ScaleLeft.localScale += new Vector3(0.04f, 0.04f, 0.04f);
			ScaleRight.localScale += new Vector3(0.04f, 0.04f, 0.04f);
			//leftCamera
		}

		if (Input.GetKey(KeyCode.JoystickButton8)) 
		{
			ScaleLeft.localScale -= new Vector3(0.04f, 0.04f, 0.04f);
			ScaleRight.localScale -= new Vector3(0.04f, 0.04f, 0.04f);
			//leftCamera
		}

		if (Input.GetKey (KeyCode.C)) 
		{
			TranslateLeft.localPosition += new Vector3(0.04f, 0.0f, 0.0f);
			TranslateRight.localPosition -= new Vector3(0.04f, 0.0f, 0.0f);
			//leftCamera
		}
		
		if (Input.GetKey (KeyCode.V)) 
		{
			TranslateLeft.localPosition -= new Vector3(0.04f, 0.0f, 0.0f);
			TranslateRight.localPosition += new Vector3(0.04f, 0.0f, 0.0f);
			//leftCamera
		}

		if (Input.GetKey (KeyCode.A)) 
		{
			angle += 200 * Time.deltaTime * speed;
			LeftView.eulerAngles = new Vector3(0, angle, 0);
			RightView.eulerAngles = new Vector3(0, -angle, 0);
		}
		
		if (Input.GetKey (KeyCode.D)) 
		{
			angle -= 200 * Time.deltaTime * speed;
			LeftView.eulerAngles = new Vector3(0, angle, 0);
			RightView.eulerAngles = new Vector3(0, -angle, 0);
		}

		if (Input.GetKey (KeyCode.W)) 
		{
			RightPlane.localPosition += new Vector3(0.0f, 0.04f, 0.0f);

		}

		if (Input.GetKey (KeyCode.S)) 
		{
			RightPlane.localPosition -= new Vector3(0.0f, 0.04f, 0.0f);
		}

		if (Input.GetKey (KeyCode.LeftArrow)) 
		{
			VirtualObjectPos.localPosition += new Vector3(0.0f, 0.0f, 0.04f);
		}
		
		if (Input.GetKey (KeyCode.RightArrow)) 
		{
			VirtualObjectPos.localPosition -= new Vector3(0.0f, 0.0f, 0.04f);
		}
		
		if (Input.GetKey (KeyCode.UpArrow)) 
		{
			VirtualObjectPos.localPosition += new Vector3(0.0f, 0.04f, 0.0f);
		}
		
		if (Input.GetKey (KeyCode.DownArrow)) 
		{
			VirtualObjectPos.localPosition -= new Vector3(0.0f, 0.04f, 0.0f);
		}
	}
}
