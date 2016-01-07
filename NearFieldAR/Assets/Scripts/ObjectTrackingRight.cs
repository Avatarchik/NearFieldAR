using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using OpenCvSharp;
//using OpenCvSharp.CPlusPlus;
using UnityEngine.UI;
using System.IO;
using System.IO.Ports;


using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
//using System.Drawing;
//using System.Windows.Forms;
using Emgu.Util;
using Emgu.CV.Util;
using System.Drawing;


public class ObjectTrackingRight : MonoBehaviour {
	public string spName = "COM6";
	public static SerialPort sp;
	public string deviceName = "UI325xLE-C_4102832627";
	MeshRenderer mr;
	bool useMorphOps = true;
	Image<Rgb, Byte> oriImage;
	//private CvMat Gray;
	//private CvMat threshold;
	RenderTexture was;
	Texture2D tex;


    bool has_circle;


	const int FRAME_WIDTH = 640;
	const int FRAME_HEIGHT = 480;

	int servoPosition = 90;
	int servoOrientation = 0;
	// Use this for initialization
	//CvMemStorage p_strStorage;


	void Start () {
		sp = new SerialPort(spName, 9600, Parity.None, 8, StopBits.One);
		OpenConnection ();
		AVProLiveCameraManager.Instance.GetDevice(deviceName).Start(-1);    
		mr = GetComponent<MeshRenderer> ();
		//oriImage = new CvMat (FRAME_WIDTH, FRAME_HEIGHT, MatrixType.U8C3);
		//Gray = new CvMat (FRAME_HEIGHT, FRAME_WIDTH, MatrixType.U8C1);
		tex = new Texture2D (FRAME_WIDTH, FRAME_HEIGHT);
        //p_strStorage = Cv.CreateMemStorage (0);

        has_circle = false;


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

        Image<Hsv, Byte> hsv_image = oriImage.Convert<Hsv, Byte>();

        Image<Gray, Byte> grayImg;


        //grayImg = img[0].InRange(new Gray(80), new Gray(100));
        Image<Gray, Byte>[] channels = hsv_image.Split();
        Image<Gray, Byte> imghue = channels[0];            //hsv, so channels[0] is hue.
        Image<Gray, Byte> imgval = channels[2];            //hsv, so channels[2] is value.


        // Change the HSV value here
        Hsv hsvmin = new Hsv(100, 150, 150);
        Hsv hsvmax = new Hsv(200, 255, 255);

        hsv_image = hsv_image.SmoothGaussian(5, 5, 0.1, 0.1);

        Image<Gray, byte> red_object = hsv_image.InRange(hsvmin, hsvmax);

        Hsv hsvmin2 = new Hsv(300, 0, 0);
        Hsv hsvmax2 = new Hsv(365, 360, 360);
        Image<Gray, byte> red_object2 = hsv_image.InRange(hsvmin2, hsvmax2);

        //red_object = red_object + red_object2;
        //red_object = red_object2;
        //red_object = red_object.Or(red_object2);
        //red_object = red_object2;
        red_object = red_object.Erode(1);
        red_object = red_object.Dilate(1);

        
       VectorOfVectorOfPoint contoursDetected = new VectorOfVectorOfPoint();
       CvInvoke.FindContours(red_object, contoursDetected, null, Emgu.CV.CvEnum.RetrType.List, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple, default(Point));
       List<VectorOfPoint> contoursArray = new List<VectorOfPoint>();
       int count = contoursDetected.Size;

        VectorOfPoint biggest_contour = null;
        int prev_size = 0;
        for (int i = 0; i < count; i++)
        {
            using (VectorOfPoint currContour = contoursDetected[i])
            {
                if (prev_size < currContour.Size)
                {
                    prev_size = currContour.Size;

                    biggest_contour = currContour;
                }

            }
        }

        double centroid_x = 0;
        double centroid_y = 0;
        double diameter = 0;
        has_circle = false;
        if (prev_size > 50)
        {
            MCvMoments moment = CvInvoke.Moments(biggest_contour);
            centroid_x = moment.M10 / moment.M00;
            centroid_y = moment.M01 / moment.M00;

            double area = CvInvoke.ContourArea(biggest_contour);

            diameter = Math.Sqrt(4 * area / Math.PI);

            Console.WriteLine(centroid_x);

            CvInvoke.Circle(oriImage, new Point((int)centroid_x, (int)centroid_y), (int)diameter / 2, new MCvScalar(255, 0, 0), 5);
            has_circle = true;
        }

        if (!has_circle) {
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
            }
            // Run this if the camera can see at least one circle
            if (has_circle) {
                
                    Debug.Log (centroid_x);	// x position of center point of circle
                    // y position of center point of circle
                    Debug.Log (diameter);	// radius of circle
                    servoOrientation = 0;

                    // Check whether camera should turn to its left if the circle gets near the right end of the screen
                    if (centroid_y < tex.width/2) {
                        sp.Write ("l");
                        servoPosition += 1;

                        if (servoPosition > 180)
                            servoPosition = 180;
                    }

            // Check whether camera should turn to its right if the circle gets near the left end of the screen
            //if (centroid_x < 200) {
                    else { 
                        sp.Write ("r");
                        servoPosition -= 1;

                        if (servoPosition < 0)
                            servoPosition = 0;
                    }

                
            }


            CvInvoke.Imshow("right image", oriImage); //Show the image

        CvInvoke.WaitKey(30);


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

    private Image<Rgb, Byte> Texture2dToMat(Texture2D tex)
    {
        Image<Rgb, Byte> rgb_image = new Image<Rgb, Byte>(tex.width, tex.height);

        var cols = tex.GetPixels();
        for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width; j++)
            {

                UnityEngine.Color col;
                col = cols[j + i * tex.width];
                byte r = (byte)(col.r * 255);
                byte g = (byte)(col.g * 255);
                byte b = (byte)(col.b * 255);

                rgb_image.Data[i, j, 0] = b;
                rgb_image.Data[i, j, 1] = g;
                rgb_image.Data[i, j, 2] = r;

            }
        }
        return rgb_image;
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