using UnityEngine;
using System.Collections;

public class webcamClass : MonoBehaviour {
	public GameObject CameraTarget;
	string deviceName;
	WebCamTexture wct;
	private AndroidJavaObject camera;
	private bool Active;
	
	// Use this for initialization
	void Start () {
		AndroidJavaClass cameraClass = new AndroidJavaClass("android.hardware.Camera");

		WebCamDevice[] devices = WebCamTexture.devices;
		deviceName = devices[0].name;
		wct = new WebCamTexture(deviceName, 400, 300, 12);
		//GetComponent<Renderer>().material.mainTexture = wct;
		//CameraTarget = wct;

		CameraTarget.GetComponent<Renderer>().material.mainTexture = wct;

		wct.Play();



//		int camID = 0;
//		camera = cameraClass.CallStatic<AndroidJavaObject>("open",camID);
//
//		// I'm pretty sure camera will never be null at this point
//		// It will either be a valid object or Camera.open would throw an exception
//		if (camera != null)
//		{
//			AndroidJavaObject cameraParameters = camera.Call<AndroidJavaObject>("getParameters");
//			cameraParameters.Call("setFlashMode","torch");
//			camera.Call("setParameters",cameraParameters);
//			camera.Call("startPreview");
//			Active = true;
//		}
//		else
//		{
//			Debug.LogError("[CameraParametersAndroid] Camera not available");
//		}
	}

//	void OnDestroy()
//	{
//		FL_Stop();
//	}
//	
//	void FL_Stop () {
//		
//		if (camera != null)
//		{
//			camera.Call("stopPreview");
//			camera.Call("release");
//			Active = false;
//		}
//		else
//		{
//			Debug.LogError("[CameraParametersAndroid] Camera not available");
//		}
//		
//	}

	
	// For photo varibles
	public Texture2D heightmap;
	public Vector3 size = new Vector3(100, 10, 100);

	void OnGUI() {      
		//if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
		//	TakeSnapshot();

//		GUILayout.BeginArea(new Rect(Screen.width*0.1f,Screen.height*0.1f,Screen.width*0.3f,Screen.height*0.1f) );
//		if (!Active)
//		{
//			if (GUILayout.Button("ENABLE FLASHLIGHT") )
//				Start();
//		}else{
//			if (GUILayout.Button("DISABLE FLASHLIGHT") )
//				FL_Stop();
//		}
//		GUILayout.EndArea();
	}
	
	// For saving to the _savepath
	private string _SavePath = "C:/WebcamSnaps/"; //Change the path here!
	int _CaptureCounter = 0;

	public void TakeSnapshot()
	{
		Texture2D snap = new Texture2D(wct.width, wct.height);
		snap.SetPixels(wct.GetPixels());
		snap.Apply();
		
		System.IO.File.WriteAllBytes(_SavePath + _CaptureCounter.ToString() + ".png", snap.EncodeToPNG());
		++_CaptureCounter;
	}
}
