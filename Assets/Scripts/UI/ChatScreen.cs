using System.Net;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    protected override void Initialize()
    {
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);

        NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;
    }

    void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
    {
        NetConsole netConsole = new();

        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(data);
        }

        messages.text += netConsole.Deserialize(data) + System.Environment.NewLine;
    }

    void OnEndEdit(string str)
    {
        NetConsole netConsole = new();
        string message;
        if (inputMessage.text != "")
        {
            if (NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.Broadcast(netConsole.Serialize());
                
                messages.text += inputMessage.text + System.Environment.NewLine;
            }
            else
            {
                NetworkManager.Instance.SendToServer(netConsole.Serialize());
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }

    }

}
