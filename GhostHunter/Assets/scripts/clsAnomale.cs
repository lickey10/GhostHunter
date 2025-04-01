using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clsAnomale : MonoBehaviour
{
    Vector3 location;
    double timeStamp;
    DetectionSources detectionSource;

    public enum DetectionSources
    {
        EMF, Temperature, CameraClass, Proximity
    }

    public Vector3 Location
    {
        get { return location; }
        set { location = value; }
    }

    public List<clsEMF> EMF
    {
        get { return null; }
        set {  }
    }

    public double TimeStamp
    {
        get { return timeStamp; }
        set { timeStamp = value; }
    }

    //public int ConsecutiveHits
    //{
    //    get { return consecutiveHits; }
    //    set { consecutiveHits = value; }
    //}

    public DetectionSources DetectionSource
    {
        get { return detectionSource; }
        set { detectionSource = value; }
    }

    public clsAnomale()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Save this instance.
    /// </summary>
    //public void Save()
    //{

    //}

}

public class clsAnomales : MonoBehaviour
{
    List<clsAnomale> anomales = new List<clsAnomale>();

    public List<clsAnomale> Anomales 
    { 
        get { return anomales; } 
        set { anomales = value; }
    }

    public clsAnomales()
    {
        string json = PlayerPrefs.GetString("Anomales", null);

        if (string.IsNullOrEmpty(json) == true)
            anomales = new List<clsAnomale>();
        else
            anomales = JsonUtility.FromJson<List<clsAnomale>>(json);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(anomales);
        PlayerPrefs.SetString("Anomales", json);
    }

    public void Delete()
    {
        PlayerPrefs.SetString("Anomales", null);

        anomales = new List<clsAnomale>();
    }
}
