using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Ports;

using System.Threading;
using System.Collections;

using System;

using System.Collections.Generic;

public class ArduinoSerialHandlerBase {


	public string spName;
	protected SerialPort sp;

	protected int read_timeout = 50;
	protected int write_timeout = 50;

	/*
	 * Write these va/es from object tracking classes 
	 */
	//public static int diff_right = 0;
	//public static int diff_left = 0;

	public int flush_counter = 0;

	private string data_send;

	protected Thread thread;

	public  bool die = false;

	//private static int value = 0;
	public static int value0 = 0;
	public static int value1 = 0;

	//static Dictionary<Type, int> value;
	int value = 0;
	/*void Start () 
	{
		thread = new Thread (StartConnection);
		thread.Start ();
		//StartConnection ();
		sp.WriteTimeout = 50;
		sp.ReadTimeout = 50;
	}*/

	public ArduinoSerialHandlerBase()
	{
	}

	public void set_value(int input)
	{
		value = input;
	}



	public void StartConnection(int id)
	{
		Debug.Log ("Starting Arduino connection at: " + spName);
		Debug.Log ("ID: " + id.ToString ());
		sp = new SerialPort(spName, 9600, Parity.None, 8, StopBits.One);
		OpenConnection ();

		while (true) 
		{
			if (die)
				break;

			Thread.Sleep (350);
			//diff_right = diff_right * -1;
			//data_send = diff_left.ToString () + "," + diff_right.ToString () + '\n';

			//data_send = diff_right.ToString();
		//	Debug.Log (data_send);

			if (id == 0)
				data_send = value0.ToString ();
			else
				data_send = value1.ToString ();

			Debug.Log ("Data to send: " + data_send);

			//sp.Write (data_send);

			sp.Write (data_send);
			//sp.BaseStream.Flush ();

			//value0 = 0;
			//value1 = 0;
			

			//diff_left = 0;
			//diff_right = 0;

			flush_counter++;
			if (flush_counter > 20) {
				sp.BaseStream.Flush ();
				flush_counter = 0;
			}
		}

	}


	public void OpenConnection()
	{
		Debug.Log("looking for port");
		if (sp != null) {
			if (sp.IsOpen) {
				sp.Close ();
				Debug.Log ("Closing port, because it was already open");
			} else {
				sp.Open ();
				sp.ReadTimeout = 50;  // sets the timeout value before reporting error
				Debug.Log ("Port Opened!s");
			}
		} else {
			if(sp.IsOpen)
			{
				Debug.Log("Port is already open");
			}
			else
			{
				Debug.Log("Port == null");
			}
		}
	}






}
