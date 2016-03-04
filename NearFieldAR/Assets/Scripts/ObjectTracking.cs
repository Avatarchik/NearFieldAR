using Emgu.CV.CvEnum;
using UnityEngine;
using System;
using System.Threading;
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

public class ObjectTracking : MonoBehaviour {
	public string spName = "COM2";
	private static SerialPort sp;
	public string deviceNameL = "UI325xLE-C_4102826019";
	public string deviceNameR = "UI325xLE-C_4102826019";
	AVProLiveCameraDevice deviceL;
	AVProLiveCameraDevice deviceR;
	public GameObject planeL;
	public GameObject planeR;
	MeshRenderer mrL;
	MeshRenderer mrR;
	const int FRAME_WIDTH = 1024;
	const int FRAME_HEIGHT = 768;
	Image<Bgr, Byte> oriImageL;
	Image<Bgr, Byte> oriImageR;
	Image<Bgr, Byte> resImageL;
	Image<Bgr, Byte> resImageR;
	Texture2D texL;
	Texture2D texR;
	Mat Camera_Matrix;
	Mat Distortion_Coefficients;
	Thread LeftThread;
	[SerializeField]
	private int H_MIN = 0;
	[SerializeField]
	private int H_MAX = 50;
	[SerializeField]
	private int S_MIN = 219;
	[SerializeField]
	private int S_MAX = 255;
	[SerializeField]
	private int V_MIN = 198;
	[SerializeField]
	private int V_MAX = 255;
	// Use this for initialization
	void Start () {
		AVProLiveCameraManager.Instance.GetDevice(deviceNameL).Start(-1);   
		AVProLiveCameraManager.Instance.GetDevice(deviceNameR).Start(-1); 
		mrL = planeL.GetComponent<MeshRenderer> ();
		mrR = planeR.GetComponent<MeshRenderer> ();
		oriImageL = new Image<Bgr, Byte> (FRAME_WIDTH, FRAME_HEIGHT);
		oriImageR = new Image<Bgr, Byte> (FRAME_WIDTH, FRAME_HEIGHT);
		resImageL = new Image<Bgr, Byte> (FRAME_WIDTH, FRAME_HEIGHT);
		resImageR = new Image<Bgr, Byte> (FRAME_WIDTH, FRAME_HEIGHT);
		texL = new Texture2D (FRAME_WIDTH, FRAME_HEIGHT);
		texR = new Texture2D (FRAME_WIDTH, FRAME_HEIGHT);
		double[] camera_Matrix = new double[9]{5.4432826135870630e+002, 0.0, 5.1150000000000000e+002, 0.0,
			5.4432826135870630e+002, 3.8350000000000000e+002, 0.0, 0.0, 1.0};
		Camera_Matrix = new Mat(3, 3, DepthType.Cv64F, 1);
		Camera_Matrix.SetTo(camera_Matrix);
		
		double[] distortion_Coefficients = new double[5]{-4.0582850454021074e-001, 2.0214084200881555e-001, 0.0, 0.0,
			-5.3089969982021680e-002};
		Distortion_Coefficients = new Mat(5, 1, DepthType.Cv64F, 1);
		Distortion_Coefficients.SetTo(distortion_Coefficients);
		new Thread (UpdateCameraL).Start();
	}
	private void Update(){
	//	LeftThread.Join ();
	}
	// Update is called once per frame
/*	private void Update(){
		deviceL = AVProLiveCameraManager.Instance.GetDevice(deviceNameL);
	//	deviceR = AVProLiveCameraManager.Instance.GetDevice(deviceNameR);
		deviceL.Update(false);
	//	deviceR.Update(false);
		mrL.material.mainTexture = AVProLiveCameraManager.Instance.GetDevice(deviceNameL).OutputTexture;
	//	mrR.material.mainTexture = AVProLiveCameraManager.Instance.GetDevice(deviceNameR).OutputTexture;

		RenderTexture.active = (RenderTexture)mrL.material.mainTexture;
		texL.ReadPixels (new UnityEngine.Rect(0, 0, FRAME_WIDTH, FRAME_HEIGHT), 0, 0);
		oriImageL = Texture2dToImage<Bgr, byte> (texL, true);


		//RenderTexture.active = (RenderTexture)mrR.material.mainTexture;
		//texR.ReadPixels (new UnityEngine.Rect(0, 0, FRAME_WIDTH, FRAME_HEIGHT), 0, 0);
		//oriImageR = Texture2dToImage<Bgr, byte> (texR, true);


		CvInvoke.Undistort (oriImageL, resImageL, Camera_Matrix, Distortion_Coefficients);
	//	CvInvoke.Undistort (oriImageR, resImageR, Camera_Matrix, Distortion_Coefficients);
		CvInvoke.Imshow("Left image", resImageL); //Show the image
	//	CvInvoke.Imshow("Right image", resImageR); //Show the image
	}*/
	private void UpdateCameraL(){

		while (true) {
			Debug.Log ("KAD");
			deviceL = AVProLiveCameraManager.Instance.GetDevice (deviceNameL);
			deviceL.Update (false);
			mrL.material.mainTexture = AVProLiveCameraManager.Instance.GetDevice (deviceNameL).OutputTexture;
			RenderTexture.active = (RenderTexture)mrL.material.mainTexture;
			texL.ReadPixels (new UnityEngine.Rect (0, 0, FRAME_WIDTH, FRAME_HEIGHT), 0, 0);
			oriImageL = Texture2dToImage<Bgr, byte> (texL, true);
			CvInvoke.Undistort (oriImageL, resImageL, Camera_Matrix, Distortion_Coefficients);
			CvInvoke.Imshow ("Left image", resImageL); //Show the image
		}
	}
/*	private int _lastFrameCount;
	void OnRenderObject()
	{
		if (_lastFrameCount != Time.frameCount)
		{
			_lastFrameCount = Time.frameCount;
			
			UpdateCameras();
		}
	}*/

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

	void OnApplicationQuit() 
	{
		AVProLiveCameraManager.Instance.GetDevice (deviceNameL).Close ();
		AVProLiveCameraManager.Instance.GetDevice (deviceNameR).Close ();
		deviceL.Close ();
		deviceR.Close ();
//		sp.Close();
	}
}
