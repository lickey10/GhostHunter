using UnityEngine;
using System.Collections;

public class main : MonoBehaviour {


	//varied temperature detection
	//display light changes
	//display proximity indicator
	//save DetectedGhost object to file
	//record audio for later playback
	//create intuitive UI for basic functions
	//check am/fm if possible

	//motion sensing camera mode
	//display geo map of recent hits - relative to current position
	//add slider for sensitivity setting
	//add slider for all other int settings
	//add checkbox for booleans
	//what environment sensors are available
	//have something to annoy ghosts so you can get there attention so they will respond
	//rss ghost feed reader
	//analyze audio for ghost sounds
	//make phones communicate to each other or at least a main controller phone

	//--------- DONE -----------
	//emf detection
	//detect fluctuations in emf over time
	//create basic algorithm to detect an emf "hit"
	//get geolocation of emf hit
	//vibrate phone on emf hit
	//take burst of pictures on emf hit
	//make all settings configurable
	//get timestamp of emf hit



	float magneticHeading = 0;
	float trueHeading = 0;
	float currentTrueHeading = 0;
	Vector3 rawVector;
	Vector3 hitLocation;
	double hitTimeStamp;
	double currentHitTimeStamp;
	bool firstRun = true;
	int consecutiveHits = 0;//number of hits that happen within a certain timeframe - 5 seconds

	public TextMesh TheText;
	public Canvas TheCanvas;
	public int GhostDiscoveryThreshold = 20;//the threshold for changes in trueHeading readings before it's considered a hit
	public int ConsecutiveHitsThreshold = 5;//number of hits before it is considered a "big" hit
	public int ConsecutiveHitsTimeFrame = 5;//number of seconds between hits for hits to be "consecutive"
	public bool AutomaticPictureTaking = false; //whether or not the app will automatically take pictures
	public int AutomaticPictureNumber = 5; //the number of pictures to take if AutomaticPictureTaking is on and triggered
	public bool VibrateHandset = true;//whether the handset will vibrate on a big hit
	public bool CheckTemperature = true;//whether or not to watch for temperature changes

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

		TheText.text = "MagneticHeading:"+ magneticHeading + "\n";
		TheText.text += "TrueHeading:"+ trueHeading + "\n";
		TheText.text += "RawVector:"+ rawVector.x +","+ rawVector.y +","+ rawVector.z;

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
			else if(CheckTemperature)//check for temperature changes
			{

			}
			else//no field fluctuations detected so we will look for other signs
				detectEntities();
		}
		else
			firstRun = false;

		currentTrueHeading = trueHeading;
		currentHitTimeStamp = hitTimeStamp;

		TheCanvas.enabled = foundGhost;

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

	void recordAudio()
	{

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
