#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include "opencv2/opencv.hpp"
#include <time.h> // to calculate time needed
#include <limits.h> // to get INT_MAX, to protect against overflow
#include <Windows.h>
#include <thread>
#define FRAME_WIDTH 800
#define FRAME_HEIGHT 600
using namespace cv;
using namespace std;
int erosion_elem = 0;
int erosion_size = 1;
int dilation_elem = 0;
int dilation_size = 1;
int diff = 0;
//int servoPosition = 90;
char outputChars[] = "";
DWORD btsIO;
HANDLE hSerial;
Scalar avgPixelIntensity;
Mat nonZeroCoordinates;
void SendAngle()
{
	while (1){
		//ROTATE_DEGREE = (int)((avgPixelIntensity.val[1] - (double)(FRAME_HEIGHT / 2)) / 70.0f * 10.0f);
		diff = 0;
		if (nonZeroCoordinates.rows > 10000)
			diff = (int)(avgPixelIntensity.val[1] - (double)(FRAME_HEIGHT / 2));
		//	ROTATE_DEGREE = ROTATE_DEGREE * -1;
		cout << diff << endl;
		sprintf(outputChars, "%d", diff);
	//	cout << outputChars << endl;
		// Check whether camera should turn to its left if the circle gets near the right end of the screen
		WriteFile(hSerial, outputChars, strlen(outputChars), &btsIO, NULL);
		/*servoPosition += ROTATE_DEGREE;

		if (servoPosition > 180)
			servoPosition = 180;

		if (servoPosition < 0)
			servoPosition = 0;
			*/
		Sleep(120);
	}

}



int main(int, char**)
{
	// Setup serial port connection and needed variables.
	hSerial = CreateFile("COM4", GENERIC_READ | GENERIC_WRITE, 0, 0, OPEN_EXISTING, 0, 0);

	if (hSerial != INVALID_HANDLE_VALUE)
	{
		printf("Port opened! \n");

		DCB dcbSerialParams;
		GetCommState(hSerial, &dcbSerialParams);

		dcbSerialParams.BaudRate = CBR_9600;
		dcbSerialParams.ByteSize = 8;
		dcbSerialParams.Parity = NOPARITY;
		dcbSerialParams.StopBits = ONESTOPBIT;

		SetCommState(hSerial, &dcbSerialParams);
	}
	else
	{
		if (GetLastError() == ERROR_FILE_NOT_FOUND)
		{
			printf("Serial port doesn't exist! \n");
		}

		printf("Error while setting up serial port! \n");
	}
	

	
	VideoCapture cap(0); // open the default camera
	cap.set(CV_CAP_PROP_FRAME_WIDTH, FRAME_WIDTH);
	cap.set(CV_CAP_PROP_FRAME_HEIGHT, FRAME_HEIGHT);
	if (!cap.isOpened())  // check if we succeeded
		return -1;

	Mat edges;
	Mat imgHSV;
	Mat imgResult(FRAME_WIDTH, FRAME_HEIGHT, CV_8UC1);
	namedWindow("frame", 1);
	time_t start, end;
	int counter = 0;
	double sec;
	double fps;
	int Number_Of_Elements;
	thread mThread(SendAngle);

	// fps counter end
	for (;;)
	{
		if (counter == 0){
			time(&start);
		}
		Mat frame;
		cap >> frame; // get a new frame from camera
		// fps counter begin
		time(&end);
		counter++;
		sec = difftime(end, start);
		fps = counter / sec;
		if (counter > 30)
			//cout << fps << endl;
		// overflow protection

		if (counter == (INT_MAX - 1000))
			counter = 0;
		cvtColor(frame, imgHSV, CV_BGR2HSV);
		inRange(imgHSV, Scalar(100, 150, 150), Scalar(200, 225, 225), imgResult);
		GaussianBlur(imgResult, imgResult, Size(5, 5), 0.1, 0.1);
		Mat element = getStructuringElement(MORPH_RECT,
			Size(2 * erosion_size + 1, 2 * erosion_size + 1),
			Point(erosion_size, erosion_size));
		/// Apply the erosion operation
		erode(imgResult, imgResult, element);
		element = getStructuringElement(MORPH_RECT,
			Size(2 * dilation_size + 1, 2 * dilation_size + 1),
			Point(dilation_size, dilation_size));
		/// Apply the dilation operation
		dilate(imgResult, imgResult, element);
		
		findNonZero(imgResult, nonZeroCoordinates);
		//cout << "Non-Zero Locations = " << nonZeroCoordinates << endl << endl;
	    avgPixelIntensity = cv::mean(nonZeroCoordinates);

	//	cout << "X = " << avgPixelIntensity.val[0] << " Y = " << avgPixelIntensity.val[1] << endl;

		imshow("imgResult", imgResult);
		imshow("frame", frame);
		if (waitKey(30) >= 0) break;
	}
	mThread.join();
	CloseHandle(hSerial);
	// the camera will be deinitialized automatically in VideoCapture destructor
	return 0;
}

