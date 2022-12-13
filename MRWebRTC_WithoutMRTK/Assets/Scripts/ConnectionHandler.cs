using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
using UnityEngine.UI;

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
    }

    public void Deconnect(Connection con)
    {
        GameObject conGo = ConnectionGos[con.Index];
        PeerConnection goPc = conGo.transform.GetChild(0).gameObject.GetComponent<PeerConnection>();

        goPc.gameObject.SetActive(false);
        Destroy(conGo);
        ConnectionGos[con.Index] = null;
    }

    private void Update()
    {
        for(int i = 0; i<ConnectionGos.Count; i++)
        {
            if(ConnectionGos[i] != null && Connections[i].IsConnected == false)
            {
                GameObject conGo = ConnectionGos[i];
                PeerConnection pc = conGo.transform.GetChild(0).gameObject.GetComponent<PeerConnection>();
                if(pc.Peer.IsConnected == true)
                {
                    Connections[i].SetConnectedText();
                    Connections[i].IsConnected = true;
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
    public bool IsConnected = false;
    private ConnectionHandler _conHandler;
    private PeerConnection _peerConnection;


    public event Notify PcConnected;


    public Connection(string senderId, string receiverId, string status, int index, ConnectionHandler conHandler)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Status = status;
        Index = index;
        _conHandler = conHandler;
    }

    public void ActivateConnection()
    {
        _conHandler.InstanziateConnection(this);
    }

    public void DeactivateConnection()
    {
        _conHandler.Deconnect(this);
    }

    public void SetPeerConnection(PeerConnection pC)
    {
        _peerConnection = pC;
        _peerConnection.Peer.Connected += SetConnectedText;
    }

    public virtual void SetConnectedText()
    {
        Debug.Log("Hello Event sth happend");
        PcConnected?.Invoke();
    }
}
