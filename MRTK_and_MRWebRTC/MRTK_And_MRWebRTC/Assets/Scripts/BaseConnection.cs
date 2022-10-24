/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC;
using Microsoft.MixedReality.WebRTC.Unity;
using PeerConnection = Microsoft.MixedReality.WebRTC.PeerConnection;

public class BaseConnection : MonoBehaviour
{
    private PeerConnection pc;
    private DataChannel data;
    private NodeDssSignaler _signaler;
    void Start()
    {
        pc = new PeerConnection();

        PeerConnectionConfiguration config = new PeerConnectionConfiguration
        {
            IceServers = new List<IceServer> {
            new IceServer{ Urls = { "stun:stun.l.google.com:19302" } }
        }
        };
        pc.InitializeAsync(config).Wait();

        Debug.Log("PeerConnection initialized");

        pc.DataChannelAdded += OnDataChannelAdded;
        pc.AddDataChannelAsync(40, "dummy", true, true).Wait();

        pc.LocalSdpReadytoSend += Peer_LocalSdpReadytoSend;

        _signaler = new NodeDssSignaler()
        {
            HttpServerAddress = "http://127.0.0.1:3000/",
            LocalPeerId = "ReceiverData",
            RemotePeerId = "SenderData",
        };
        _signaler.OnMessage += async (Message msg) =>
        {
            switch (msg.MessageType)
            {
                case WireMessageType.Offer:
                    // Wait for the offer to be applied
                    await pc.SetRemoteDescriptionAsync(msg.ToSdpMessage());
                    // Once applied, create an answer
                    pc.CreateAnswer();
                    break;

                case WireMessageType.Answer:
                    // No need to await this call; we have nothing to do after it
                    pc.SetRemoteDescriptionAsync(msg.ToSdpMessage()).Wait();
                    break;

                case WireMessageType.Ice:
                    pc.AddIceCandidate(msg.ToIceCandidate());
                    break;
            }
        };
        _signaler.StartPollingAsync();

        pc.Connected += () => {
            Debug.Log("PeerConnection: connected.");
        };

    }

    public void ButtonCreateOffer()
    {
        pc.CreateOffer();
    }

    private void OnDataChannelAdded(DataChannel channel)
    {
        Debug.Log("Hello " + data.Label);
        data = channel;
        data.StateChanged += OnDataChannelStateChanged;
    }

    private void OnDataChannelStateChanged()
    {
        Debug.Log("Dummy: " + data.State);
    }

    void OnApplicationQuit()
    {
        Debug.Log("App Closing");
        pc.Close();
        pc.Dispose();
        pc = null;

        if (_signaler != null)
        {
            _signaler.StopPollingAsync();
            _signaler = null;
        }
    }

    private void Peer_LocalSdpReadytoSend(SdpMessage message)
    {
        var msg = FromSdpMessage(message);
        _signaler.SendMessageAsync(msg);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}*/
