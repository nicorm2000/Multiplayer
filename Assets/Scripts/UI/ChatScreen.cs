using UnityEngine.UI;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    private static int consoleMessageOrder = 0;

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
            string name = NetworkManager.Instance.userName + ": ";
            str = name + str;

            NetMessage netMessage = new (MessagePriority.Sortable | MessagePriority.NonDisposable, str.ToCharArray())
            {
                MessageOrder = consoleMessageOrder
            };
            consoleMessageOrder++;

            // If this client is the server, broadcast the message to all clients
            if (NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.Broadcast(netMessage.Serialize());
                messages.text += str + System.Environment.NewLine;
            }
            else
            {
                // Otherwise, send the message to the server
                NetworkManager.Instance.SendToServer(netMessage.Serialize());
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}