﻿using UnityEngine;
using Net;
using System;

namespace Game
{
    [NetTRS(NetTRS.SYNC.DEFAULT)]
    public class bulletBehaviour : MonoBehaviour, INetObj
    {
        [SerializeField] float velocity;
        [SerializeField] float gravity = 9.8f;
        private Vector3 velocityVector;

        int originPlayerID = -1;

        NetObj netObj = new NetObj(-1, -1);

        NetworkManager nm;

        private Action onEventA;
        [NetEvent(0)]
        public event Action OnEventA
        {
            add => onEventA += value;
            remove => onEventA -= value;
        }

        private Action<int> onEventB;
        [NetEvent(1)]
        public event Action<int> OnEventB
        {
            add => onEventB += value;
            remove => onEventB -= value;
        }

        private Action<string, float> shootTriggered;

        [NetEvent(2, backingFieldName: "shootTriggered")]
        public event Action<string, float> OnEventC
        {
            add => shootTriggered += value;
            remove => shootTriggered -= value;
        }

        private void Start()
        {
            nm = NetworkManager.Instance;
            velocityVector = transform.forward * velocity;

            OnEventA += () => Debug.Log("C# Event: OnEventA triggered!");
            OnEventB += (value) => Debug.Log($"C# Event: OnEventB({value}) triggered!");
            OnEventC += (text, weight) => Debug.Log($"C# Event: OnEventC(\"{text}\", {weight}) triggered!");

            Debug.Log("Shoot Game");
            ReflectionSystem.Instance.reflection.SendMethodMessage(this, nameof(TestMR));
            ReflectionSystem.Instance.reflection.SendMethodMessage(this, nameof(TestMRB), false);
            ReflectionSystem.Instance.reflection.SendMethodMessage(this, nameof(TestMRI), 3);

            ReflectionSystem.Instance.reflection.SendCSharpEventMessage(this, nameof(OnEventA));
            ReflectionSystem.Instance.reflection.SendCSharpEventMessage(this, nameof(OnEventB), 99);
            ReflectionSystem.Instance.reflection.SendCSharpEventMessage(this, nameof(OnEventC), "test", 4.2f);
            
            originPlayerID = netObj.OwnerId;
        }

        [NetMethod(0)]
        private void TestMR()
        {
            Debug.Log("Funca");
        }

        [NetMethod(1)]
        private void TestMRB(bool a)
        {
            Debug.Log("a: " + a);
        }

        [NetMethod(2)]
        private void TestMRI(int a)
        {
            Debug.Log("int: " + a);
        }

        public void SetOwnerID(int clientIdOrigin)
        {
            originPlayerID = clientIdOrigin;
        }

        void Update()
        {
            velocityVector.y -= gravity * Time.deltaTime;
            transform.position += velocityVector * Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            NetDestroyGO netDestroyGO = new NetDestroyGO(MessagePriority.Default, (GetID(), originPlayerID));
            nm.networkEntity.SendMessage(netDestroyGO.Serialize());

            if (collision.transform.TryGetComponent(out PlayerController pc))
            {
                GameManager.OnBulletHit.Invoke(pc.clientID, originPlayerID);
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

        public TRS GetTRS()
        {
            return transform.TranslateTRS();
        }

        public void SetTRS(TRS trs, NetTRS.SYNC sync)
        {
            transform?.FromTRS(trs, sync);
        }
    }
}