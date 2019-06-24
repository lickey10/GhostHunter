using UnityEngine;
using System.Collections;

public class GravitySensorHelperClass : MonoBehaviour {

	private static GravitySensorHelperClass _instance;
	string theError = "";
	string theError2 = "";
	public AndroidJavaObject GravitySensor;
	private AndroidJavaObject playerActivityContext = null;

	public static GravitySensorHelperClass Instance
	{
		get
		{
			if(_instance == null)
				_instance = new GravitySensorHelperClass();
			
			return _instance;
		}
	}
	
	public GravitySensorHelperClass()
	{
		GravitySensor = new AndroidJavaObject ("com.snicklefritz.sensorplugin.GravitySensor");

		// First, obtain the current activity context
		using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
		}

		GravitySensor.Call("SetCurrentContext",playerActivityContext);
	}
	
	public float GetGravityIntensityValue()
	{
		return GravitySensor.Call<float>("GetGravityValue");
	}
	
	void OnApplicationQuit()
	{
		_instance = null;
	}
}
