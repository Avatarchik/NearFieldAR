using Emgu.CV.CvEnum;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.IO.Ports;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.Util;
using System.Drawing;
using System.Runtime.InteropServices;


public class ObjectTrackingRight : MonoBehaviour {
	public string spName = "COM4";
	public static SerialPort sp;
	public string deviceName = "UI325xLE-C_4102832627";
	AVProLiveCameraDevice device;
	MeshRenderer mr;
	Image<Bgr, Byte> oriImage;
	RenderTexture was;
	Texture2D tex;
	float time1;
	float time2;

	const int FRAME_WIDTH = 800;
	const int FRAME_HEIGHT = 600;
	int servoPosition = 90;
	Mat nonZeroCoordinates;
	MCvScalar avgPixelIntensity;
	int diff = 0;
	// Use this for initialization

	void Start () {
		sp = new SerialPort(spName, 9600, Parity.None, 8, StopBits.One);
		OpenConnection ();
		AVProLiveCameraManager.Instance.GetDevice(deviceName).Start(-1);    

		mr = GetComponent<MeshRenderer> ();
		tex = new Texture2D (FRAME_WIDTH, FRAME_HEIGHT);
		nonZeroCoordinates = new Mat();
		StartCoroutine ("SendDiff");
    }

	private void UpdateCameras()
	{
		device = AVProLiveCameraManager.Instance.GetDevice(deviceName);

		device.Update(false);
		mr.material.mainTexture = AVProLiveCameraManager.Instance.GetDevice(deviceName).OutputTexture;
		was = RenderTexture.active;
		RenderTexture.active = (RenderTexture)mr.material.mainTexture;
		tex.ReadPixels (new UnityEngine.Rect(0, 0, FRAME_WIDTH, FRAME_HEIGHT), 0, 0);
		tex.Apply ();
		RenderTexture.active = was;
	/*	oriImage = Texture2dToMat (tex);*/
		oriImage = Texture2dToImage<Bgr, byte> (tex, true);
        Image<Hsv, Byte> hsv_image = oriImage.Convert<Hsv, Byte>();

        // Change the HSV value here
        Hsv hsvmin = new Hsv(100, 150, 150);
        Hsv hsvmax = new Hsv(200, 255, 255);

        hsv_image = hsv_image.SmoothGaussian(5, 5, 0.1, 0.1);

        Image<Gray, byte> red_object = hsv_image.InRange(hsvmin, hsvmax);

        red_object = red_object.Erode(1);
        red_object = red_object.Dilate(1);

		CvInvoke.FindNonZero (red_object, nonZeroCoordinates);
		avgPixelIntensity = CvInvoke.Mean(nonZeroCoordinates);
	//	Debug.Log (avgPixelIntensity.V1);
		CvInvoke.Imshow("right image", red_object); //Show the image
  //    CvInvoke.WaitKey(30);
    }

	IEnumerator SendDiff()
	{
		while (true) {
			yield return new WaitForSeconds (0.11f);
			diff = 0;
			if (nonZeroCoordinates.Rows > 10000)
				diff = (int)(avgPixelIntensity.V1 - (double)(FRAME_HEIGHT / 2));
			Debug.Log (diff);
			sp.Write (diff + "");
		}
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

	public static Image<TColor, TDepth> Texture2dToImage<TColor, TDepth>(Texture2D texture, bool correctForVerticleFlip = true)
         where TColor : struct, IColor
         where TDepth : new()
    {
    	int width = texture.width;
        int height = texture.height;

        Image<TColor, TDepth> result = new Image<TColor, TDepth>(width, height);
        try
        {
        	Color32[] colors = texture.GetPixels32();
            GCHandle handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            using (Image<Rgba, Byte> rgba = new Image<Rgba, byte>(width, height, width * 4, handle.AddrOfPinnedObject()))
            {
               result.ConvertFrom(rgba);
            }
            handle.Free();
        }
        catch (Exception)
        {
        	byte[] jpgBytes = texture.EncodeToJPG();
            using (Mat tmp = new Mat())
            {
               CvInvoke.Imdecode(jpgBytes, LoadImageType.AnyColor, tmp);
               result.ConvertFrom(tmp);
            }
        }
        if (correctForVerticleFlip)
            CvInvoke.Flip(result, result, Emgu.CV.CvEnum.FlipType.Vertical);
        return result;
     }

    public void OpenConnection()
	{
        print("looking for port");
		if (sp != null) {
			if (sp.IsOpen) {
				sp.Close ();
				print ("Closing port, because it was already open");
			} else {
				sp.Open ();
				sp.ReadTimeout = 50;  // sets the timeout value before reporting error
				print ("Port Opened!s");
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
		AVProLiveCameraManager.Instance.GetDevice (deviceName).Close ();
		device.Close ();
		sp.Close();
	}
}