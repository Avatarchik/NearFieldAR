using UnityEngine;
using System.Collections;

using System.Threading;

public class ArduinoSerialHandlerRight : ArduinoSerialHandlerBase {

	//public string spName = "COM2";

	// Use this for initialization

	//public static int value = 0;
	int id = 1;

	public ArduinoSerialHandlerRight() 
	{

		spName = "COM9";
		thread = new Thread (() => StartConnection(id));
		thread.Start ();

		sp.WriteTimeout = write_timeout;
		sp.ReadTimeout = read_timeout;
		Debug.Log ("Started Right Serial");
	}


	public void close() 
	{
		
	}

	~ArduinoSerialHandlerRight()
	{
		Debug.Log ("DIE");
		//die = true;
		thread.Abort ();

		sp.Close();
		die = true;
	}

}
