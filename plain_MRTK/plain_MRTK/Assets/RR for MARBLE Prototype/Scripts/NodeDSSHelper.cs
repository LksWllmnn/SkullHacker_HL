using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;

public class NodeDSSHelper : MonoBehaviour
{
    public NodeDssSignaler Signaler;

    public void StartConnection()
    {
        Signaler.PeerConnection.StartConnection();
    }
}
