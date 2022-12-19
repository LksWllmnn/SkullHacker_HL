using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConnectionThumbScript : MonoBehaviour
{
    public TextMeshProUGUI SenderIDText;
    public TextMeshProUGUI ReceiverIDText;
    public TextMeshProUGUI ConnectionStatusText;
    public TextMeshProUGUI ButtonText;
    public Image BackgroundImage;

    [HideInInspector] public string SenderId;
    [HideInInspector] public string ReceiverId;
    [HideInInspector] public string ConnectionStatus;
    [HideInInspector] public Connection Con;

    private bool _isConnected = false;


    public void SetConStatTextConnected()
    {
        switch(Con.GetState())
        {
            case ConnectionState.Connected:
                SetConnectionStatusText("Connected");
                break;
            case ConnectionState.Open:
                SetConnectionStatusText("Open");
                break;
            case ConnectionState.Closed:
                SetConnectionStatusText("Closed");
                break;
        }
    }

    public void SetSenderText(string text)
    {
        SenderIDText.text = text;
    }

    public void SetReceiverText(string text)
    {
        ReceiverIDText.text = text;
    }

    public void SetConnectionStatusText(string text)
    {
        ConnectionStatusText.text = text;
        switch (text)
        {
            case "Connected":
                ConnectionStatusText.color = Color.green;
                break;
            case "Open":
                ConnectionStatusText.color = Color.yellow;
                break;
            case "Closed":
                ConnectionStatusText.color = Color.red;
                break;
        }
    }

    public void SetButtonText(string text)
    {
        ButtonText.text = text;
        
    }

    public void SetThisConnection(Connection con)
    {
        Con = con;
        Con.PcConnected += SetConStatTextConnected;
    }

    public void ToggleConnectionState()
    {
        if (_isConnected) DeactivateConnection();
        else
        {
            ActivateConnection();
        }
    }

    private void ActivateConnection()
    {
        Con.ActivateConnection();
        SetButtonText("deactivate");
        SetConnectionStatusText("Open");
        BackgroundImage.color = new Color(1, 1, 1, 1);
        _isConnected = true;
    }

    private void DeactivateConnection()
    {
        Con.DeactivateConnection();
        SetButtonText("activate");
        BackgroundImage.color = new Color(1, 1, 1, 0.2f);
        _isConnected = false;
    }
} 
