using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
using UnityEngine.InputSystem;

using Microsoft.MixedReality.Toolkit.Experimental.UI;


public class ConInfos : MonoBehaviour
{
    public NodeDssSignaler Signaler;
    public PCReceiver PcR;
    public GameObject LocalID;
    public GameObject RemoteID;
    public GameObject IPID;
    public GameObject DepthTextGo;
    public GameObject NoDepthTextGo;

    public TouchScreenKeyboard Keyboard;

    private bool _depthSelection;

    
    public void Safe()
    {
        Signaler.LocalPeerId = LocalID.GetComponent<TextMesh>().text;
        Signaler.RemotePeerId = RemoteID.GetComponent<TextMesh>().text;
        Signaler.HttpServerAddress = "http://"+ IPID.GetComponent<TextMesh>().text + ":3000/";
        PcR.ShouldAddDepth = _depthSelection;
    }

    public void SelectDepthVersion()
    {
        _depthSelection = true;
        DepthTextGo.GetComponent<TextMesh>().color = Color.green;
        NoDepthTextGo.GetComponent<TextMesh>().color = Color.white;
    }

    public void SelectNoDepthVersion()
    {
        _depthSelection = false;
        DepthTextGo.GetComponent<TextMesh>().color = Color.white;
        NoDepthTextGo.GetComponent<TextMesh>().color = Color.green;
    }
}
