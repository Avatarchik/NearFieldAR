using UnityEngine;
using System.Collections;

using System.Threading;

public class ArduinoSerialHandlerLeft : ArduinoSerialHandlerBase {

	//public string spName = "COM4";

	// Use this for initialization

	//public static int value = 0;

	int id = 0;

	public ArduinoSerialHandlerLeft() 
	{
		spName = "COM9";
		Debug.Log ("Starting Left Serial");

		thread = new Thread (() => StartConnection(id));
		thread.Start ();

		sp.WriteTimeout = write_timeout;
		sp.ReadTimeout = read_timeout;
		Debug.Log ("Started Left Serial");
	
	
	}

	public void close() 
	{
		Debug.Log ("DIE");
		die = true;
		thread.Abort();
		sp.Close();
	}

}
