using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gravity : MonoBehaviour {
	public Text GravityValue;
	public Text errorValue;
	
	private GravitySensorHelperClass gravitySensorHelperClass;
	static string sensorsString = "";
	static string errorString = "";
	
	// Use this for initialization
	void Start () {
		gravitySensorHelperClass = GravitySensorHelperClass.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		try {
			GravityValue.text = gravitySensorHelperClass.GetGravityIntensityValue().ToString();
		} catch (System.Exception ex) {
			errorValue.text = ex.Message;
		}
	}
}
