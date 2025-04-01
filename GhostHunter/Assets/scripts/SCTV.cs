using RedCorona.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public enum StatusSeverityEnum
{
    Normal,
    Important
}

public enum StatusTypeEnum
{
    Message,
    Error,
    Debug
}

public enum SCTVState
{ playing, paused, stopped, error }

public enum MediaTypeEnum
{
    Movies,
    TV,
    All,
    Other
}

public class SCTV : MonoBehaviour
{
    public InputField resultBox;
    //public GameObject MovieList;
    //public GameObject MovieButton;
    //public SwipeMenu.MenuItem MenuItem;
    public TextAsset MediaXmlFile;
    public event EventHandler<SCTVStatusEventArgs> OnStatusUpdated;
    [HideInInspector]
    public string SctvStatus = "";
    public GameObject NoteObject;
    [HideInInspector]
    public string CurrentMediaTitle = "";
    ClientInfo client;
    public string serverIpVLC = "10.0.0.84";//my computer right now - this is the ip of the sctv server running vlc
    static string serverPortVLC = "8080";//vlc port to use - this will determine what instance of vlc/chromecast you connect to
    static string serverBaseURLVLC = @"/requests/status.xml";
    //static string serverIP1 = "chad"; //or url
    static string vlcPassword = "bob";
    //int serverPort = 2345;
    string selectedChromecastIP = "10.0.0.103";//Master Bedroom -  10.0.0.71 Living Room
    //string selectedFilePath = "D:\\DVDRips\\LOONEY TUNES BIGGEST COMPILATION  Bugs Bunny, Daffy Duck and more!.mp4";
    Texture2D img;
    SCTVState sctvState = SCTVState.stopped;
    public float StatusCheckInterval = 2f;
    int currentPlayTime = 0;
    //XElement xmlMedia = null;
    XmlDocument xDoc = new XmlDocument();
    clsAnomales Anomales = null;
    Socket listener;
    static string serverIP = "10.0.0.84"; //or url or computer name
    string currentFilePath = "";
    public GameObject VerticalListView;
    bool crRunning = false;
    bool justChangedState = false;//did the state of vlc just change
    public string ChromecastInfo = "Master Bedroom,10.0.0.103,8080|Living Room,10.0.0.71,8090"; //chromecast name, chromecast IP, vlc port
    public string SelectedChromecast = "";//this is the selected chromecast from chromecastInfo
    public GameObject MediaDetailsDisplay_Large;
    public GameObject GridviewObject;//to display the results
    public GameObject NowPlayingObject;
    public GameObject ConfigCanvas;
    public Button PlayButton;
    public Button PauseButton;
    public Button PreviousButton;
    public Button NextButton;
    public Button ForwardButton;
    public Button BackButton;
    public UnityEngine.UI.Text MediaTitle;

    // Declare our worker thread
    private Thread workerThread = null;

    public string ServerIpVLC
    {
        get { return serverIpVLC; }
        set
        {
            serverIpVLC = value;

            PlayerPrefs.SetString("serverIpVLC", value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Anomales = new clsAnomales();


        //look for other servers



        //if a server exists make this server ip = existing server IP + 1 - send hello message
        //hello message should have gps location, name, ip






        // Initialise and start worker thread
        this.workerThread = new Thread(new ThreadStart(this.StartServer));
        this.workerThread.Start();

        //SendMessage("hi");


        //VerticalListView = GameObject.FindGameObjectWithTag("VerticalListView");

        //string tempSetting = PlayerPrefs.GetString("serverIpVLC", "");

        //if (tempSetting.Trim().Length > 0)
        //{
        //    serverIpVLC = tempSetting;

        //    ConfigCanvas.GetComponentInChildren<TMP_InputField>().text = serverIpVLC;
        //}

        //tempSetting = PlayerPrefs.GetString("ChromecastInfo", "");

        //if (tempSetting.Trim().Length > 0)
        //    ChromecastInfo = tempSetting;

        //tempSetting = PlayerPrefs.GetString("selectedChromecastIP", "");

        //if (tempSetting.Trim().Length > 0)
        //    selectedChromecastIP = tempSetting;

        //tempSetting = PlayerPrefs.GetString("serverPortVLC", "");

        //if (tempSetting.Trim().Length > 0)
        //    serverPortVLC = tempSetting;

        //tempSetting = PlayerPrefs.GetString("SelectedChromecast", "");

        //if (tempSetting.Trim().Length > 0)
        //    SelectedChromecast = tempSetting;

        //make sure we have the latest media file
        //SendSCTVMessage("updateMediaFile");

        //get current vlc status
        //Status();

        //Invoke("Status", StatusCheckInterval);


    }

    // Update is called once per frame
    void Update()
    {
        //if ((sctvState == SCTVState.playing || sctvState == SCTVState.paused) && nextActionTime > 0 && Time.time > nextActionTime)
        //{
        //    nextActionTime = Time.time + StatusCheckInterval;

        //    //check status
        //    Status();
        //}

        //if (crRunning == false)
        //    Status();
        //Invoke("Status", StatusCheckInterval);
    }

    public void StartServer()
    {
        bool serverStarted = false;

        try
        {
            // Get Host IP Address that is used to establish a connection  
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
            // If a host has multiple addresses, you will get a list of addresses  
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = host.AddressList[0];
            //IPAddress ipAddress = System.Net.IPAddress.Parse("10.10.10.1");
            IPAddress ipAddress = System.Net.IPAddress.Parse("0.0.0.0");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a Socket that will use Tcp protocol      
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // A Socket must be associated with an endpoint using the Bind method  
            listener.Bind(localEndPoint);
            // Specify how many requests a Socket can listen before it gives Server busy response.  
            // We will listen 10 requests at a time  
            listener.Listen(10);

            startListening();

            serverStarted = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        //return serverStarted;
    }

    private void startListening()
    {
        //updateLblStatusThreadSafe("Waiting for a connection..." + Environment.NewLine);
        
        Console.WriteLine("Waiting for a connection...");
        Socket handler = listener.Accept();
        handler.SendTimeout = 2000;

        // Incoming data from the client.    
        string data = null;
        byte[] bytes = null;

        while (true)
        {
            bytes = new byte[1024];
            int bytesRec = handler.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

            if (data.IndexOf("<EOF>") > -1)
            {
                data = data.Replace("<EOF>", "");

                switch (data.Split('|')[0])
                {
                    case "status"://current vlc status
                                  //http://127.0.0.1:9090/requests/status.xml

                        break;
                    case "command=in_play":
                        //data will have the full file path of the file to cast from vlc and what IP to send the cast to

                        //startVLC(data.Split('|')[1], data.Split('|')[2]);
                        break;
                    case "command=pl_stop":
                        //http://127.0.0.1:9090/requests/status.xml?command=pl_stop

                        break;
                    case "pl_pause":
                        //?command=pl_pause&id=<id>

                        break;
                    case "pl_stop":
                        //?command=pl_stop

                        break;
                    case "pl_next":
                        //?command=pl_next

                        break;
                    case "pl_previous":
                        //?command=pl_previous 

                        break;
                    case "volume":
                        //?command=volume&val=<val>
                        //                         Allowed values are of the form:
                        //+< int >, -< int >, < int > or<int> %


                        break;
                    case "seek":
                        //?command = seek & val =< val >
                        //Allowed values are of the form:
                        //  [+or -][< int >< H or h >:][< int >< M or m or '>:][<int><nothing or S or s or ">]
                        //  or[+or -] < int >%
                        //  (value between[ ] are optional, value between < > are mandatory)
                        //examples:
                        //                           1000->seek to the 1000th second
                        //  +1H: 2M->seek 1 hour and 2 minutes forward
                        //  -10 % ->seek 10 % back


                        break;
                    case "in_enqueue": //add to playlist
                                       //?command=in_enqueue&input=<mrl>

                        break;
                    case "pl_empty": //empty playlist
                                     //?command=pl_empty

                        break;
                    case "updateMediaFile":
                        string version = "-1";

                        if (data.Contains("|"))
                            version = data.Split('|')[1];

                        //get current version and compare
                        //string newMedia = File.ReadAllText(Application.StartupPath + "/mediaFile/media.xml");
                        //string newVersion = newMedia.Substring(newMedia.IndexOf("<mediaFiles version=\"")).Replace("<mediaFiles version=\"", "");
                        //newVersion = newVersion.Substring(0, newVersion.IndexOf("\">"));

                        //if (newVersion == version)
                        //    data = "";
                        //else
                        //    data = newMedia;

                        break;
                }

                //sendVLCCommandsAsync();

                break;
            }

            if(data.Trim().Length > 0)
                System.Diagnostics.Debug.Write("DATA: "+ data);
        }

        //updateLblStatusThreadSafe("Text received : " + data);

        byte[] msg = Encoding.ASCII.GetBytes(data + "<EOF>");
        handler.Send(msg);
        handler.Shutdown(SocketShutdown.Both);
        handler.Close();

        startListening();//listen for the next client and data
    }

    public static void SendMessage(string message)
    {
        byte[] bytes = new byte[1024];

        try
        {
            // Connect to a Remote server  
            // Get Host IP Address that is used to establish a connection  
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
            // If a host has multiple addresses, you will get a list of addresses  
            //IPHostEntry host = Dns.GetHostEntry(serverIP);
            IPHostEntry host = Dns.GetHostEntry("0.0.0.0");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.    
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.    
            try
            {
                // Connect to Remote EndPoint  
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.    
                byte[] msg = Encoding.ASCII.GetBytes(message + "<EOF>");

                // Send the data through the socket.    
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.    
                int bytesRec = sender.Receive(bytes);
                Console.WriteLine("Echoed test = {0}",
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));

                // Release the socket.    
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());

                throw ane;
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());

                throw se;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());

                throw e;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());

            throw e;
        }
    }

    /// <summary>
    /// My hard coded test function that is working
    /// </summary>
    /// <returns></returns>
    IEnumerator SendVlcCommand()
    {
        //works from browser
        //http://127.0.0.1:8080/requests/status.xml?command=in_play&input=\\FILESERVER\Video\Movies\@-M\DEADPOOL.avi

        UnityWebRequest www = UnityWebRequest.Get(@"http://10.0.0.84:8080/requests/status.xml?command=in_play&input=\\FILESERVER\Video\Movies\@-M\DEADPOOL.avi");
        string authorization = authenticate("", "bob");
        www.SetRequestHeader("AUTHORIZATION", authorization);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (resultBox)
                resultBox.text = www.error;

            UnityEngine.Debug.Log(www.error);

            handleStatusChange(www.error, StatusTypeEnum.Error, StatusSeverityEnum.Important);
        }
        else
        {
            // Show results as text
            UnityEngine.Debug.Log(www.downloadHandler.text);

            if (resultBox)
                resultBox.text = www.downloadHandler.text;

            handleStatusChange(www.downloadHandler.text, StatusTypeEnum.Error, StatusSeverityEnum.Important);

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
        }
    }

    IEnumerator SendVlcCommand(string CommandName, string CommandValue)
    {
        //have to use api target of 27 or lower unless you have "https://" url for vlc
        //api levels after 27 require secure url's for UnityWebRequest

        UnityWebRequest www = null;
        crRunning = true;

        try
        {
            //works from browser
            //http://127.0.0.1:8080/requests/status.xml?command=in_play&input=\\FILESERVER\Video\Movies\@-M\DEADPOOL.avi

            string queryString = ""; // "?"+ CommandName +"&input="+ CommandValue;

            switch (CommandName)
            {
                case "status"://current vlc status http://127.0.0.1:9090/requests/status.xml

                    break;
                case "in_play"://http://127.0.0.1:9090/requests/status.xml?command=pl_play&input=<MRL>
                case "in_enqueue": //add to playlist http://127.0.0.1:9090/requests/status.xml?command=in_enqueue&input=<mrl>
                    queryString = "?command=" + CommandName + "&input=" + CommandValue;
                    break;
                case "pl_stop"://http://127.0.0.1:9090/requests/status.xml?command=pl_stop
                case "pl_pause"://http://127.0.0.1:9090/requests/status.xml?command=pl_pause&id=<id>
                case "pl_next"://http://127.0.0.1:9090/requests/status.xml?command=pl_next
                case "pl_previous"://http://127.0.0.1:9090/requests/status.xml?command=pl_previous 
                case "pl_empty": //empty playlist http://127.0.0.1:9090/requests/status.xml?command=pl_empty
                    queryString = "?command=" + CommandName;
                    break;

                case "volume":
                //http://127.0.0.1:9090/requests/status.xml?command=volume&val=<val>
                //                         Allowed values are of the form:
                //+< int >, -< int >, < int > or<int> %
                case "seek":
                    //http://127.0.0.1:9090/requests/status.xml?command=seek&val=<val>
                    //Allowed values are of the form:
                    //  [+or -][< int >< H or h >:][< int >< M or m or '>:][<int><nothing or S or s or ">]
                    //  or[+or -] < int >%
                    //  (value between[ ] are optional, value between < > are mandatory)
                    //examples:
                    //                           1000->seek to the 1000th second
                    //  +1H: 2M->seek 1 hour and 2 minutes forward
                    //  -10 % ->seek 10 % back

                    queryString = "?command=" + CommandName + "&val=" + CommandValue;
                    break;
            }

            if (SelectedChromecast.Trim().Length > 0 && SelectedChromecast.Contains(","))
                serverPortVLC = SelectedChromecast.Substring(SelectedChromecast.IndexOf(",") + 1);

            www = UnityWebRequest.Get("http://" + serverIpVLC + ":" + serverPortVLC + serverBaseURLVLC + queryString);
            string authorization = authenticate("", vlcPassword);
            www.SetRequestHeader("AUTHORIZATION", authorization);
        }
        catch (Exception ex)
        {
            handleStatusChange(ex.Message, StatusTypeEnum.Error, StatusSeverityEnum.Important);
        }

        yield return www.SendWebRequest();

        try
        {
            if (www.isNetworkError || www.isHttpError)
            {
                UnityEngine.Debug.Log(www.error);

                if (sctvState != SCTVState.error)//send change event
                {
                    OnStatusUpdated?.Invoke(this, new SCTVStatusEventArgs("Can't connect to SCTV", StatusTypeEnum.Error, StatusSeverityEnum.Important));

                    displayChangeOnUI(SCTVState.error);
                }

                sctvState = SCTVState.error;

                SctvStatus = www.error;

                //if (www.isNetworkError)
                //{
                //    OnStatusUpdated?.Invoke(this, new SCTVStatusEventArgs("Can't connect to SCTV", StatusTypeEnum.Error, StatusSeverityEnum.Important));

                //    //handleStatusChange("isNetworkError: " + www.error, StatusTypeEnum.Error, StatusSeverityEnum.Important);
                //}
                //else
                //{
                //    OnStatusUpdated?.Invoke(this, new SCTVStatusEventArgs("Can't connect to SCTV", StatusTypeEnum.Error, StatusSeverityEnum.Important));

                //    //handleStatusChange("isHttpError: " + www.error, StatusTypeEnum.Error, StatusSeverityEnum.Important);
                //}

                switch (SctvStatus)
                {
                    case "Cannot connect to destination host": //vlc isn't reachable - probably not started

                        break;

                }
            }
            else
            {
                // get results as text
                UnityEngine.Debug.Log(www.downloadHandler.text);

                SctvStatus = www.downloadHandler.text;

                

                //get vlc state
                string vlcState = findValue(SctvStatus, "<state>", "</state>");
                SCTVState tempState = sctvState;

                //set sctvState if value is valid
                if (SCTVState.TryParse(vlcState, out tempState))
                {
                    if (tempState != sctvState)//send change event message
                    {
                        if (!justChangedState)
                        {
                            OnStatusUpdated?.Invoke(this, new SCTVStatusEventArgs(tempState.ToString(), StatusTypeEnum.Message, StatusSeverityEnum.Normal));

                            sctvState = tempState;

                            displayChangeOnUI(sctvState);
                        }
                        else
                            justChangedState = false;
                    }
                }

                if (sctvState == SCTVState.playing || sctvState == SCTVState.paused)//check the status and make sure we are still playing
                {
                    //track the current position of file as we play
                    string timeInSeconds = findValue(SctvStatus, "<time>", "</time>");

                    int tempPlayTime = 0;

                    if (int.TryParse(timeInSeconds, out tempPlayTime) && tempPlayTime > currentPlayTime)
                    {
                        //update LastPlayPosition for this file

                        currentPlayTime = tempPlayTime;

                        if (currentFilePath.Trim().Length == 0)//this file was already playing when this application started.  Can't get the Media info
                        {
                            currentFilePath = findValue(SctvStatus, "<info name='filename'>", "</info>");//use filename as movie title
                            //CurrentMediaTitle = currentFilePath.Substring(0, CurrentMediaTitle.IndexOf("."));
                        }
                      

                    }
                }
            }

            crRunning = false;
        }
        catch (Exception ex)
        {
            throw ex;
            //handleStatusChange(ex.Message, StatusTypeEnum.Error, StatusSeverityEnum.Important);
        }
    }

    private void displayChangeOnUI(SCTVState sctvState)
    {
        MediaTitle.text = CurrentMediaTitle;

        switch (sctvState)
        {
            case SCTVState.playing:
                PlayButton.gameObject.SetActive(false);
                PauseButton.gameObject.SetActive(true);
                NowPlayingObject.SetActive(true);
                break;
            case SCTVState.stopped:
                PlayButton.gameObject.SetActive(true);
                PauseButton.gameObject.SetActive(false);
                NowPlayingObject.SetActive(false);
                break;
            case SCTVState.paused:
                PlayButton.gameObject.SetActive(true);
                PauseButton.gameObject.SetActive(false);
                NowPlayingObject.SetActive(true);
                break;
            case SCTVState.error:
                NowPlayingObject.SetActive(false);
                break;
        }

    }

    private void handleStatusChange(string message, StatusTypeEnum statusType, StatusSeverityEnum statusSeverity)
    {
        //display message appropriatly
        NoteObject.GetComponentInChildren<UnityEngine.UI.Text>().text = message;
        NoteObject.GetComponent<AutoHide>().Delay = 5000f;

        if (statusType == StatusTypeEnum.Error)
            NoteObject.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.yellow;
        else
            NoteObject.GetComponentInChildren<UnityEngine.UI.Image>().color = Color.gray;

        NoteObject.SetActive(true);


    }

    /// <summary>
    /// create authentications string for vlc web interface
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    string authenticate(string username, string password)
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

    private void SendSCTVMessage()
    {
        string mediaVersion = "0";

        //if (xmlMedia != null)
        //    mediaVersion = xmlMedia.Attribute("version").ToString().Replace("version=", "");

        string serverResponse = SendSCTVMessage("updateMediaFile|" + mediaVersion);

        try
        {
            //if (serverResponse != null)
            //    xmlMedia = XElement.Parse(serverResponse);


        }
        catch (Exception ex)
        {

        }
    }

    public string SendSCTVMessage(string message)
    {
        byte[] bytes = new byte[1024];

        try
        {
            // Connect to a Remote server  
            // Get Host IP Address that is used to establish a connection  
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
            // If a host has multiple addresses, you will get a list of addresses  
            //IPHostEntry host = Dns.GetHostEntry(serverIP1);
            //IPAddress ipAddress = host.AddressList[0];
            IPAddress ipAddress = IPAddress.Parse("10.0.0.84");//ip of sctv server
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.    
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.    
            try
            {
                // Connect to Remote EndPoint  
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.    
                byte[] msg = Encoding.ASCII.GetBytes(message + "<EOF>");

                // Send the data through the socket.    
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.    
                //int bytesRec = sender.Receive(bytes);

                // Incoming data from the client.    
                string socketResponse = null;
                //byte[] bytes = null;

                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = sender.Receive(bytes);
                    socketResponse += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (socketResponse.IndexOf("<EOF>") > -1)
                    {
                        socketResponse = socketResponse.Replace("<EOF>", "");

                        break;
                    }
                }


                //Console.WriteLine("Echoed test = {0}",
                //   socketResponse);

                // Release the socket.    
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                //if we are calling updateMediaFile then get response and save file
                if(message.StartsWith("updateMediaFile") && socketResponse.Trim().Length > 10)
                {
                    //backup existing file
                    //File.WriteAllText(Application.persistentDataPath + "/media.xml.BAK", xmlMedia.ToString());

                    //save new file
                    //File.WriteAllText(Application.persistentDataPath + "/media.xml", socketResponse);
                }

                //if (resultBox)
                //    resultBox.text = "Success";

                return socketResponse;

            }
            catch (ArgumentNullException ane)
            {
                if (resultBox)
                    resultBox.text = ane.ToString();
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                if (resultBox)
                    resultBox.text = se.ToString();
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                if (resultBox)
                    resultBox.text = e.ToString();
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            if (resultBox)
                resultBox.text = e.ToString();

            Console.WriteLine(e.ToString());
        }

        return null;
    }

    public void Play()
    {
        //string filePath = EventSystem.current.currentSelectedGameObject.GetComponent<Media>().FilePath;

        //using sockets
        //SendSCTVMessage(filePath + "|" + selectedChromecastIP);

        //using vlc web interface
        //StartCoroutine(SendVlcCommand("in_play", currentFilePath));

        Play(currentFilePath);
    }

    public void Play(string filePath)
    {
        //using sockets
        //SendSCTVMessage(filePath + "|" + selectedChromecastIP);

        currentFilePath = filePath;

        //CurrentMediaTitle = (from seg in xmlMedia.Descendants("media")
        //                     select (XElement)seg)
        //                                    .Where(x => x.Elements("filePath")
        //                                        .Where(filePath1 => filePath1.Value.ToLower() == currentFilePath.ToLower()).Any()).FirstOrDefault().Element("title").Value;

        //nextActionTime = Time.time + StatusCheckInterval;

        //if (sctvState == SCTVState.paused)//we don't want to start the media over
        //{
        //    //sctvState = SCTVState.playing;
        //    //Play(currentFilePath);
        //    Pause();
        //}
        //else
        //{
        justChangedState = true;

        sctvState = SCTVState.playing;

        OnStatusUpdated?.Invoke(this, new SCTVStatusEventArgs(sctvState.ToString(), StatusTypeEnum.Message, StatusSeverityEnum.Normal));

        //using vlc web interface
        StartCoroutine(SendVlcCommand("in_play", filePath));

        //}
    }

    public void Stop()
    {
        StartCoroutine(SendVlcCommand("pl_stop", ""));
    }

    public void Pause()
    {
        StartCoroutine(SendVlcCommand("pl_pause", ""));
    }

    public void Seek(string newValue)
    {
        StartCoroutine(SendVlcCommand("seek", newValue));
    }

    public void Next()
    {
        StartCoroutine(SendVlcCommand("pl_next", ""));
    }

    public void Previous()
    {
        StartCoroutine(SendVlcCommand("pl_previous", ""));
    }

    public void Volume(string newVolume)
    {
        StartCoroutine(SendVlcCommand("volume", newVolume));
    }

    public void Status()
    {
        StartCoroutine(SendVlcCommand("status", ""));
    }

    private void saveFile(string FileName, string content)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(new FileStream("Assets/" + FileName, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                sw.Write(content);
            }
        }
        catch (Exception e)
        {
            string error = e.Message;
        }
    }

    private string findValue(string stringToParse, string startPattern, string endPattern)
    {
        return findValue(stringToParse, startPattern, endPattern, false);
    }

    private string findValue(string stringToParse, string startPattern, string endPattern, bool returnSearchPatterns)
    {
        int start = 0;
        int end = 0;
        string foundValue = "";

        try
        {
            start = stringToParse.IndexOf(startPattern);

            if (start > -1)
            {
                if (!returnSearchPatterns)
                    stringToParse = stringToParse.Substring(start + startPattern.Length);
                else
                    stringToParse = stringToParse.Substring(start);

                end = stringToParse.IndexOf(endPattern);

                if (end > 0)
                {
                    if (returnSearchPatterns)
                        foundValue = stringToParse.Substring(0, end + endPattern.Length);
                    else
                        foundValue = stringToParse.Substring(0, end);
                }
            }
        }
        catch (Exception ex)
        {
            //Tools.WriteToFile(ex);
        }

        return foundValue;
    }

    /// <summary>
    /// Find the string between the startPattern and endPattern and replace it with newValue
    /// </summary>
    /// <param name="stringToParse"></param>
    /// <param name="startPattern"></param>
    /// <param name="endPattern"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    private string findAndReplace(string stringToParse, string startPattern, string endPattern, string newValue)
    {
        string tempString = findValue(stringToParse, startPattern, endPattern, true);

        //check for xml tags - <LastPlayPosition />
        if (tempString.Trim().Length == 0 && startPattern.StartsWith("<"))
            tempString = findValue(stringToParse, startPattern.Replace(">", ""), "/>", true);

        stringToParse = stringToParse.Replace(tempString, startPattern + newValue + endPattern);

        return stringToParse;
    }

    public void HideMediaDetails()
    {
        //iTweenEvent.GetEvent(MediaDetailsDisplay_Large.GetComponentsInChildren<RectTransform>().Where(r => r.name == "MediaDetails_Prefab Large").FirstOrDefault().gameObject, "Hide").Play();

        //if (VerticalListView.activeSelf)
        //    iTweenEvent.GetEvent(VerticalListView, "Show").Play();

        //if (GridviewObject.activeSelf)
        //    iTweenEvent.GetEvent(GridviewObject, "Show").Play();

        MediaDetailsDisplay_Large.SetActive(false);
        VerticalListView.SetActive(true);
    }

    public void AnomaleDetected(clsAnomale Anomale)
    {

    }
}

public class SCTVStatusEventArgs : EventArgs
{
    public SCTVStatusEventArgs(string status)
    {
        SCTVStatus = status;
        StatusType = StatusTypeEnum.Message;
        Severity = StatusSeverityEnum.Normal;
    }

    public SCTVStatusEventArgs(string status, StatusTypeEnum statusType, StatusSeverityEnum severity)
    {
        SCTVStatus = status;
        StatusType = statusType;
        Severity = severity;
    }

    public string SCTVStatus { get; set; }
    public StatusTypeEnum StatusType { get; set; }//debug - error - message
    public StatusSeverityEnum Severity { get; set; }//normal - important
}




