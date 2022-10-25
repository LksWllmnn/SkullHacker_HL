using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.WebRTC;
using System.Text;

public class PCReceiver : MonoBehaviour
{
    public Microsoft.MixedReality.WebRTC.Unity.PeerConnection pC;
    public Camera cam;
    public GameObject representationObject;
    public InputSender inputSender;

    private DataChannel dataDummy;
    private DataChannel dataObj;

    private bool setInputChannel = false;

    public void CreateChannels()
    {
        pC.Peer.DataChannelAdded += this.OnDataChannelAdded;
        pC.Peer.AddDataChannelAsync(40, "dummy", false, false).Wait();
        pC.Peer.AddDataChannelAsync(41, "objChannel", false, false).Wait();
    }

    private void OnDataChannelAdded(DataChannel channel)
    {
        Debug.Log("Hello " + channel.Label);
        switch (channel.Label)
        {
            case "dummy":
                dataDummy = channel;
                dataDummy.StateChanged += this.OnStateChangedDummy;
                setInputChannel = true;
                break;

            case "objChannel":
                dataObj = channel;
                dataObj.StateChanged += this.OnStateChangedObj;
                break;
        }
    }
    private void OnStateChangedDummy()
    {
        Debug.Log("DataDummy: " + dataDummy.State);

        if (dataDummy.State == DataChannel.ChannelState.Open)
        {
            dataDummy.SendMessage(Encoding.ASCII.GetBytes("Hello over there... from Dummy"));
            Debug.Log("Message sended... from Dummy");
        }
    }

    private void OnStateChangedObj()
    {
        Debug.Log("DataObj: " + dataObj.State);

        if (dataObj.State == DataChannel.ChannelState.Open)
        {
            dataDummy.SendMessage(Encoding.ASCII.GetBytes(representationObject.transform.position.ToString("F3")));
            Debug.Log("Message sended... from Obj");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (dataDummy != null)
        {
            if (dataDummy.State == DataChannel.ChannelState.Open)
            {
                dataDummy.SendMessage(Encoding.ASCII.GetBytes(cam.transform.rotation.ToString("F3") + "|" + cam.transform.position.ToString("F3")));
            }
        }

        if(setInputChannel)
        {
            inputSender.SetChannel(dataDummy.ID.ToString(), dataDummy);
            setInputChannel = false;
        }

    }
}
