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
        [NetVariable(1)] public int testInt2;
        [NetVariable(2)] public int testInt3;
    }
    
    [NetVariable(0)] public float health = 3;
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
    //[NetVariable(16)] public TestingStruct testingStruct = new() { testInt = 0, testInt2 = 0, testInt3 = 0 };
    //[NetVariable(17)] public int[] myArray = new int[2];
    //[NetVariable(18)] TestingClass4 testingClass4;
    //[NetVariable(19)] public List<TestingClass3> jajaxd = null;
    //[NetVariable(20)] public TestingClass3 testingclass3 = new();
    //[NetVariable(21)] public Vector2 MyVector2 = Vector2.zero;
    //[NetVariable(22)] public Vector3 MyVector3 = Vector3.zero;
    //[NetVariable(23)] public Vector4 MyVector4 = Vector4.zero;
    //[NetVariable(24)] public Quaternion MyQuaternion = new(0f, 0f, 0f, 1f);
    //[NetVariable(25)] public Color MyColor = new(1f,0.5f,0f,1f);
    //[NetVariable(26)] public Color32 MyColor32 = new(0, 0, 0, 255); // Funca bien pero tira error al principio
    //[NetVariable(27)] public Rect MyRect = new(0, 0, 1, 1); // Problema de escritura, lo lee + error
    //[NetVariable(28)] public Bounds MyBounds = new(Vector3.zero, Vector3.one); // Problema de escritura, lo lee + error
    //[NetVariable(29)] public Matrix4x4 MyMatrix4x4 = new(new Vector4(0,0,0,0), new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 0)); // Funca, pero me mata los FPS
    //[NetVariable(30)] public Plane MyPlane = new(new Vector3(1,2,3), 0); // Problema de escritura, lo lee + error
    //[NetVariable(31)] public Vector2Int MyVector2Int = new (0,0); // Problema de escritura, lo lee + error porque es struct
    //[NetVariable(32)] public Vector3Int MyVector3Int = new (0,0,0); // Problema de escritura, lo lee + error porque es struct
    // Mas jodidos de lo que parecen
    //[NetVariable(33)]
    //public Gradient MyGradient = new Gradient
    //{
    //    colorKeys = new GradientColorKey[]
    //    {
    //    new GradientColorKey(Color.white, 0f),
    //    new GradientColorKey(Color.black, 1f)
    //    },
    //    alphaKeys = new GradientAlphaKey[]
    //    {
    //    new GradientAlphaKey(1f, 0f),
    //    new GradientAlphaKey(1f, 1f)
    //    }
    //};
    //[NetVariable(34)]
    //public AnimationCurve MyAnimationCurve = new AnimationCurve
    //(
    //    new Keyframe(0f, 0f),
    //    new Keyframe(1f, 1f)
    //);
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
        //if (testingClass4 == null)
        //{
        //    testingClass4 = new TestingClass4(testing4ConstructorInt, testing4ConstructorClass);
        //}
    }

    [ContextMenu("Destroy TestingClass4")]
    private void TestDestroyer()
    {
        //if (testingClass4 != null)
        //{
        //    testingClass4 = null;
        //}
    }

    [ContextMenu("Add Decimal")]
    private void AddDecimal()
    {
        //myDecimal += 1;
    }

    [ContextMenu("Substract Decimal")]
    private void SubstractDecimal()
    {
        //myDecimal -= 1;
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
        //
        //testingClass4 = null;
        //
        //testingclass3.testInt = 1;
        //
        //jajaxd = new List<TestingClass3>
        //{
        //    new() { testInt = 7 },
        //    new() { testInt = 17 }
        //};
    }

    private void Update()
    {
        //if (testingClass4 != null)
        //{
        //    //Debug.Log($"Client {clientID} testScript IS NOT NULL");
        //}
        //else
        //{
        //    //Debug.Log($"Client {clientID} testScript IS NULL");
        //}
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    MyPlane.normal += new Vector3 (1,0,1);
        //    MyPlane.distance += 2.67f;
        //    Debug.Log("Normal:" + MyPlane.normal);
        //    Debug.Log("Distance:" + MyPlane.distance);
        //}
        //Debug.Log($"Client {clientID} myDecimal: /*myDecimal*/");
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

