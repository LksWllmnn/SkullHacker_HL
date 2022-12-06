using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.WebRTC;
using System.Text;
using UnityEngine.XR.OpenXR;
using Microsoft.MixedReality.Toolkit.XRSDK.OpenXR;
using System;
using Microsoft.MixedReality.Toolkit.Input;

public class PCReceiver : MonoBehaviour
{
    [Header("General Connection")]
    [Tooltip("PeerConnections which builds Connection with NodeDSS")]
    public Microsoft.MixedReality.WebRTC.Unity.PeerConnection PC;
    [Tooltip("Mixed Reality Cam of the Scene")]
    public Camera Cam;
    [Tooltip("Object in which the streamed object is placed and with which you can manipulate it.")]
    public GameObject RepresentationObject;
    [Tooltip("Material that the manipulation object assumes when hovering over it.")]
    public Material HoverMaterial;

    private DataChannel _dataDummy;
    private DataChannel _dataObj;
    private DataChannel _dataEye;
    private DataChannel _dataBounds;
    private DataChannel _dataVelo;
    private DataChannel _dataDepth;
    private DataChannel _dataDepthRight;

    private bool _setBounds;
    private Vector3 _bounds;
    private bool _isScaleSet = false;
    private Vector3 _standartScale;

    private Vector3 _camLastPos;
    private Quaternion _camLastRot;

    [Header("Depth Settings")]
    [Tooltip("Decides whether the data channels responsible for sending the depth information should be created in the receiver.")]
    public bool ShouldAddDepth;
    private byte[] _depthMessage;
    private Texture2D _depthTex;
    [Tooltip("Plane on which the information for the left eye is displayed.")]
    public GameObject PlaneLeft;
    private byte[] _depthMessageRight;
    private Texture2D _depthTexRight;
    [Tooltip("Plane on which the information for the right eye is displayed.")]
    public GameObject PlaneRight;

    private bool _shouldRenderDepth;
    [Tooltip("Material from the left side Plane.")]
    public Material MatLeft;
    [Tooltip("Material from the right side Plane.")]
    public Material MatRight;

    [Tooltip("Shader which calculates Depth into the local Scene from the remote Object for the left eye.")]
    public Shader DepthLeft;
    [Tooltip("Shader which calculates Depth into the local Scene from the remote Object for the right eye.")]
    public Shader DepthRight;
    [Tooltip("Shader which doesn't calculates Depth into the local Scene from the remote Object for the right eye.")]
    public Shader NormLeft;
    [Tooltip("Shader which doesn't calculates Depth into the local Scene from the remote Object for the right eye.")]
    public Shader NormRight;

    private void Start()
    {
        _camLastPos = Cam.transform.position;
        _camLastRot = Cam.transform.rotation;

        _depthTex = new Texture2D(2, 2);
        _depthTexRight = new Texture2D(2, 2);

        _shouldRenderDepth = ShouldAddDepth;
    }

    public void CreateChannels()
    {
        PC.Peer.DataChannelAdded += this.OnDataChannelAdded;
        Task<DataChannel> dummy = PC.Peer.AddDataChannelAsync(40, "dummy", false, false);
        dummy.Wait();

        Task<DataChannel> objChannel = PC.Peer.AddDataChannelAsync(41, "objChannel", false, false);
        objChannel.Wait();

        Task<DataChannel> eyeChannel = PC.Peer.AddDataChannelAsync(42, "eyeChannel", false, false);
        eyeChannel.Wait();

        Task<DataChannel> boundsChannel = PC.Peer.AddDataChannelAsync(43, "boundsChannel", false, false);
        boundsChannel.Wait();

        Task<DataChannel> veloChannel = PC.Peer.AddDataChannelAsync(44, "veloChannel", false, false);
        veloChannel.Wait();

        if(ShouldAddDepth)
        {
            Task<DataChannel> depthChannel = PC.Peer.AddDataChannelAsync(45, "depthChannel", false, false);
            depthChannel.Wait();

            Task<DataChannel> depthRightChannel = PC.Peer.AddDataChannelAsync(46, "depthRightChannel", false, false);
            depthRightChannel.Wait();
        }
        
    }

    private void OnDataChannelAdded(DataChannel channel)
    {
        switch (channel.Label)
        {
            case "dummy":
                _dataDummy = channel;
                _dataDummy.StateChanged += this.OnStateChangedDummy;
                break;

            case "objChannel":
                _dataObj = channel;
                _dataObj.StateChanged += this.OnStateChangedObj;
                break;
            case "eyeChannel":
                _dataEye = channel;
                _dataEye.StateChanged += this.OnStateChangedEye;
                break;
            case "boundsChannel":
                _dataBounds = channel;
                _dataBounds.StateChanged += this.OnStateChangedBounds;
                _dataBounds.MessageReceived += this.MessageReceivedBounds;
                break;
            case "veloChannel":
                _dataVelo = channel;
                _dataVelo.StateChanged += this.OnStateChangedVelo;
                break;
            case "depthChannel":
                _dataDepth = channel;
                _dataDepth.StateChanged += this.OnStateChangedDepth;
                _dataDepth.MessageReceived += this.MessageReceivedDepth;
                break;
            case "depthRightChannel":
                _dataDepthRight = channel;
                _dataDepthRight.StateChanged += this.OnStateChangedDepthRight;
                _dataDepthRight.MessageReceived += this.MessageReceivedDepthRight;
                break;
        }
    }
    private void OnStateChangedDummy()
    {
        Debug.Log("DataDummy: " + _dataDummy.State);
    }

    private void OnStateChangedObj()
    {
        Debug.Log("DataObj: " + _dataObj.State);
    }

    private void OnStateChangedEye()
    {
        Debug.Log("DataEye: " + _dataEye.State);
    }
    private void OnStateChangedBounds()
    {
        Debug.Log("DataBounds: " + _dataBounds.State);
    }

    private void OnStateChangedVelo()
    {
        Debug.Log("DataVelo: " + _dataVelo.State);
    }

    private void OnStateChangedDepth()
    {
        Debug.Log("DataDepth: " + _dataDepth.State);
    }

    private void OnStateChangedDepthRight()
    {
        Debug.Log("DataDepthRight: " + _dataDepthRight.State);
    }

    private void MessageReceivedBounds(byte[] message)
    {
        string m = Encoding.UTF8.GetString(message); 
        try
        {
            _bounds = StringToVector3(m);
            _setBounds = true;
        }catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void MessageReceivedDepth(byte[] message)
    {
        try
        {
            _depthMessage = message;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void MessageReceivedDepthRight(byte[] message)
    {
        try
        {
            _depthMessageRight = message;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void OnHoverShower()
    {
        RepresentationObject.GetComponent<MeshRenderer>().material = HoverMaterial;
    }

    public void OnExitHoverShower()
    {
        Material[] emptyArray = new Material[0];
        RepresentationObject.GetComponent<MeshRenderer>().materials = emptyArray;
    }

    public void SwitchMat()
    {
        if(_shouldRenderDepth)
        {
            PlaneLeft.GetComponent<Renderer>().material.shader = DepthLeft;
            PlaneRight.GetComponent<Renderer>().material.shader = DepthRight;
            _shouldRenderDepth = false;
        } else
        {
            PlaneLeft.GetComponent<Renderer>().material.shader = NormLeft;
            PlaneRight.GetComponent<Renderer>().material.shader = NormRight;
            _shouldRenderDepth = true;
        }
    }
    
    void Update()
    {
        
        if (_dataDummy != null && _dataDummy.State == DataChannel.ChannelState.Open)
        {
            _dataDummy.SendMessage(Encoding.ASCII.GetBytes(Cam.transform.rotation.ToString("F3") + "|" + Cam.transform.position.ToString("F3")));
        }

        if (_dataObj != null && _dataObj.State == DataChannel.ChannelState.Open && _isScaleSet)
        {
            Vector3 normalizedScale = new Vector3(RepresentationObject.transform.localScale.x / _standartScale.x, RepresentationObject.transform.localScale.y / _standartScale.y, RepresentationObject.transform.localScale.z / _standartScale.z);
            _dataObj.SendMessage(Encoding.ASCII.GetBytes(RepresentationObject.transform.position.ToString("F3") + "|" + RepresentationObject.transform.rotation.ToString("F3") + "|" + normalizedScale.ToString("F3")));
        }

        if (_dataEye != null && _dataEye.State == DataChannel.ChannelState.Open)
        {
            _dataEye.SendMessage(Encoding.ASCII.GetBytes("" + Cam.stereoSeparation));
        }

        if (_dataVelo != null && _dataVelo.State == DataChannel.ChannelState.Open)
        {
            _dataVelo.SendMessage(Encoding.ASCII.GetBytes(((Cam.transform.position - _camLastPos)*Time.deltaTime).ToString("F5") + "|" + ((Cam.transform.rotation.eulerAngles - _camLastRot.eulerAngles) * Time.deltaTime).ToString("F5")));
        }
        _camLastPos = Cam.transform.position;
        _camLastRot = Cam.transform.rotation;

        if (_setBounds)
        {
            _standartScale = new Vector3(_bounds.x, _bounds.z, _bounds.y);
            RepresentationObject.transform.localScale = _standartScale;
            _isScaleSet = true;
            _setBounds = false;
        }

        if(_shouldRenderDepth)
        {
            _depthTex.LoadImage(_depthMessage);
            PlaneLeft.GetComponent<Renderer>().material.SetTexture("_DepthPlane", _depthTex);

            _depthTexRight.LoadImage(_depthMessageRight);
            PlaneRight.GetComponent<Renderer>().material.SetTexture("_DepthPlane", _depthTexRight);
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
