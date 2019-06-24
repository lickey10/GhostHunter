using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EMF : MonoBehaviour {
	float magneticHeading = 0;
	float trueHeading = 0;
	float currentTrueHeading = 0;
	Vector3 rawVector;
	Vector3 hitLocation;
	double hitTimeStamp;
	double currentHitTimeStamp;
	bool firstRun = true;
	int consecutiveHits = 0;//number of hits that happen within a certain timeframe - 5 seconds
	
	public Image BackgroundImage;
	public Text TrueHeadingValue;
	public Text MagneticHeadingValue;
	public Text RawVectorValueX;
	public Text RawVectorValueY;
	public Text RawVectorValueZ;
	public Text GeoLocationValue;
	public int GhostDiscoveryThreshold = 15;//the threshold for changes in trueHeading readings before it's considered a hit
	public int ConsecutiveHitsThreshold = 5;//number of hits before it is considered a "big" hit
	public int ConsecutiveHitsTimeFrame = 5;//number of seconds between hits for hits to be "consecutive"
	public bool AutomaticPictureTaking = false; //whether or not the app will automatically take pictures
	public int AutomaticPictureNumber = 5; //the number of pictures to take if AutomaticPictureTaking is on and triggered
	public bool VibrateHandset = true;//whether the handset will vibrate on a big hit
	
	// Use this for initialization
	void Start () {
		Input.location.Start();
		Input.compass.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		magneticHeading = Input.compass.magneticHeading;
		trueHeading = Input.compass.trueHeading;
		rawVector = Input.compass.rawVector;
		hitTimeStamp = Input.compass.timestamp;

		TrueHeadingValue.text = trueHeading.ToString();
		MagneticHeadingValue.text = magneticHeading.ToString();
		RawVectorValueX.text = rawVector.x.ToString();
		RawVectorValueY.text = rawVector.y.ToString();
		RawVectorValueZ.text = rawVector.z.ToString();
		
		checkForGhosts();
	}

	bool checkForGhosts()
	{
		bool foundGhost = false;
		
		if(!firstRun)
		{
			if(Mathf.Abs(currentTrueHeading - trueHeading) > GhostDiscoveryThreshold)//magnetic field fluctuation detected
			{
				foundGhost = true;
				
				if(hitTimeStamp - Input.compass.timestamp < ConsecutiveHitsTimeFrame)//make sure we are within a given timeframe
					consecutiveHits++;
				else
				{
					if(consecutiveHits >= ConsecutiveHitsThreshold)//this is a "big" hit
						saveHit();
					
					consecutiveHits = 0;
				}
			}
			else//no field fluctuations detected so we will look for other signs
				detectEntities();
		}
		else
			firstRun = false;
		
		currentTrueHeading = trueHeading;
		currentHitTimeStamp = hitTimeStamp;

		if(foundGhost)
			BackgroundImage.color = Color.red;
		else
			BackgroundImage.color = Color.blue;
		
		return foundGhost;
	}
	
	void saveHit()
	{
		//save this one
		saveGhost();
		
		if(VibrateHandset)//vibrate the handset
			Handheld.Vibrate();
		
		if(AutomaticPictureTaking)//take pictures
		{
			webcamClass theCamera = new webcamClass();
			
			for(int x = 0; x < AutomaticPictureNumber; x++)
				theCamera.TakeSnapshot();
			
			theCamera = null;
		}
	}
	
	void saveGhost()
	{
		DetectedGhost detectedGhost = new DetectedGhost();
		detectedGhost.ConsecutiveHits = consecutiveHits;
		detectedGhost.Location = hitLocation;
		detectedGhost.MagneticHeading = magneticHeading;
		detectedGhost.RawVector = rawVector;
		detectedGhost.TrueHeading = trueHeading;
		detectedGhost.TimeStamp = hitTimeStamp;
		
		detectedGhost.Save();
	}

	/// <summary>
	/// Detect entities using other signs
	/// </summary>
	bool detectEntities()
	{
		bool entityDetected = false;
		
		
		return entityDetected;
	}
}
