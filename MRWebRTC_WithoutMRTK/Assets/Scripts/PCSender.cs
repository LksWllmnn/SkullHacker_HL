using UnityEngine;
using Microsoft.MixedReality.WebRTC;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;
using System.Text;
using System;
using System.Threading.Tasks;

public class PCSender : MonoBehaviour
{
    [Header("General Connection - Sender")]
    [Tooltip("Peer Connection which builds Connection with NodeDSS")]
    public PeerConnection PC;

    private DataChannel _dataObj;
    private DataChannel _dataEye;
    private DataChannel _dataDummy;
    private DataChannel _dataBounds;
    private DataChannel _dataVelo;
    private DataChannel _dataDepth;
    private DataChannel _dataDepthRight;
    private DataChannel _dataScalePlane;

    private bool _isPeerInitialized = false;
    private bool _camRemoting = false;

    [Tooltip("GameObject which is the Parent of the two Cameras.")]
    public GameObject Head;
    [Tooltip("Seperation of the two Cameras (Eye Seperation)")]
    [Range(0,0.04f)]public float StereoSeperation = 0.022f;
    private bool _setEyes = false;

    private bool _sendBounds = false;

    private Vector3 _modelPos;
    private Quaternion _modelRot;
    private Vector3 _modelScale;
    [Tooltip("Model which should be streamed. Beware of z-up or y-up Models. Main Mesh has to be at environment>Model>Mesh (Child[0])")]
    public GameObject Model;

    Quaternion _camRotQuad;
    Vector3 _camPosVec;

    Vector3 _camPosVelo = new Vector3(0,0,0);
    Vector3 _lastCamposVelo;
    Vector3 _camRotVelo = new Vector3(0,0,0);
    Vector3 _lastCamRotVelo;
    [Tooltip("Velocity of prediction Direction.")]
    public float VeloFaktor = 5;
    [Tooltip("Prediction Factor which is calculated every Frame. Your input here doesn't change anything. Just for watching.")]
    public float AddedFactor;

    [Tooltip("Value depends on whether the displaying image is attached to the manipulator object in the receiver scene or to the camera. If it is attached to the camera then 'false' otherwise 'true'.")]
    public bool RotationIsStatic;

    [Header("Depth Section")]
    [Tooltip("Should Depth Information be sended or not.")]
    public bool ActivateDepthInfo;
    [Tooltip("DepthTexture Calculation Script which should be added to the left eye Camera")]
    public DepthTextureExtractor DepthTexture;
    [Tooltip("DepthTexture Calculation Script which should be added to the right eye Camera")]
    public DepthTextureExtractor DepthTextureRight;
    private bool _sendDepth = false;

    private bool _sendPlaneScale = false;
    private float _scalePlaneFactor;

    private void initPeer()
    {
        PC.Peer.DataChannelAdded += this.OnDataChannelAdded;
        PC.Peer.Connected += this.PeerIsConnected;

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

        if (ActivateDepthInfo)
        {
            Task<DataChannel> depthChannel = PC.Peer.AddDataChannelAsync(45, "depthChannel", false, false);
            depthChannel.Wait();

            Task<DataChannel> depthRightChannel = PC.Peer.AddDataChannelAsync(46, "depthRightChannel", false, false);
            depthRightChannel.Wait();
        }

        Task<DataChannel> planeScaleChannel = PC.Peer.AddDataChannelAsync(47, "planeScaleChannel", false, false);
        planeScaleChannel.Wait();
    }

    private void OnDataChannelAdded(DataChannel channel)
    {
        Debug.Log("Hello " + channel.Label);
        switch (channel.Label)
        {
            case "dummy":
                _dataDummy = channel;
                _dataDummy.StateChanged += this.OnStateChangedDummy;
                _dataDummy.MessageReceived += OnMessageReceived;
                break;
            case "objChannel":
                _dataObj = channel;
                _dataObj.StateChanged += this.OnStateChanged1;
                _dataObj.MessageReceived += OnMessageReceivedObj;
                break;
            case "eyeChannel":
                _dataEye = channel;
                _dataEye.StateChanged += this.OnStateChangedEye;
                _dataEye.MessageReceived += OnMessageReceivedEye;
                break;
            case "boundsChannel":
                _dataBounds = channel;
                _dataBounds.StateChanged += this.OnStateChangedBounds;
                break;
            case "veloChannel":
                _dataVelo = channel;
                _dataVelo.StateChanged += this.OnStateChangedVelo;
                _dataVelo.MessageReceived += OnMessageReceivedVelo;
                break;
            case "depthChannel":
                _dataDepth = channel;
                _dataDepth.StateChanged += this.OnStateChangedDepth;
                break;
            case "depthRightChannel":
                _dataDepthRight = channel;
                _dataDepthRight.StateChanged += this.OnStateChangedDepthRight;
                break;
            case "planeScaleChannel":
                _dataScalePlane = channel;
                _dataScalePlane.StateChanged += this.OnStateChangedScalePlane;
                break;
        }
    }

    private void OnStateChangedDummy()
    {
        Debug.Log("DataDummy: " + _dataDummy.State);

        if (_dataDummy.State + "" == "Open")
        {
            _camRemoting = true;
        }
    }

    private void OnStateChanged1()
    {
        Debug.Log("DataObj: " + _dataObj.State);
    }

    private void OnStateChangedEye()
    {
        Debug.Log("DataEyes: " + _dataEye.State);
    }

    private void OnStateChangedBounds()
    {
        Debug.Log("DataBounds: " + _dataBounds.State);
        if(_dataBounds.State  == DataChannel.ChannelState.Open)
        {
            _sendBounds = true;
        }
    }

    private void OnStateChangedVelo()
    {
        Debug.Log("Velo: " + _dataVelo.State);
    }

    private void OnStateChangedDepth()
    {
        Debug.Log("Depth: " + _dataDepth.State);
        if(_dataDepth.State == DataChannel.ChannelState.Open)
        {
            _sendDepth = true;
        }
    }

    private void OnStateChangedDepthRight()
    {
        Debug.Log("DepthRight: " + _dataDepthRight.State);
    }

    private void OnStateChangedScalePlane()
    {
        Debug.Log("PlaneScale: " + _dataScalePlane.State);

        if( _dataScalePlane.State == DataChannel.ChannelState.Open)
        {
            _sendPlaneScale = true;
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

            _camRotQuad = StringToQuaternion(camRot);
            _camPosVec = StringToVector3_F4(camTrans);
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
            _modelPos = StringToVector3(messageStrings[0]);
            _modelRot = StringToQuaternion(messageStrings[1]);
            _modelScale = StringToVector3(messageStrings[2]);
            
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
            _setEyes = true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    private void OnMessageReceivedVelo(byte[] message)
    {
        string[] camTransVeloString = Encoding.UTF8.GetString(message).Split("|");
        _lastCamRotVelo = _camRotVelo;
        _lastCamposVelo = _camPosVelo;
        try
        {
            string camPosVeloS = camTransVeloString[0];
            string camRotVeloS = camTransVeloString[1];

            _camPosVelo = StringToVector3_F5(camPosVeloS);
            _camRotVelo = StringToVector3_F5(camRotVeloS);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!_isPeerInitialized && PC.Peer != null)
        {
            _isPeerInitialized = true;
            Debug.Log("Peer is available");
            initPeer();
        }

        if(_setEyes)
        {
            Vector3 leftEye = new Vector3(0, 0, 0);
            
            Vector3 rightEye = new Vector3(StereoSeperation, 0, 0);
            Head.transform.GetChild(0).transform.localPosition = leftEye;
            Head.transform.GetChild(1).transform.localPosition = rightEye;
            _setEyes = false;
        }

        if(_camRemoting)
        {
            if(!RotationIsStatic)
            {
                Head.transform.rotation = _camRotQuad;
                Head.transform.Rotate(_camRotVelo * (((_camRotVelo.magnitude - _lastCamRotVelo.magnitude) / Time.deltaTime) * VeloFaktor));
            } else
            {
                Head.transform.LookAt(Model.transform.GetChild(0).transform.position);
            }

            AddedFactor = ((_camPosVelo.magnitude - _lastCamposVelo.magnitude)/Time.deltaTime) * VeloFaktor;
            Head.transform.position = _camPosVec;
            Head.transform.Translate(_camPosVelo * AddedFactor);
            

            Model.transform.position = _modelPos;
            Model.transform.rotation = _modelRot;
            Model.transform.localScale = _modelScale;
        }

        if(_sendBounds)
        {
            _dataBounds.SendMessage(Encoding.UTF8.GetBytes((Matrix4x4.Scale(Model.transform.GetChild(0).transform.localScale) * (Model.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.extents * 2)).ToString("F3")));
            _sendBounds = false;
        }

        if(_sendDepth)
        {
            _dataDepth.SendMessage(DepthTexture.JpgSample);
            _dataDepthRight.SendMessage(DepthTextureRight.JpgSample);
        }

        if(_sendPlaneScale)
        {
            _scalePlaneFactor = 2 * Vector3.Distance(Model.transform.position, Head.transform.position);
            _dataScalePlane.SendMessage(Encoding.UTF8.GetBytes(_scalePlaneFactor.ToString("F3")));
        }
    }

    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
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

    public static Vector3 StringToVector3_F4(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]) / 10000,
            float.Parse(sArray[1]) / 10000,
            float.Parse(sArray[2]) / 10000);

        return result;
    }

    public static Vector3 StringToVector3_F5(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
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
