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
    private DataChannel dataVelo;
    private DataChannel dataDepth;
    private DataChannel dataDepthRight;

    private bool isPeerInitialized = false;
    private bool camRemoting = false;

    public GameObject head;
    public float stereoSeperation = 0.022f;
    public float calcSep;
    private bool eyesAreSet = false;
    private bool setEyes = false;

    private bool showStats = false;
    private bool sendBounds = false;

    private Vector3 modelPos;
    private Quaternion modelRot;
    private Vector3 modelScale;
    public GameObject model;

    Quaternion camRotQuad;
    Vector3 camPosVec;

    Vector3 camPosVelo = new Vector3(0,0,0);
    Vector3 lastCamposVelo;
    Vector3 camRotVelo = new Vector3(0,0,0);
    Vector3 lastCamRotVelo;
    public float veloFaktor = 10000;
    public float addedFactor;

    public bool rotationIsStatic;

    public bool activateDepthInfo;
    public dt2 depthTexture;
    public dt2 depthTextureRight;
    private bool sendDepth = false;

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

        Task<DataChannel> veloChannel = pc.Peer.AddDataChannelAsync(44, "veloChannel", false, false);
        veloChannel.Wait();

        

        if (activateDepthInfo)
        {
            Task<DataChannel> depthChannel = pc.Peer.AddDataChannelAsync(45, "depthChannel", false, false);
            depthChannel.Wait();

            Task<DataChannel> depthRightChannel = pc.Peer.AddDataChannelAsync(46, "depthRightChannel", false, false);
            depthRightChannel.Wait();
        }
        
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
            case "veloChannel":
                dataVelo = channel;
                dataVelo.StateChanged += this.OnStateChangedVelo;
                dataVelo.MessageReceived += OnMessageReceivedVelo;
                break;
            case "depthChannel":
                dataDepth = channel;
                dataDepth.StateChanged += this.OnStateChangedDepth;
                break;
            case "depthRightChannel":
                dataDepthRight = channel;
                dataDepthRight.StateChanged += this.OnStateChangedDepthRight;
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
        Debug.Log("DataObj: " + dataObj.State);
    }

    private void OnStateChangedEye()
    {
        Debug.Log("DataEyes: " + dataEye.State);
    }

    private void OnStateChangedBounds()
    {
        Debug.Log("DataBounds: " + dataBounds.State);
        if(dataBounds.State  == DataChannel.ChannelState.Open)
        {
            sendBounds = true;
        }
    }

    private void OnStateChangedVelo()
    {
        Debug.Log("Velo: " + dataVelo.State);
    }

    private void OnStateChangedDepth()
    {
        Debug.Log("Depth: " + dataDepth.State);
        if(dataDepth.State == DataChannel.ChannelState.Open)
        {
            sendDepth = true;
        }
    }

    private void OnStateChangedDepthRight()
    {
        Debug.Log("DepthRight: " + dataDepthRight.State);
        
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

    private void OnMessageReceivedVelo(byte[] message)
    {
        string[] camTransVeloString = Encoding.UTF8.GetString(message).Split("|");
        lastCamRotVelo = camRotVelo;
        lastCamposVelo = camPosVelo;
        try
        {
            string camPosVeloS = camTransVeloString[0];
            string camRotVeloS = camTransVeloString[1];

            camPosVelo = StringToVector3_2(camPosVeloS);
            camRotVelo = StringToVector3_2(camRotVeloS);
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

        if(setEyes)
        {
            calcSep = -0.1f * Vector3.Distance(model.transform.position,head.transform.position) + 0.08f;
            Vector3 leftEye = new Vector3(0, 0, 0);
            
            Vector3 rightEye = new Vector3(stereoSeperation, 0, 0);
            head.transform.GetChild(0).transform.localPosition = leftEye;
            head.transform.GetChild(1).transform.localPosition = rightEye;
            setEyes = false;
            eyesAreSet = true;
            showStats = true;
        }

        if(camRemoting)
        {
            if(!rotationIsStatic)
            {
                head.transform.rotation = camRotQuad;
                head.transform.Rotate(camRotVelo * (((camRotVelo.magnitude - lastCamRotVelo.magnitude) / Time.deltaTime) * veloFaktor));
            } else
            {
                head.transform.LookAt(model.transform.GetChild(0).transform.position);
            }
            

            addedFactor = ((camPosVelo.magnitude - lastCamposVelo.magnitude)/Time.deltaTime) * veloFaktor;
            head.transform.position = camPosVec;
            head.transform.Translate(camPosVelo * addedFactor);
            

            model.transform.position = modelPos;
            model.transform.rotation = modelRot;
            model.transform.localScale = modelScale;
        }

        if(sendBounds)
        {
            dataBounds.SendMessage(Encoding.UTF8.GetBytes((Matrix4x4.Scale(model.transform.GetChild(0).transform.localScale) * (model.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.extents * 2)).ToString("F3")));
            sendBounds = false;
        }

        if(showStats)
        {
            //SetEyeSeparationStats(Vector3.Distance(model.transform.position, head.transform.position) + "", calcSep + "");
        }

        if(sendDepth)
        {
            dataDepth.SendMessage(depthTexture.jpgSample);
            dataDepthRight.SendMessage(depthTextureRight.jpgSample);
        }
    }

    private void SetEyeSeparationStats(string distance, string seperation)
    {
        //model.transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = distance;
        //model.transform.GetChild(0).GetChild(1).GetComponent<TextMesh>().text = seperation;
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

    public static Vector3 StringToVector3_2(string sVector)
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
            float.Parse(sArray[0]) / 100000,
            float.Parse(sArray[1]) / 100000,
            float.Parse(sArray[2]) / 100000);

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
