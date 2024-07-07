using Net;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, INetObj
{
    //[NetVariable(0)] List<int> test = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 50 };
    //[NetVariable(0), SerializeField] RouteInfo route = new RouteInfo(0);
    [NetVariable(0)] public float health = 3;
    [NetVariable(1)] public bool myBool = false;
    [NetVariable(2)] public string myString = "pepe";
    [NetVariable(3)] public char myChar = 'a';
    [NetVariable(4)] public decimal myDecimal = 1;
    [NetVariable(5)] public double myDouble = 1;
    [NetVariable(6)] public short myShort = 1;
    [NetVariable(7)] public ushort myUShort = 1;
    [NetVariable(8)] public int myInt = 1;
    [NetVariable(9)] public uint myUInt = 1;
    [NetVariable(10)] public long myLong = 1;
    [NetVariable(11)] public ulong myULong = 1;
    [NetVariable(12)] public byte myByte = 1;
    [NetVariable(13)] public sbyte mySByte = 1;
    [SerializeField, /*NetVariable(1)*/] TowerTurns towerTurns;
    [SerializeField, /*NetVariable(2)*/] TankMovement movement;
    [SerializeField] Transform cameraPivot;

    public bool currentPlayer = false;
    public int clientID = -1;

    NetObj netObj = new(-1, -1);

    NetworkManager nm;

    private void Start()
    {
        nm = NetworkManager.Instance;

        if (currentPlayer)
        {
            Camera.main.gameObject.GetComponent<CameraOrbit>().SetFollowObject(cameraPivot);
        }
    }

    private void Update()
    {
        if (health <= 0)
        {
            Debug.Log(clientID + " died");
            NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, clientID);
            nm.networkEntity.SendMessage(netDisconnection.Serialize());
            nm.networkEntity.RemoveClient(clientID);
        }
    }

    public void OnReciveDamage()
    {
        health--; 
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

