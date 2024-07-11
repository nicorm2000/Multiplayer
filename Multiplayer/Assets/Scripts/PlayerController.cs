using Net;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, INetObj
{
    [Serializable]
    public class TestingClass
    {
        [NetVariable(0)] public TestingClass2 testInt = new();
    }
    [Serializable]
    public class TestingClass2
    {
        [NetVariable(0)] public TestingClass3 testInt = new();
    }
    [Serializable]
    public class TestingClass3
    {
        [NetVariable(0)] public int testInt = 0;
    }

    private class TestingClass4
    {
        [NetVariable(0)] public int testInt = 0;
        public TestingClass2 testIntClass2 = new();

        public TestingClass4(int testingInt, TestingClass2 testingClass2)
        {
            this.testInt = testingInt;
            this.testIntClass2 = testingClass2;
        }
    }

    public struct TestingStruct
    {
        [NetVariable(0)] public int testInt;
    }

    [SerializeField] public float health = 3;
    //[NetVariable(1)] public bool myBool = false;
    //[NetVariable(2)] public string myString = "pepe";
    //[NetVariable(3)] public char myChar = 'a';
    //[NetVariable(4)] public decimal myDecimal = 1;
    //[NetVariable(5)] public double myDouble = 1;
    //[NetVariable(6)] public short myShort = 1;
    //[NetVariable(7)] public ushort myUShort = 1;
    //[NetVariable(8)] public int myInt = 1;
    //[NetVariable(9)] public uint myUInt = 1;
    //[NetVariable(10)] public long myLong = 1;
    //[NetVariable(11)] public ulong myULong = 1;
    //[NetVariable(12)] public byte myByte = 1;
    //[NetVariable(13)] public sbyte mySByte = 1;
    //[NetVariable(14)] public List<int> test = new() { 0, 1, 2, 3, 4, 5, 6, 50 };
    //[NetVariable(15)] public TestingClass testing = new();
    //[NetVariable(16)] public TestingStruct testingStruct = new() { testInt = 0 };
    //[NetVariable(17)] public int[] myArray = new int[2];
    [NetVariable(18)] TestingClass4 testingClass4;
    [SerializeField] TowerTurns towerTurns;
    [SerializeField] TankMovement movement;
    [SerializeField] Transform cameraPivot;

    public bool currentPlayer = false;
    public int clientID = -1;
    private int testing4ConstructorInt = 0;
    private TestingClass2 testing4ConstructorClass = new();

    NetObj netObj = new(-1, -1);

    NetworkManager nm;

    [ContextMenu("Create TestingClass4")]
    private void TestCreator()
    {
        if (testingClass4 == null)
        {
            testingClass4 = new TestingClass4(testing4ConstructorInt, testing4ConstructorClass);
        }
    }

    [ContextMenu("Destroy TestingClass4")]
    private void TestDestroyer()
    {
        if (testingClass4 != null)
        {
            testingClass4 = null;
        }
    }

    private void Start()
    {
        nm = NetworkManager.Instance;
        if (currentPlayer)
        {
            Camera.main.gameObject.GetComponent<CameraOrbit>().SetFollowObject(cameraPivot);
        }
        //myArray[0] = 1;
        //myArray[1] = 2;

        testingClass4 = null;
    }

    private void Update()
    {
        if (testingClass4 != null)
        {
            Debug.Log($"Client {clientID} testScript IS NOT NULL");
        }
        else
        {
            Debug.Log($"Client {clientID} testScript IS NULL");
        }

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

