using UnityEngine;
using System.Collections;

public class ProximitySensorHelperClass : MonoBehaviour {

	private static ProximitySensorHelperClass _instance;
	string theError = "";
	string theError2 = "";
	public AndroidJavaObject ProximitySensor;
	private AndroidJavaObject playerActivityContext = null;

	public static ProximitySensorHelperClass Instance
	{
		get
		{
			if(_instance == null)
				_instance = new ProximitySensorHelperClass();
			
			return _instance;
		}
	}
	
	public ProximitySensorHelperClass()
	{
		ProximitySensor = new AndroidJavaObject ("com.snicklefritz.sensorplugin.ProximitySensor");

		// First, obtain the current activity context
		using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
		}

		ProximitySensor.Call("SetCurrentContext",playerActivityContext);
	}
	
	public float GetProximityIntensityValue()
	{
		return ProximitySensor.Call<float>("GetProximityValue");
	}
	
	void OnApplicationQuit()
	{
		_instance = null;
	}
}
