using System;
using UnityEngine;

using Microsoft.MixedReality.WebRTC;

/// <summary>
/// 
/// </summary>
public class InputSender : MonoBehaviour
{
    private Sender sender;
    private InputRemoting senderInput;
    private IDisposable suscriberDisposer;

    private DataChannel _channel;

    /// <summary>
    ///
    /// </summary>
    /// <param name="track"></param>
    public void SetChannel(string connectionId, DataChannel channel)
    {
        if (channel == null)
        {
            Dispose();
        }
        else
        {
            sender = new Sender();
            _channel = channel;
            senderInput = new InputRemoting(sender);
            suscriberDisposer = senderInput.Subscribe(new Observer(_channel));
            _channel.StateChanged += OnStateChanged;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size">Texture Size.</param>
    /// <param name="region">Region of the texture in world coordinate system.</param>
    public void SetInputRange(Rect region, Vector2Int size)
    {
        sender.SetInputRange(region, new Rect(Vector2.zero, size));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enabled"></param>
    public void EnableInputPositionCorrection(bool enabled)
    {
        sender.EnableInputPositionCorrection = enabled;
    }

    void OnStateChanged()
    {
        switch(_channel.State)
        {
            case DataChannel.ChannelState.Open:
                OnOpen();
                break;
            case DataChannel.ChannelState.Closed:
                OnClose();
                break;
        }
    }

    void OnOpen()
    {
        senderInput.StartSending();
    }
    void OnClose()
    {
        senderInput.StopSending();
    }

    protected virtual void OnDestroy()
    {
        this.Dispose();
    }

    protected void Dispose()
    {
        senderInput?.StopSending();
        suscriberDisposer?.Dispose();
        sender?.Dispose();
        sender = null;
    }
}

