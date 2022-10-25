using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.WebRTC.Unity;
using Microsoft.MixedReality.WebRTC;

public class test : MonoBehaviour
{
    public Microsoft.MixedReality.WebRTC.Unity.PeerConnection pc;



    // Start is called before the first frame update
    void Start()
    {

    }

    public void testFunk()
    {
        Task <Microsoft.MixedReality.WebRTC.DataChannel> task0 = pc.Peer.AddDataChannelAsync(69, "testFromOutside", true, true);
        task0.Wait();

        pc.Peer.DataChannelAdded += anotherDataChannelAddedEvent;
    }

    private void anotherDataChannelAddedEvent(DataChannel channel)
    {
        Debug.Log("DataChannel Event fired from outside of PeerConnection");
        Debug.Log(channel.State);
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
