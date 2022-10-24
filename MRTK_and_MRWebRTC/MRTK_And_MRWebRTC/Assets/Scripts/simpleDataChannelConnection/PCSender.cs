using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.WebRTC;
using System.Text;

public class PCSender : MonoBehaviour
{
    public Microsoft.MixedReality.WebRTC.Unity.PeerConnection pC;
    public Camera cam;

    private DataChannel dataDummy;
    private DataChannel data2;

    private bool IsData2Open = false;

    public void CreateChannels()
    {
        pC.Peer.DataChannelAdded += this.OnDataChannelAdded;
        pC.Peer.AddDataChannelAsync(40, "dummy", true, true).Wait();
    }

    public void CreateChannelWhileRunningConnection()
    {
        pC.Peer.AddDataChannelAsync(42, "dataChannel1", true, true).Wait();
    }

    private void OnDataChannelAdded(DataChannel channel)
    {
        Debug.Log("Hello " + channel.Label);
        switch (channel.Label)
        {
            case "dummy":
                dataDummy = channel;
                dataDummy.StateChanged += this.OnStateChangedDummy;
                break;
        }
    }
    private void OnStateChangedDummy()
    {
        Debug.Log("DataDummy: " + dataDummy.State);

        if (dataDummy.State + "" == "Open")
        {
            dataDummy.SendMessage(Encoding.ASCII.GetBytes("Hello over there... from Dummy"));
            Debug.Log("Message sended... from Dummy");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsData2Open)
        {
            data2.SendMessage(Encoding.ASCII.GetBytes("Hello over there... from data2"));
            Debug.Log("Message sended... from data2");

            Task<Microsoft.MixedReality.WebRTC.PeerConnection.StatsReport> reportTask = pC.Peer.GetSimpleStatsAsync();
            reportTask.Wait();
            Microsoft.MixedReality.WebRTC.PeerConnection.StatsReport report = reportTask.Result;

            Debug.Log(report.ToString());
            
        }

        if(dataDummy != null)
        {
            if (dataDummy.State == DataChannel.ChannelState.Open)
            {
                dataDummy.SendMessage(Encoding.ASCII.GetBytes(cam.transform.rotation + "|" + cam.transform.position));
            }
        }
        
    }
}
