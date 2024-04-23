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
    }

    void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {
            if (NetworkManager.Instance.isServer)
            {
                NetConsole netConsole = new NetConsole(str.ToCharArray());
                NetworkManager.Instance.Broadcast(netConsole.Serialize());
                messages.text += str + System.Environment.NewLine;
            }
            else
            {
                NetConsole netConsole = new NetConsole(str.ToCharArray());
                NetworkManager.Instance.SendToServer(netConsole.Serialize());
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}