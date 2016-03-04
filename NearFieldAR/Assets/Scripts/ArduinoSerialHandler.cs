using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Ports;

using System.Threading;
using System.Collections;



public class ArduinoSerialHandler : MonoBehaviour {


	public string spName = "COM4";
	private static SerialPort sp;

	/*
	 * Write these values from object tracking classes 
	 */
	public static int diff_right = 0;
	public static int diff_left = 0;

	public int flush_counter = 0;

	private string data_send;

	private Thread thread;

	private bool die = false;

	void Start () 
	{
		thread = new Thread (StartConnection);
		thread.Start ();
		//StartConnection ();
		sp.WriteTimeout = 50;
		sp.ReadTimeout = 50;
	}

	public void StartConnection()
	{
		Debug.Log ("Starting Arduino connection");
		sp = new SerialPort(spName, 9600, Parity.None, 8, StopBits.One);
		OpenConnection ();

		while (true) 
		{
			if (die)
				return;

			Thread.Sleep (250);
			diff_right = diff_right * -1;
			data_send = diff_left.ToString () + "," + diff_right.ToString () + '\n';

			//data_send = diff_right.ToString();
			Debug.Log (data_send);
			//Debug.Log ("Data to send: " + data_send);

			//sp.Write (data_send);

			sp.Write (data_send);
			//sp.BaseStream.Flush ();


			

			diff_left = 0;
			diff_right = 0;

			flush_counter++;
			if (flush_counter > 20) {
				sp.BaseStream.Flush ();
				flush_counter = 0;
			}
		}

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
		die = true;
		thread.Abort ();
		sp.Close();
	}



}
