using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightDetection : MonoBehaviour {
	public Text LightValue;
	public Text errorValue;

	private LightSensorHelperClass lightSensorHelperClass;
	static string sensorsString = "";
	static string errorString = "";

	// Use this for initialization
	void Start () {
		lightSensorHelperClass = LightSensorHelperClass.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		try {
			LightValue.text = lightSensorHelperClass.GetLightIntensityValue().ToString();
		} catch (System.Exception ex) {
			errorValue.text = ex.Message;
		}
	}
}

