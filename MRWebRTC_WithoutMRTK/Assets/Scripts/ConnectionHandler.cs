using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;

public class ConnectionHandler : MonoBehaviour
{
    
    
    public List<GameObject> Connections;

    public GameObject ConnectionPrefab;

    public GameObject Model;

    public GameObject ConnectionIPInput;

    public GameObject ConnectionThumbPrefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddConnection()
    {
        GameObject newConnection = Instantiate(ConnectionPrefab);
        Connections.Add(newConnection);
        DataChannelReceiver newConnectionDataChannelReceiver = newConnection.transform.GetChild(0).gameObject.GetComponent<DataChannelReceiver>();
        newConnectionDataChannelReceiver.model = Model;

        NodeDssSignaler newConNodeDSSSignaler = newConnection.transform.GetChild(1).gameObject.GetComponent<NodeDssSignaler>();
        newConNodeDSSSignaler.LocalPeerId = "sender" + (Connections.Count - 1);
        newConNodeDSSSignaler.RemotePeerId = "receiver" + (Connections.Count - 1);
        newConNodeDSSSignaler.HttpServerAddress = "http://" + ConnectionIPInput.GetComponent<TMPro.TMP_InputField>().text + ":3000/";
    }
}
