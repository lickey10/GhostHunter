using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Proximity : MonoBehaviour {
	public Text ProximityValue;
	public Text errorValue;
	
	private ProximitySensorHelperClass proximitySensorHelperClass;
	static string sensorsString = "";
	static string errorString = "";
	
	// Use this for initialization
	void Start () {
		proximitySensorHelperClass = ProximitySensorHelperClass.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		try {
			ProximityValue.text = proximitySensorHelperClass.GetProximityIntensityValue().ToString();
		} catch (System.Exception ex) {
			errorValue.text = ex.Message;
		}
	}
}
