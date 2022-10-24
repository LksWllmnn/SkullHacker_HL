using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
using Microsoft.MixedReality.WebRTC;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;
using System.Text;
using System;

public class DataChannelReceiver : MonoBehaviour
{
    public PeerConnection pc;
    private DataChannel data1;

    private DataChannel dataDummy;

    private bool isPeerInitialized = false;
    private bool isPeerConnected = false;

    private bool camRemoting = false;

    public Camera cam;

    Quaternion camRotQuadLast;
    Vector3 camPosVecLast;
    Quaternion camRotQuad;
    Vector3 camPosVec;

    private void initPeer()
    {
        pc.Peer.DataChannelAdded += this.OnDataChannelAdded;
        pc.Peer.Connected += this.PeerIsConnected;

        pc.Peer.AddDataChannelAsync(40, "dummy", true, true).Wait();
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
            /*case "dataChannel":
                data1 = channel;
                data1.StateChanged += this.OnStateChanged1;
                break;*/
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

    /*private void OnStateChanged1()
    {
        Debug.Log("Data1: " + data1.State);

        if (data1.State + "" == "Open")
        {
            camRemoting = true;
        }
    }*/

    private void PeerIsConnected()
    {
        Debug.Log("Peer is Connected ...lw");
        isPeerConnected = true;
    }

    private void OnMessageReceived(byte[] message)
    {
        //Debug.Log(Encoding.UTF8.GetString(message));
        string[] camTransString = Encoding.UTF8.GetString(message).Split("|");

        try
        {
            //Debug.Log(camTransString[1]);
            string camRot = camTransString[0];
            string camTrans = camTransString[1];

            camRotQuad = StringToQuaternion(camRot);
            camPosVec = StringToVector3(camTrans);
            Debug.Log(camPosVec);
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
            cam.transform.rotation = camRotQuad;
            cam.transform.position = camPosVec;
        }
        
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
            Debug.Log("sVector: " + sVector);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0])/10,
            float.Parse(sArray[1])/10,
            float.Parse(sArray[2])/10);

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
