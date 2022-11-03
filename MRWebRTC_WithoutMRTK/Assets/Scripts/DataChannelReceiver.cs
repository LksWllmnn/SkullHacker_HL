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
    private DataChannel dataDummy;

    private bool isPeerInitialized = false;

    private bool camRemoting = false;

    public GameObject head;

    public Vector3 modelPos;
    public GameObject model;

    Quaternion camRotQuad;
    Vector3 camPosVec;

    private void initPeer()
    {
        pc.Peer.DataChannelAdded += this.OnDataChannelAdded;
        pc.Peer.Connected += this.PeerIsConnected;

        Task<DataChannel> dummy = pc.Peer.AddDataChannelAsync(40, "dummy", false, false);
        dummy.Wait();

        Task<DataChannel> objChannel = pc.Peer.AddDataChannelAsync(41, "objChannel", false, false);
        objChannel.Wait();
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

        if (dataObj.State + "" == "Open")
        {
            
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
        Debug.Log(Encoding.UTF8.GetString(message));

        try
        {
            modelPos = StringToVector3(Encoding.UTF8.GetString(message));
        } catch (Exception e)
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

        if(camRemoting)
        {
            head.transform.rotation = camRotQuad;
            head.transform.position = camPosVec;

            model.transform.position = modelPos;
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
