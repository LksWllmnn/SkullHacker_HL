using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
using Microsoft.MixedReality.WebRTC;
using UnityEngine.UI;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;

public class ConnectionHandler : MonoBehaviour
{
    [Tooltip("All Connections. You don't have to add anything here.")]
    public List<GameObject> ConnectionGos;
    [Tooltip("The wished and prepared Connection Prefab you want.")]
    public GameObject ConnectionPrefab;
    [Tooltip("The Model GameObject with every Child.")]
    public GameObject Model;
    [Tooltip("The Input-Field of the Canvas")]
    public GameObject ConnectionIPInput;
    
    [Tooltip("The UI Prefab for showing that there are more Connections...doesn't do anything yet.")]
    public GameObject ConnectionThumbPrefab;

    public List<ConnectionThumbScript> ConnectionThumbScripts;
    public List<Connection> Connections;

    private void Start()
    {
        Connections = new List<Connection>();
        ConnectionGos = new List<GameObject>();
        for(int i = 0; i < ConnectionThumbScripts.Count; i++)
        {
            Connection newCon = new Connection("sender" + i, "receiver" + i, "closed", i, this);
            Connections.Add(newCon);
            ConnectionThumbScripts[i].SetSenderText(newCon.SenderId);
            ConnectionThumbScripts[i].SetReceiverText(newCon.ReceiverId);
            ConnectionThumbScripts[i].SetConnectionStatusText(newCon.Status);
            ConnectionThumbScripts[i].SetThisConnection(newCon);
            ConnectionGos.Add(null);
        }
    }

    public void EnableActivateButton()
    {
        for(int i = 0; i < ConnectionThumbScripts.Count; i++)
        {
            ConnectionThumbScripts[i].transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    public void InstanziateConnection(Connection con)
    {
        GameObject newConnection = Instantiate(ConnectionPrefab);
        ConnectionGos[con.Index] = newConnection;
        PCSender newConnectionDataChannelReceiver = newConnection.transform.GetChild(0).gameObject.GetComponent<PCSender>();
        newConnectionDataChannelReceiver.Model = Model;

        NodeDssSignaler newConNodeDSSSignaler = newConnection.transform.GetChild(1).gameObject.GetComponent<NodeDssSignaler>();
        newConNodeDSSSignaler.LocalPeerId = con.SenderId;
        newConNodeDSSSignaler.RemotePeerId = con.ReceiverId;
        newConNodeDSSSignaler.HttpServerAddress = "http://" + ConnectionIPInput.GetComponent<TMPro.TMP_InputField>().text + ":3000/";

        con.SetState(ConnectionState.Open);
    }

    public void Deconnect(Connection con, bool reConnect)
    {
        con.Peer.IceStateChanged -= con.LogIceState;
        GameObject conGo = ConnectionGos[con.Index];
        PeerConnection goPc = conGo.transform.GetChild(0).gameObject.GetComponent<PeerConnection>();
        goPc.gameObject.SetActive(false);
        Destroy(conGo);
        ConnectionGos[con.Index] = null;
        con.Peer = null;
        con.SetState(ConnectionState.Closed);
        
        if (reConnect) InstanziateConnection(con);
    }

    private void Update()
    {
        for(int i = 0; i<ConnectionGos.Count; i++)
        {
            if(ConnectionGos[i] != null)
            {
                GameObject conGo = ConnectionGos[i];
                PeerConnection pc = conGo.transform.GetChild(0).gameObject.GetComponent<PeerConnection>();
                if (Connections[i].Peer == null && pc.Peer != null)
                {
                    Connections[i].Peer = pc.Peer;
                    Connections[i].Peer.IceStateChanged += Connections[i].LogIceState;
                }

                if (Connections[i].IsConnected == true && Connections[i].WasConnected == false)
                {
                    Connections[i].SetState(ConnectionState.Connected);
                    Connections[i].WasConnected = true;
                }
                else if(Connections[i].IsConnected == false && Connections[i].WasConnected == true)
                {
                    Connections[i].SetState(ConnectionState.Closed);
                    Deconnect(Connections[i], true);
                    Connections[i].WasConnected = false;
                }
            }
        }
    }
}

public delegate void Notify();

public class Connection
{
    public string SenderId;
    public string ReceiverId;
    public string Status;
    public int Index;
    public bool WasConnected = false;
    public bool IsConnected = false;
    public Microsoft.MixedReality.WebRTC.PeerConnection Peer;
    private ConnectionState _state = ConnectionState.Closed;
    
    
    private ConnectionHandler _conHandler;

    public event Notify PcConnected;


    public Connection(string senderId, string receiverId, string status, int index, ConnectionHandler conHandler)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Status = status;
        Index = index;
        _conHandler = conHandler;
    }

    public void LogIceState(IceConnectionState state)
    {
        switch(state)
        {
            case IceConnectionState.Connected:
                this.IsConnected = true;
                break;
            case IceConnectionState.Disconnected:
                this.IsConnected= false;
                break;
        }
    }

    public ConnectionState GetState()
    {
        return this._state;
    }

    public void SetState(ConnectionState state)
    {
        this._state = state;
        switch (state)
        {
            case ConnectionState.Connected:
                this.SetConnectedText();
                break;
            case ConnectionState.Closed:
                this.SetDisconnected();
                break;
            case ConnectionState.Open:
                this.SetOpenText();
                break;
        }
    }

    public void ActivateConnection()
    {
        _conHandler.InstanziateConnection(this);
    }

    public void DeactivateConnection()
    {
        _conHandler.Deconnect(this, false);
    }


    protected virtual void SetOpenText()
    {
        PcConnected?.Invoke();
    }

    protected virtual void SetConnectedText()
    {
        PcConnected?.Invoke();
    }

    protected virtual void SetDisconnected()
    {
        PcConnected?.Invoke();
    }
}

public enum ConnectionState
{
    Open, Connected, Closed
}
