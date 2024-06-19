﻿using Net;
using System.Net;
using UnityEngine.UI;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public Text messages;
    public InputField inputMessage;

    static int consoleMessageOrder = 1;
    protected override void Initialize()
    {
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);
    }

    void OnEndEdit(string str)
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
                NetworkManager.Instance.networkEntity.SendMessage(netMessage.Serialize());
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
