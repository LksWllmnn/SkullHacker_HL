using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
using Microsoft.MixedReality.WebRTC;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;
using System.Text;
using System;
using System.Threading.Tasks;

public class DataChannelReceiver : MonoBehaviour
{
    public PeerConnection pc;

    private DataChannel dataObj;
    private DataChannel dataEye;
    private DataChannel dataDummy;
    private DataChannel dataBounds;

    private bool isPeerInitialized = false;

    private bool camRemoting = false;

    public GameObject head;
    public float stereoSeperation = 0.022f;
    public bool eyesAreSet = false;
    public bool setEyes = false;

    public bool sendBounds = false;

    public Vector3 modelPos;
    public Quaternion modelRot;
    public Vector3 modelScale;
    public GameObject model;

    Quaternion camRotQuad;
    Vector3 camPosVec;

    private Vector3 modelBounds;

    private void initPeer()
    {
        pc.Peer.DataChannelAdded += this.OnDataChannelAdded;
        pc.Peer.Connected += this.PeerIsConnected;

        Task<DataChannel> dummy = pc.Peer.AddDataChannelAsync(40, "dummy", false, false);
        dummy.Wait();

        Task<DataChannel> objChannel = pc.Peer.AddDataChannelAsync(41, "objChannel", false, false);
        objChannel.Wait();

        Task<DataChannel> eyeChannel = pc.Peer.AddDataChannelAsync(42, "eyeChannel", false, false);
        eyeChannel.Wait();

        Task<DataChannel> boundsChannel = pc.Peer.AddDataChannelAsync(43, "boundsChannel", false, false);
        boundsChannel.Wait();
    }

    private void OnDataChannelAdded(DataChannel channel)
    {
        Debug.Log("Hello " + channel.Label);
        switch (channel.Label)
        {
            case "dummy":
                dataDummy = channel;
                dataDummy.StateChanged += this.OnStateChangedDummy;
                dataDummy.MessageReceived += OnMessageReceived;
                break;
            case "objChannel":
                dataObj = channel;
                dataObj.StateChanged += this.OnStateChanged1;
                dataObj.MessageReceived += OnMessageReceivedObj;
                break;
            case "eyeChannel":
                dataEye = channel;
                dataEye.StateChanged += this.OnStateChangedEye;
                dataEye.MessageReceived += OnMessageReceivedEye;
                break;
            case "boundsChannel":
                dataBounds = channel;
                dataBounds.StateChanged += this.OnStateChangedBounds;
                break;
        }
    }

    private void OnStateChangedDummy()
    {
        Debug.Log("DataDummy: " + dataDummy.State);

        if (dataDummy.State + "" == "Open")
        {
            camRemoting = true;
        }
    }

    private void OnStateChanged1()
    {
        Debug.Log("Data1: " + dataObj.State);
    }

    private void OnStateChangedEye()
    {
        Debug.Log("Data1: " + dataEye.State);
    }

    private void OnStateChangedBounds()
    {
        Debug.Log("Data1: " + dataBounds.State);
        if(dataBounds.State  == DataChannel.ChannelState.Open)
        {
            sendBounds = true;
        }
    }

    private void PeerIsConnected()
    {
        Debug.Log("Peer is Connected ...lw");
    }

    private void OnMessageReceived(byte[] message)
    {
       
        string[] camTransString = Encoding.UTF8.GetString(message).Split("|");

        try
        {
            string camRot = camTransString[0];
            string camTrans = camTransString[1];

            camRotQuad = StringToQuaternion(camRot);
            camPosVec = StringToVector3(camTrans);
        } catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }

    private void OnMessageReceivedObj(byte[] message)
    {

        string[] messageStrings = Encoding.UTF8.GetString(message).Split("|");

        try
        {
            modelPos = StringToVector3(messageStrings[0]);
            modelRot = StringToQuaternion(messageStrings[1]);
            modelScale = StringToVector3(messageStrings[2]);
            
        } catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }

    private void OnMessageReceivedEye(byte[] message)
    {
        //Debug.Log(Encoding.UTF8.GetString(message));
        //string messageString = Encoding.UTF8.GetString(message);

        try
        {
            //messageString.Replace(",", ".");
            //stereoSeperation = float.Parse(messageString);
            setEyes = true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!isPeerInitialized && pc.Peer != null)
        {
            isPeerInitialized = true;
            Debug.Log("Peer is available");
            initPeer();
        }

        if(setEyes && !eyesAreSet)
        {
            Vector3 leftEye = new Vector3(0, 0, 0);
            
            Vector3 rightEye = new Vector3(stereoSeperation, 0, 0);
            Debug.Log(rightEye);
            head.transform.GetChild(0).transform.position = leftEye;
            head.transform.GetChild(1).transform.position = rightEye;
            setEyes = false;
            eyesAreSet = true;
        }

        if(camRemoting)
        {
            head.transform.rotation = camRotQuad;
            head.transform.position = camPosVec;
            //head.transform.Translate(new Vector3(0, 0, -0.15f));

            model.transform.position = modelPos;
            model.transform.rotation = modelRot;
            model.transform.localScale = modelScale;
            
        }
        


        if(sendBounds)
        {
            Debug.Log("Bounds: " + (Matrix4x4.Scale(model.transform.GetChild(0).transform.localScale) * (model.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.extents * 2)).ToString("F3"));
            dataBounds.SendMessage(Encoding.UTF8.GetBytes((Matrix4x4.Scale(model.transform.GetChild(0).transform.localScale) * (model.transform.GetChild(0).transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.extents * 2)).ToString("F3")));
            sendBounds = false;
        }
        
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
            //Debug.Log("sVector: " + sVector);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0])/1000,
            float.Parse(sArray[1])/1000,
            float.Parse(sArray[2])/1000);

        return result;
    }

    public static Vector4 StringToVector4(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector4 result = new Vector4(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));

        return result;
    }

    public static Quaternion StringToQuaternion(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Quaternion result = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));

        return result;
    }
}
