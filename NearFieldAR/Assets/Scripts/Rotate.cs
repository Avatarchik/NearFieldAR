using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	private float angle;
	private float speed = 0.2f;
	public Transform LeftView;
	public Transform RightView;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
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
	}
}
