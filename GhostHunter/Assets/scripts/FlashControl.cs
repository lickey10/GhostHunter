using UnityEngine;
using System.Collections;

public class FlashControl : MonoBehaviour {

	public GameObject MyCube;

	private bool isLightOn = false;
	private FlashControlsHelperClass flashControlHelperClass;
	
	void Start()
	{
		flashControlHelperClass = FlashControlsHelperClass.Instance;

		string checkFlashControl = "";
	}
	
	void Update()
	{
		if(Input.GetMouseButton(0))
		{
			if(MyCube != null)
			{
				MyCube.GetComponent<Renderer>().material.color = Color.red;

				ToggleLight();
			}


			
//			try 
//			{
//				#if UNITY_ANDROID
//				flashControlHelperClass.ToggleLight();
//				#endif
//				
//			} 
//			catch (System.Exception ex) 
//			{
//				Debug.Log(ex.Message);
//				string errorMessage = ex.Message;
//			}
		}
		else
			if(MyCube != null)
				MyCube.GetComponent<Renderer>().material.color = Color.blue; //C#
	}

	public void ToggleLight()
	{
		Debug.Log("In Toggle light");
//		if(MyCube != null)
//			MyCube.GetComponent<Renderer>().material.color = Color.red; //C#
//		
		try 
		{
			#if UNITY_ANDROID
			//flashControlHelperClass = FlashControlsHelperClass.Instance;
			flashControlHelperClass.ToggleLight();
			#endif
			
		} 
		catch (System.Exception ex) 
		{
			Debug.LogException(ex);
		}
	}
}
