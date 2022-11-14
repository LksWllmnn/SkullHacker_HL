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

    public TouchScreenKeyboard keyboard;

    private bool IsLocalIDWriting = false;
    private bool IsRemoteIDWriting = false;
    private bool IsIPIDWriting = false;

    MixedRealityKeyboard keyb;

    public void WriteLocalID()
    {
        IsLocalIDWriting = true;
        IsRemoteIDWriting = false;
        IsIPIDWriting = false;
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void WriteRemoteID()
    {
        IsLocalIDWriting = false;
        IsRemoteIDWriting = true;
        IsIPIDWriting = false;
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void WriteIPID()
    {
        IsLocalIDWriting = false;
        IsRemoteIDWriting = false;
        IsIPIDWriting = true;
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }
    
    public void Safe()
    {
        Signaler.LocalPeerId = LocalID.GetComponent<TextMesh>().text;
        Signaler.RemotePeerId = RemoteID.GetComponent<TextMesh>().text;
        Signaler.HttpServerAddress = "http://"+ IPID.GetComponent<TextMesh>().text + ":3000/";
    }

    private void Update()
    {
        if (keyboard != null && IsLocalIDWriting)
        {
            LocalID.GetComponent<TextMesh>().text = keyboard.text;
            // Do stuff with keyboardText
        }

        if (keyboard != null && IsRemoteIDWriting)
        {
            RemoteID.GetComponent<TextMesh>().text = keyboard.text;
            // Do stuff with keyboardText
        }

        if (keyboard != null && IsIPIDWriting)
        {
            IPID.GetComponent<TextMesh>().text = keyboard.text;
            // Do stuff with keyboardText
        }
    }
}
