using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using UnityEngine.UI;
using System.IO;
using System.IO.Ports;

public class ObjectTrackingRight : MonoBehaviour {
	public string spName = "COM6";
	public static SerialPort sp;
	public string deviceName = "UI325xLE-C_4102832627";
	MeshRenderer mr;
	bool useMorphOps = true;
	private CvMat oriImage;
	private CvMat Gray;
	private CvMat threshold;
	RenderTexture was;
	Texture2D tex;


	const int FRAME_WIDTH = 640;
	const int FRAME_HEIGHT = 480;

	int servoPosition = 90;
	int servoOrientation = 0;
	// Use this for initialization
	CvMemStorage p_strStorage;


	void Start () {
		sp = new SerialPort(spName, 9600, Parity.None, 8, StopBits.One);
		OpenConnection ();
		AVProLiveCameraManager.Instance.GetDevice(deviceName).Start(-1);    
		mr = GetComponent<MeshRenderer> ();
		oriImage = new CvMat (FRAME_WIDTH, FRAME_HEIGHT, MatrixType.U8C3);
		Gray = new CvMat (FRAME_HEIGHT, FRAME_WIDTH, MatrixType.U8C1);
		tex = new Texture2D (FRAME_WIDTH, FRAME_HEIGHT);
		p_strStorage = Cv.CreateMemStorage (0);

	}

	private void UpdateCameras()
	{
		AVProLiveCameraDevice device = AVProLiveCameraManager.Instance.GetDevice(deviceName);
		device.Update(false);
		mr.material.mainTexture = AVProLiveCameraManager.Instance.GetDevice(deviceName).OutputTexture;
		was = RenderTexture.active;
		RenderTexture.active = (RenderTexture)mr.material.mainTexture;
		tex.ReadPixels (new UnityEngine.Rect(0, 0, FRAME_WIDTH, FRAME_HEIGHT), 0, 0);
		tex.Apply ();
		RenderTexture.active = was;
		oriImage = Texture2dToMat (tex);
		Cv.CvtColor (oriImage, Gray, ColorConversion.BgrToGray);
		CvSeq<CvCircleSegment> circl = Gray.HoughCircles (p_strStorage, HoughCirclesMethod.Gradient, 1, 100, 150, 55, 0, 0);

		Debug.Log (circl.Total);
	//
/*		if (circl.Total == 0) {
			if (servoOrientation == 0) {
				if (servoPosition >= 90)
					servoOrientation = 1;
				else
					servoOrientation = -1;
			}

			if (servoOrientation == 1) {
				sp.Write ("l");
				servoPosition += 5;
				if (servoPosition > 180) {
					servoPosition = 180;
					servoOrientation = -1;
				}
			} else {
				sp.Write ("r");
				servoPosition -= 5;
				if (servoPosition < 0) {
					servoPosition = 0;
					servoOrientation = 1;
				}
			}
		}*/
		// Run this if the camera can see at least one circle
		if (circl.Total == 1) {
			foreach (CvCircleSegment crcl in circl) {
				Debug.Log (crcl.Center);	// x position of center point of circle
				// y position of center point of circle
				Debug.Log (crcl.Radius);	// radius of circle
				servoOrientation = 0;

				// Check whether camera should turn to its left if the circle gets near the right end of the screen
				if (crcl.Center.Y > 280) {
					sp.Write ("l");
					servoPosition += 5;

					if (servoPosition > 180)
						servoPosition = 180;
				}

				// Check whether camera should turn to its right if the circle gets near the left end of the screen
				if (crcl.Center.Y < 200) {
					sp.Write ("r");
					servoPosition -= 5;

					if (servoPosition < 0)
						servoPosition = 0;
				}
				oriImage.Circle (crcl.Center, (int)crcl.Radius, CvColor.Red, 3);
			}
		}
	//	circl.Dispose ();
	//	p_strStorage.Dispose ();
		Cv.ShowImage ("RightImage", oriImage);
//		Cv.ShowImage("RightGray", Gray);
	//	CvWindow.WaitKey (10);
	}

	private int _lastFrameCount;
	void OnRenderObject()
	{
		if (_lastFrameCount != Time.frameCount)
		{
			_lastFrameCount = Time.frameCount;
			UpdateCameras();
		}
	}

	private CvMat Texture2dToMat(Texture2D tex)
	{
		//Debug.Log (tex.height);
		CvMat a = new CvMat(tex.height, tex.width, MatrixType.U8C3);
		var cols = tex.GetPixels();
		for (int i = 0; i < tex.height; i++)
		{
			for (int j = 0; j < tex.width; j++)
			{
				CvColor cvcol;
				Color col;
				col = cols[j + i * tex.width];
				cvcol.R = (byte)(col.r * 255);
				cvcol.G = (byte)(col.g * 255);
				cvcol.B = (byte)(col.b * 255);
				a.Set2D(tex.height - 1 - i, j, cvcol);
			}
		}
		return a;
	}

	public void OpenConnection()
	{
		if (sp != null) {
			if (sp.IsOpen) {
				sp.Close ();
				print ("Closing port, because it was already open");
			} else {
				sp.Open ();
				sp.ReadTimeout = 50;  // sets the timeout value before reporting error
				print ("Port Opened!");
			}
		} else {
			if(sp.IsOpen)
			{
				print("Port is already open");
			}
			else
			{
				print("Port == null");
			}
		}
	}

	void OnApplicationQuit() 
	{
		sp.Close();
	}

/*	void morphOps(ref CvMat thresh) {
		// Maybe it is a DLL problem, is the correct DLL file in the Assets folder?

		// This causes error:
//		 		CvMat erodeElement = Cv2.GetStructuringElement (StructuringElementShape.Rect, new Size(3, 3));
		// Not assigning to erodeElement does not cause error:
//		 		Cv2.GetStructuringElement (StructuringElementShape.Rect, new Size(3, 3));
		// These do not cause errors:
//		var erodeElement = Cv2.GetStructuringElement (StructuringElementShape.Rect, new Size(3, 3));
//		var dilateElement = Cv2.GetStructuringElement (StructuringElementShape.Rect, new Size (8, 8));

		// Or you could try something like this:
//		 CvInvoke.cvCreateStructuringElementEx(3,3,1,1,StructuringElementShape.Rect, null);

		// I do not know how to convert Mat to InputArray
		// This causes error
//		InputArray thresharr = new InputArray (thresh); 

		//Cv2.Erode (thresh, thresh, erodeElement, Cv.Point (1, 1), 1, 0, default(CvScalar));
		//Cv2.Erode (thresh, thresh, erodeElement);
		//Cv2.Erode (thresharr, thresharr, erodeElement, default(CvPoint), 1, 0, default(CvScalar?));
		//Cv2.Dilate (thresh, thresh, dilateElement);
		//Cv2.Dilate (thresh, thresh, dilateElement);
	}
*/
}