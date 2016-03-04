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
	private static SerialPort sp;
	public string deviceName = "UI325xLE-C_4102832626";
	AVProLiveCameraDevice device;
	MeshRenderer mr;
	Image<Bgr, Byte> oriImage;
	Image<Bgr, Byte> resImage;
	RenderTexture was;
	Texture2D tex;
	float time1;
	float time2;

	const int FRAME_WIDTH = 1024;
	const int FRAME_HEIGHT = 768;
	int servoPosition = 90;
	Mat nonZeroCoordinates;
	Mat Camera_Matrix;
	Mat Distortion_Coefficients;
	MCvScalar avgPixelIntensity;
	int diff = 0;
	/*
	 * 5
	 * 215
	 * 171
	 * 307
	 * 139
	 * 282
	 * 
	 * 
	 */


	public int H_MIN = 0;
	public int H_MAX = 44;
	public int S_MIN = 233;
	public int S_MAX = 262;
	public int V_MIN = 139;
	public int V_MAX = 282;

	/*
	public int H_MIN = 38;
	public int H_MAX = 88;
	public int S_MIN = 152;
	public int S_MAX = 252;
	public int V_MIN = 113;
	public int V_MAX = 255;
	*/

	// Use this for initialization
	static Texture2D textureI2TC3;
	static byte[] dataI2TC3;
	static Texture2D textureI2TC4;
	static byte[] dataI2TC4;
	void Start () {
		//sp = new SerialPort(spName, 9600, Parity.None, 8, StopBits.One);
		//OpenConnection ();
		AVProLiveCameraManager.Instance.GetDevice(deviceName).Start(-1);    
		resImage = new Image<Bgr, Byte> (FRAME_WIDTH, FRAME_HEIGHT);
		mr = GetComponent<MeshRenderer> ();
		tex = new Texture2D (FRAME_WIDTH, FRAME_HEIGHT);
		nonZeroCoordinates = new Mat();
		double[] camera_Matrix = new double[9]{5.4432826135870630e+002, 0.0, 5.1150000000000000e+002, 0.0,
			5.4432826135870630e+002, 3.8350000000000000e+002, 0.0, 0.0, 1.0};
		Camera_Matrix = new Mat(3, 3, DepthType.Cv64F, 1);
		Camera_Matrix.SetTo(camera_Matrix);

		double[] distortion_Coefficients = new double[5]{-4.0582850454021074e-001, 2.0214084200881555e-001, 0.0, 0.0,
			-5.3089969982021680e-002};
		Distortion_Coefficients = new Mat(5, 1, DepthType.Cv64F, 1);
		Distortion_Coefficients.SetTo(distortion_Coefficients);
		textureI2TC3 = new Texture2D(FRAME_WIDTH, FRAME_HEIGHT, TextureFormat.RGB24, false);
		dataI2TC3 = new byte[FRAME_WIDTH * FRAME_HEIGHT * 3];
		textureI2TC4 = new Texture2D(FRAME_WIDTH, FRAME_HEIGHT, TextureFormat.RGBA32, false);
		dataI2TC4 = new byte[FRAME_WIDTH * FRAME_HEIGHT * 4];
		//StartCoroutine ("SendDiff");
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
		oriImage = Texture2dToImage<Bgr, byte> (tex, true);

		CvInvoke.Undistort (oriImage, resImage, Camera_Matrix, Distortion_Coefficients);
	//	mr.material.mainTexture = (Texture)ImageToTexture2D(resImage, true);    
		Image<Hsv, Byte> hsv_image = oriImage.Convert<Hsv, Byte>();

		// Change the HSV value here
		Hsv hsvmin = new Hsv(H_MIN, S_MIN, V_MIN);
		Hsv hsvmax = new Hsv(H_MAX, S_MAX, V_MAX);

        hsv_image = hsv_image.SmoothGaussian(5, 5, 0.1, 0.1);

        Image<Gray, byte> red_object = hsv_image.InRange(hsvmin, hsvmax);

        red_object = red_object.Erode(1);
        red_object = red_object.Dilate(1);

		CvInvoke.FindNonZero (red_object, nonZeroCoordinates);
		avgPixelIntensity = CvInvoke.Mean(nonZeroCoordinates);
	//	Debug.Log (avgPixelIntensity.V1);
		CvInvoke.Imshow("right image", red_object); //Show the image
  //    CvInvoke.WaitKey(30);
		//System.GC.Collect();
		//DestroyObject (mr.material.mainTexture);
		diff = 0;
		if (nonZeroCoordinates.Rows > 1000) {
			diff = -(int)(avgPixelIntensity.V1 - (double)(FRAME_HEIGHT / 2));
			ArduinoSerialHandler.diff_right = diff;
		}
    }

	/*IEnumerator SendDiff()
	{
		while (true) {
			yield return new WaitForSeconds (0.25f);
			diff = 0;
			if (nonZeroCoordinates.Rows > 1000)
				diff = -(int)(avgPixelIntensity.V1 - (double)(FRAME_HEIGHT / 2));
//			Debug.Log (diff);
			sp.Write (diff + "");
		}
	}*/
		
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

	public static Texture2D ImageToTexture2D<TColor, TDepth>(Image<TColor, TDepth> image, bool correctForVerticleFlip = true)
		where TColor : struct, IColor
		where TDepth : new()
	{
		Size size = image.Size;

		if (typeof(TColor) == typeof(Rgb) && typeof(TDepth) == typeof(Byte))
		{
			
			GCHandle dataHandle = GCHandle.Alloc(dataI2TC3, GCHandleType.Pinned);
			using (Image<Rgb, byte> rgb = new Image<Rgb, byte>(size.Width, size.Height, size.Width * 3, dataHandle.AddrOfPinnedObject()))
			{
				rgb.ConvertFrom(image);
				if (correctForVerticleFlip)
					CvInvoke.Flip(rgb, rgb, Emgu.CV.CvEnum.FlipType.Vertical);
			}
			dataHandle.Free();
			textureI2TC3.LoadRawTextureData(dataI2TC3);
			textureI2TC3.Apply();
			return textureI2TC3;
		}
		else //if (typeof(TColor) == typeof(Rgba) && typeof(TDepth) == typeof(Byte))
		{
			
			GCHandle dataHandle = GCHandle.Alloc(dataI2TC4, GCHandleType.Pinned);
			using (Image<Rgba, byte> rgba = new Image<Rgba, byte>(size.Width, size.Height, size.Width * 4, dataHandle.AddrOfPinnedObject()))
			{
				rgba.ConvertFrom(image);
				if (correctForVerticleFlip)
					CvInvoke.Flip(rgba, rgba, Emgu.CV.CvEnum.FlipType.Vertical);
			}
			dataHandle.Free();
			textureI2TC4.LoadRawTextureData(dataI2TC4);
			textureI2TC4.Apply();
			return textureI2TC4;
		}

		//return null;
	}




    /*public void OpenConnection()
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
	}*/

	void OnApplicationQuit() 
	{
		AVProLiveCameraManager.Instance.GetDevice (deviceName).Close ();
		device.Close ();
		//sp.Close();
	}
}