using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.WebRTC;
using System.Text;
using UnityEngine.XR.OpenXR;
using Microsoft.MixedReality.Toolkit.XRSDK.OpenXR;
using System;

public class PCReceiver : MonoBehaviour
{
    public Microsoft.MixedReality.WebRTC.Unity.PeerConnection pC;
    public Camera cam;
    public GameObject representationObject;

    private DataChannel dataDummy;
    private DataChannel dataObj;
    private DataChannel dataEye;
    private DataChannel dataBounds;

    private bool setBounds;
    private Vector3 bounds;
    private Vector3 standartScale;

    public void CreateChannels()
    {
        //Debug.Log("Eyes: " + cam.stereoSeparation);
        
        pC.Peer.DataChannelAdded += this.OnDataChannelAdded;
        Task<DataChannel> dummy = pC.Peer.AddDataChannelAsync(40, "dummy", false, false);
        dummy.Wait();

        Task<DataChannel> objChannel = pC.Peer.AddDataChannelAsync(41, "objChannel", false, false);
        objChannel.Wait();

        Task<DataChannel> eyeChannel = pC.Peer.AddDataChannelAsync(42, "eyeChannel", false, false);
        eyeChannel.Wait();

        Task<DataChannel> boundsChannel = pC.Peer.AddDataChannelAsync(43, "boundsChannel", false, false);
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
                break;

            case "objChannel":
                dataObj = channel;
                dataObj.StateChanged += this.OnStateChangedObj;
                break;
            case "eyeChannel":
                dataEye = channel;
                dataEye.StateChanged += this.OnStateChangedEye;
                break;
            case "boundsChannel":
                dataBounds = channel;
                dataBounds.StateChanged += this.OnStateChangedBounds;
                dataBounds.MessageReceived += this.MessageReceivedBounds;
                break;
        }
    }
    private void OnStateChangedDummy()
    {
        Debug.Log("DataDummy: " + dataDummy.State);

        if (dataDummy.State == DataChannel.ChannelState.Open)
        {
            dataDummy.SendMessage(Encoding.ASCII.GetBytes("Hello over there... from Dummy"));
            Debug.Log("Message sended... from Dummy");
        }
    }

    private void OnStateChangedObj()
    {
        Debug.Log("DataObj: " + dataObj.State);

        if (dataObj.State == DataChannel.ChannelState.Open)
        {
            dataObj.SendMessage(Encoding.ASCII.GetBytes("Hello over there"));
            Debug.Log("Message sended... from Obj");
        }
    }

    private void OnStateChangedEye()
    {
        Debug.Log("DataEye: " + dataEye.State);
    }
    private void OnStateChangedBounds()
    {
        Debug.Log("DataBounds: " + dataBounds.State);
    }

    private void MessageReceivedBounds(byte[] message)
    {
        string m = Encoding.UTF8.GetString(message); 
        try
        {
            bounds = StringToVector3(m);
            setBounds = true;
        }catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    // Update is called once per frame
    void Update()
    {

        
        if (dataDummy != null && dataDummy.State == DataChannel.ChannelState.Open)
        {
            dataDummy.SendMessage(Encoding.ASCII.GetBytes(cam.transform.rotation.ToString("F3") + "|" + cam.transform.position.ToString("F3")));
        }

        if (dataObj != null && dataObj.State == DataChannel.ChannelState.Open)
        {
            dataObj.SendMessage(Encoding.ASCII.GetBytes(representationObject.transform.position.ToString("F3") + "|" + representationObject.transform.rotation.ToString("F3") + "|" + representationObject.transform.localScale.ToString("F3")));
        }

        if (dataEye != null && dataEye.State == DataChannel.ChannelState.Open)
        {
            dataEye.SendMessage(Encoding.ASCII.GetBytes("" + cam.stereoSeparation));
        }

        if(setBounds)
        {
            standartScale = new Vector3(bounds.x, bounds.z, bounds.y);
            representationObject.transform.GetChild(0).transform.localScale = standartScale;
            representationObject.transform.GetChild(0).transform.localPosition = new Vector3(0, standartScale.y / 2, 0);
            setBounds = false;
        }
    }

    public static Vector3 StringToVector3(string sVector)
    {
        if( sVector.StartsWith("(") && sVector.EndsWith(")") )
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        string[] sArray = sVector.Split(',');

        Vector3 result = new Vector3(
            float.Parse(sArray[0]) / 1000,
            float.Parse(sArray[1]) / 1000,
            float.Parse(sArray[2]) / 1000);

        return result;
    }
}
