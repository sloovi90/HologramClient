using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
public class MyClient : MonoBehaviour {
    int connectionId;
    int maxConnections = 10;
    int reliableChannelID;
    int hostId;
    int serverPort=8888;
    byte error;
    public bool connected = false;
    public bool send = false;
    bool expectImage = false;
    int expectedRecSize = -1;
    int receiveSize = -1;
    byte[] recData;
    string expectedImageName;
    public GameObject connectMenu;
    public GameObject gameMenu;
    // Use this for initialization

    public void Connect()
    {
                
        string address = GameObject.Find("inputAddress").GetComponentInChildren<Text>().text;
        Debug.Log(address);
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelID = config.AddChannel(QosType.ReliableSequenced);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostId = NetworkTransport.AddHost(topology,0);
        //GameObject.Find("DebugText").GetComponent<Text>().text = "attemptin to connect";
        connectionId = NetworkTransport.Connect(hostId, address, serverPort, 0, out error);
        Debug.Log(connectionId);
        connected = true;
        connectMenu.SetActive(false);
        gameMenu.SetActive(true);
        gameMenu.transform.position += new Vector3(0, 0, 1000);
       
    }
    public void Disconnect()
    {
        connected = false;
        NetworkTransport.Disconnect(hostId, connectionId, out error);
        connectMenu.SetActive(true);
        gameMenu.SetActive(false);
        GetComponent<InputController>().doneSetup = false;

    }
    public void SendMsg(string msg)
    {
        if (connected)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(msg);
            NetworkTransport.Send(hostId, connectionId, reliableChannelID, buffer, sizeof(char) * msg.Length, out error);
        }
    }
    void Start () {
       

    }
    void HandleMsg(string msg)
    {
        string[] message = msg.Split('|');
        InputController inputCon = GetComponent<InputController>();
        switch (message[0])
        {
            case "Presentation":
                if (message[1] == "auto")
                    inputCon.SetPresentationMode(true);
                if(message[1]=="manual")
                    inputCon.SetPresentationMode(false);
                break;
            case "SlidingSpeed":
                inputCon.SetSlidingSpeed(int.Parse(message[1]));
                break;
            case "AxisRotation":
                if (message[1] == "X") inputCon.SetRotationAxis(InputController.Axis.X,int.Parse(message[2]));
                if (message[1] == "Y") inputCon.SetRotationAxis(InputController.Axis.Y, int.Parse(message[2]));
                if (message[1] == "Z") inputCon.SetRotationAxis(InputController.Axis.Z, int.Parse(message[2]));
                break;
            case "FinishInit":
                inputCon.doneSetup = true;
                break;
            case "ExpectImage":
                expectedImageName = message[1];
                expectedRecSize = int.Parse(message[2]);
                recData = new byte[expectedRecSize];
                receiveSize = 0;
                break;



        }
    }
    // Update is called once per frame
    void Update() { 
        if(!connected)
            return;
        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] buffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        //in the last iteration we finished receiving image
        if (receiveSize == expectedRecSize && receiveSize != -1)
        {
            InputController inputCon = GetComponent<InputController>();
            inputCon.LoadImage(expectedImageName, recData);
            expectedRecSize = -1;
            receiveSize = -1;
            return;
        }
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, buffer, bufferSize, out dataSize, out error);
        switch (recNetworkEvent)
        {
            case NetworkEventType.DataEvent:
                if (receiveSize < expectedRecSize)
                {
                    for(int i = 0; i < dataSize; i++)
                    {
                        recData[receiveSize + i] = buffer[i];
                    }
                    receiveSize += dataSize;
                    return;
                }
               
                string msg = Encoding.Unicode.GetString(buffer,0,dataSize);

                //GameObject.Find("DebugText").GetComponent<Text>().text = msg;
                Debug.Log(msg);
                HandleMsg(msg);
                
                break;
            case NetworkEventType.ConnectEvent:
                //GameObject.Find("DebugText").GetComponent<Text>().text = "connected";
                Debug.Log("some1 with conID " + recConnectionID + " connected!");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("some1 with conID " + recConnectionID + " disconnected!");
                break;
            default:
                Debug.Log("default");
                break;
        }
        if (send)
        {
            SendMsg("connected");
            send = false;
        }
        
    }
}
