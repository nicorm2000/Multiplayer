﻿using UnityEngine.UI;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    private static int consoleMessageOrder = 0;

    protected override void Initialize()
    {
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);
    }

    private void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {
            string name = NetworkManager.Instance.userName + ": ";
            str = name + str;

            NetMessage netMessage = new (MessagePriority.Sorteable | MessagePriority.NonDisposable, str.ToCharArray());
            netMessage.MessageOrder = consoleMessageOrder;
            consoleMessageOrder++;

            if (NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.Broadcast(netMessage.Serialize());
                messages.text += str + System.Environment.NewLine;
            }
            else
            {
                NetworkManager.Instance.SendToServer(netMessage.Serialize());
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}