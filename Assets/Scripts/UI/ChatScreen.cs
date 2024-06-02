using UnityEngine.UI;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    static int consoleMessageOrder = 1;

    /// <summary>
    /// Initializes the ChatScreen by setting up the input message event listener and deactivating the game object.
    /// </summary>
    protected override void Initialize()
    {
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles the event when the input field editing ends. Sends the message to the server or broadcasts it if the client is the server.
    /// </summary>
    /// <param name="str">The input message string.</param>
    private void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {
            string name = NetworkManager.Instance.networkEntity.userName + ": ";
            str = name + str;

            NetMessage netMessage = new NetMessage(MessagePriority.NonDisposable, str.ToCharArray());
            netMessage.MessageOrder = consoleMessageOrder;
            consoleMessageOrder++;

            if (NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.GetNetworkServer().Broadcast(netMessage.Serialize());
                messages.text += str + System.Environment.NewLine;
            }
            else
            {
                NetworkManager.Instance.GetNetworkClient().SendToServer(netMessage.Serialize());
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}