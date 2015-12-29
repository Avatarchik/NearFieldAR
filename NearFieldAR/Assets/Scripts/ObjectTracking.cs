using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;

using UnityEngine.UI;

public class ObjectTracking : MonoBehaviour {
	public string deviceName = "UI325xLE-C_4102826019";
	MeshRenderer mr;
	private CvMat oriImage;
	private CvMat HSV;
	private CvMat threshold;
	RenderTexture was;
	Texture2D tex;
/*	int H_MIN = 0;
	int H_MAX = 256;
	int S_MIN = 0;
	int S_MAX = 256;
	int V_MIN = 0;
	int V_MAX = 256;
*/

	Slider H_MIN_slider;
	Slider H_MAX_slider;
	Slider S_MIN_slider;
	Slider S_MAX_slider;
	Slider V_MIN_slider;
	Slider V_MAX_slider;

	Text H_MIN_text;
	Text H_MAX_text;
	Text S_MIN_text;
	Text S_MAX_text;
	Text V_MIN_text;
	Text V_MAX_text;


	int H_MIN_curr = 0;
	int H_MAX_curr = 0;
	int S_MIN_curr = 0;
	int S_MAX_curr = 255;
	int V_MIN_curr = 255;
	int V_MAX_curr = 255;


	// Use this for initialization
	void Start () {

		AVProLiveCameraManager.Instance.GetDevice(deviceName).Start(-1);    
		mr = GetComponent<MeshRenderer> ();
		oriImage = new CvMat (640, 480, MatrixType.U8C3);
		HSV = new CvMat (480, 640, MatrixType.U8C3);
		tex = new Texture2D (640, 480);
		//createTrackbars ();

		H_MIN_slider = GameObject.Find ("H_MIN_slider").GetComponent<Slider>();
		H_MAX_slider = GameObject.Find ("H_MAX_slider").GetComponent<Slider>();
		S_MIN_slider = GameObject.Find ("S_MIN_slider").GetComponent<Slider>();
		S_MAX_slider = GameObject.Find ("S_MAX_slider").GetComponent<Slider>();
		V_MIN_slider = GameObject.Find ("V_MIN_slider").GetComponent<Slider>();
		V_MAX_slider = GameObject.Find ("V_MAX_slider").GetComponent<Slider>();

		H_MIN_text = GameObject.Find ("H_MIN_slider/Text").GetComponent<Text>();
		H_MAX_text = GameObject.Find ("H_MAX_slider/Text").GetComponent<Text>();
		S_MIN_text = GameObject.Find ("S_MIN_slider/Text").GetComponent<Text>();
		S_MAX_text = GameObject.Find ("S_MAX_slider/Text").GetComponent<Text>();
		V_MIN_text = GameObject.Find ("V_MIN_slider/Text").GetComponent<Text>();
		V_MAX_text = GameObject.Find ("V_MAX_slider/Text").GetComponent<Text>();
	}

	private void UpdateCameras()
	{
		H_MIN_curr = (int)H_MIN_slider.value;
		H_MAX_curr = (int)H_MAX_slider.value;
		S_MIN_curr = (int)S_MIN_slider.value;
		S_MAX_curr = (int)S_MAX_slider.value;
		V_MIN_curr = (int)V_MIN_slider.value;
		V_MAX_curr = (int)V_MAX_slider.value;

		H_MIN_text.text = H_MIN_curr.ToString();
		H_MAX_text.text = H_MAX_curr.ToString();
		S_MIN_text.text = S_MIN_curr.ToString();
		S_MAX_text.text = S_MAX_curr.ToString();
		V_MIN_text.text = V_MIN_curr.ToString();
		V_MAX_text.text = V_MAX_curr.ToString();

		AVProLiveCameraDevice device = AVProLiveCameraManager.Instance.GetDevice(deviceName);
		device.Update(false);
		mr.material.mainTexture = AVProLiveCameraManager.Instance.GetDevice(deviceName).OutputTexture;
		was = RenderTexture.active;
		RenderTexture.active = (RenderTexture)mr.material.mainTexture;
		tex.ReadPixels (new Rect(0, 0, 640, 480), 0, 0);
		tex.Apply ();
		RenderTexture.active = was;
		oriImage = Texture2dToMat (tex);
		//Debug.Log (oriImage.Width);
		//Debug.Log (oriImage.Height);
		//Debug.Log (HSV.Width);
		//Debug.Log (HSV.Height);
		//HSV = oriImage;
		Cv.CvtColor (oriImage, HSV, ColorConversion.BgrToHsv);
		//Cv.ShowImage("HAHA", HSV);
		//Cv.CvtColor (HSV, oriImage, ColorConversion.HsvToBgr);
		//Cv.InRangeS (HSV, Cv.ScalarAll (H_MIN, S_MIN, V_MIN), Cv.ScalarAll (H_MAX, S_MAX, V_MAX), threshold);

		//Cv.InRangeS (HSV[0], new CvScalar (H_MIN), new CvScalar (H_MAX), threshold);
		//Cv.InRangeS (HSV[1], new CvScalar (S_MIN), new CvScalar (S_MAX), threshold);
		//Cv.InRangeS (HSV[2], new CvScalar (V_MIN), new CvScalar (V_MAX), threshold);
		/*Cv.InRange
		ref lower = CvArr (H_MIN_curr, S_MIN_curr,V_MIN_curr);
		CvArr upper = CvArr (H_MAX_curr, S_MAX_curr, V_MAX_curr);
		Cv.InRange (HSV, lower, upper, threshold);
		*/

		// I think this is the correct way
		threshold = new CvMat (480, 640, MatrixType.U8C1);
		Cv.InRangeS (HSV, new CvScalar (H_MIN_curr, S_MIN_curr, V_MIN_curr), new CvScalar (H_MAX_curr, S_MAX_curr, V_MAX_curr), threshold);
		Debug.Log (H_MIN_curr.ToString () + ";" + S_MIN_curr.ToString () + ";" + V_MIN_curr.ToString () + ";" + H_MAX_curr.ToString () + ";" + S_MAX_curr.ToString () + ";" + V_MAX_curr.ToString ());

		Cv.ShowImage("QAQ", threshold);

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
/*	void on_trackbar(int change)
	{
		Debug.Log ("H_MIN_curr: " + H_MIN_curr.ToString());
		Debug.Log ("change: " + change.ToString());
	}
	void createTrackbars() {
		H_MIN_curr = 0;
		H_MAX_curr = 256;
		S_MIN_curr = 0;
		S_MAX_curr = 256;
		V_MIN_curr = 0;
		V_MAX_curr = 256;

		Cv.NamedWindow("Trackbars", WindowMode.FreeRatio);
		Cv.CreateTrackbar( "H_MIN", "Trackbars", ref H_MIN_curr, H_MAX, on_trackbar );
		Cv.CreateTrackbar( "H_MAX", "Trackbars", ref H_MAX_curr, H_MAX, on_trackbar );
		Cv.CreateTrackbar( "S_MIN", "Trackbars", ref S_MIN_curr, S_MAX, on_trackbar );
		Cv.CreateTrackbar( "S_MAX", "Trackbars", ref S_MAX_curr, S_MAX, on_trackbar );
		Cv.CreateTrackbar( "V_MIN", "Trackbars", ref V_MIN_curr, V_MAX, on_trackbar );
		Cv.CreateTrackbar( "V_MAX", "Trackbars", ref V_MAX_curr, V_MAX, on_trackbar );
		on_trackbar (H_MIN_curr);


		//Cv.WaitKey (0);
	}

	void create_unity_trackbar_window()
	{
	}*/

}
