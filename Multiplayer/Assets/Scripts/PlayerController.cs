using Net;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, INetObj
{
    //[NetVariable(0)] List<int> test = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 50 };
    [NetVariable(0), SerializeField] RouteInfo route = new RouteInfo(0);
    /*[NetVariable(0)]*/
    public float health = 3;
    [SerializeField, /*NetVariable(1)*/] TowerTurns towerTurns;
    [SerializeField, /*NetVariable(2)*/] TankMovement movement;
    [SerializeField] Transform cameraPivot;
    
    public bool currentPlayer = false;
    public int clientID = -1;

    NetObj netObj = new(-1, -1);

    NetworkManager nm;

    static int positionMessageOrder = 1;
    static int bulletsMessageOrder = 1;

    private void Start()
    {
        nm = NetworkManager.Instance;

        if (currentPlayer)
        {
            Camera.main.gameObject.GetComponent<CameraOrbit>().SetFollowObject(cameraPivot);
        }
    }

    public void OnReciveDamage() //Solo lo maneja el server esta funcion
    {
        health--;

        if (health <= 0)
        {
            NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
            nm.networkEntity.SendMessage(netDisconnection.Serialize());
            nm.networkEntity.RemoveClient(clientID);
        }
    }

    public int GetID()
    {
        return netObj.ID;
    }

    public int GetOwnerID()
    {
        return netObj.OwnerId;
    }

    public NetObj GetNetObj()
    {
        return netObj;
    }
}

