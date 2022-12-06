using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
using UnityEngine.InputSystem;

using Microsoft.MixedReality.Toolkit.Experimental.UI;


public class ConInfos : MonoBehaviour
{
    public NodeDssSignaler Signaler;
    public GameObject LocalID;
    public GameObject RemoteID;
    public GameObject IPID;

    public TouchScreenKeyboard Keyboard;

    private bool _isLocalIDWriting = false;
    private bool _isRemoteIDWriting = false;
    private bool _isIPIDWriting = false;

    MixedRealityKeyboard _keyb;

    public void WriteLocalID()
    {
        _isLocalIDWriting = true;
        _isRemoteIDWriting = false;
        _isIPIDWriting = false;
        Keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void WriteRemoteID()
    {
        _isLocalIDWriting = false;
        _isRemoteIDWriting = true;
        _isIPIDWriting = false;
        Keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void WriteIPID()
    {
        _isLocalIDWriting = false;
        _isRemoteIDWriting = false;
        _isIPIDWriting = true;
        Keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }
    
    public void Safe()
    {
        Signaler.LocalPeerId = LocalID.GetComponent<TextMesh>().text;
        Signaler.RemotePeerId = RemoteID.GetComponent<TextMesh>().text;
        Signaler.HttpServerAddress = "http://"+ IPID.GetComponent<TextMesh>().text + ":3000/";
    }

    private void Update()
    {
        if (Keyboard != null && _isLocalIDWriting)
        {
            LocalID.GetComponent<TextMesh>().text = Keyboard.text;
            // Do stuff with keyboardText
        }

        if (Keyboard != null && _isRemoteIDWriting)
        {
            RemoteID.GetComponent<TextMesh>().text = Keyboard.text;
            // Do stuff with keyboardText
        }

        if (Keyboard != null && _isIPIDWriting)
        {
            IPID.GetComponent<TextMesh>().text = Keyboard.text;
            // Do stuff with keyboardText
        }
    }
}
