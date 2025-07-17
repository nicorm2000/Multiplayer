using NetworkServer;
using UnityEngine;
using Net;


public class NetworkServerUnity : MonoBehaviour
{
    NetworkManager nm;
    public void SendWinner(byte [] data)
    {
        NetWinnerMessage netWin = new(data);
        DisconnectAll disconnectAll = new();
        NetDisconnectionMessage netDisconnectionMessage = new(disconnectAll);
        nm.networkEntity.SendMessage(netDisconnectionMessage.Serialize());
    }

    public void SendDisconnectAll(byte[] data)
    {
        //NetObjFactory.RemoveAllINetObject();
        //for (int i = 0; i < players.Count; i++)
        //{
        //    RemoveClient(i);
        //}
        //CloseConnection();
    }

    public void SendDisconnection(byte[] data)
    {
        //NetIDMessage netDisconnection = new(data);
        //int playerID = netDisconnection.GetData();
        //RemoveClient(playerID);
    }

    public void SendError(byte[] data)
    {
        //NetErrorMessage netErrorMessage = new NetErrorMessage(data);
        //CloseConnection();
    }

    public void SendDestroyNetObjects(byte[] data)
    {
        NetDestroyGO netDestroyGO = new NetDestroyGO(data);
        int playerId = netDestroyGO.GetData().Item1;
        int instanceId = netDestroyGO.GetData().Item2;

        NetObjTracker.RemoveNetObj(playerId, instanceId);
    }
}