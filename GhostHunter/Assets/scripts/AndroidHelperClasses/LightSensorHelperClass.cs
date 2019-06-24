using UnityEngine;
using System.Collections;

public class LightSensorHelperClass : MonoBehaviour {

	private static LightSensorHelperClass _instance;
	string theError = "";
	string theError2 = "";
	public AndroidJavaObject LightSensor;
	private AndroidJavaObject playerActivityContext = null;

	public static LightSensorHelperClass Instance
	{
		get
		{
			if(_instance == null)
				_instance = new LightSensorHelperClass();
			
			return _instance;
		}
	}
	
	public LightSensorHelperClass()
	{
		LightSensor = new AndroidJavaObject ("com.snicklefritz.sensorplugin.LightSensor");

		// First, obtain the current activity context
		using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
		}

		LightSensor.Call("SetCurrentContext",playerActivityContext);
	}
	
	public float GetLightIntensityValue()
	{
		return LightSensor.Call<float>("GetLightValue");
	}
	
	void OnApplicationQuit()
	{
		_instance = null;
	}
}
