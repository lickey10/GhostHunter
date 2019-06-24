using UnityEngine;
using System.Collections;

public class DetectedGhost : MonoBehaviour {

	Vector3 location;
	float magneticHeading = 0;
	float trueHeading = 0;
	Vector3 rawVector;
	double timeStamp;
	int consecutiveHits = 0;
	DetectionSources detectionSource;

	public enum DetectionSources
	{
		EMF,Temperature,CameraClass
	}
	
	public Vector3 Location
	{
		get { return location; }
		set { location = value; }
	}

	public float MagneticHeading
	{
		get { return magneticHeading; }
		set { magneticHeading = value; }
	}

	public float TrueHeading
	{
		get { return trueHeading; }
		set { trueHeading = value; }
	}

	public Vector3 RawVector
	{
		get { return rawVector; }
		set { rawVector = value; }
	}

	public double TimeStamp
	{
		get { return timeStamp; }
		set { timeStamp = value; }
	}

	public int ConsecutiveHits
	{
		get { return consecutiveHits; }
		set { consecutiveHits = value; }
	}

	public DetectionSources DetectionSource
	{
		get { return detectionSource; }
		set { detectionSource = value; }
	}

	public DetectedGhost()
	{

	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Save this instance.
	/// </summary>
	public void Save()
	{
		
	}
}
