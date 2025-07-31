using UnityEngine;
using NetworkServer;
using System;
using Net;

namespace Game
{
    [NetTRS(NetTRS.SYNC.DEFAULT, NETAUTHORITY.CLIENT)]
    public class bulletBehaviour : MonoBehaviour, INetObj
    {
        [SerializeField] float velocity;
        [SerializeField] float gravity = 9.8f;
        private Vector3 velocityVector;

        int originPlayerID = -1;

        NetObj netObj = new NetObj(-1, -1);

        NetworkManager nm;

        private Action onEventA;
        [NetEvent(0, NETAUTHORITY.CLIENT)]
        public event Action OnEventA
        {
            add => onEventA += value;
            remove => onEventA -= value;
        }

        private Action<int> onEventB;
        [NetEvent(1, NETAUTHORITY.CLIENT)]
        public event Action<int> OnEventB
        {
            add => onEventB += value;
            remove => onEventB -= value;
        }

        private Action<string, float> shootTriggered;

        [NetEvent(2, NETAUTHORITY.CLIENT, backingFieldName: "shootTriggered")]
        public event Action<string, float> OnEventC
        {
            add => shootTriggered += value;
            remove => shootTriggered -= value;
        }

        private void OnEnable()
        {
#if SERVER
            originPlayerID = netObj.OwnerId;
#endif
        }

        private void Start()
        {
#if SERVER
            Invoke(nameof(DestroyBehaviour), 5f);
#endif
            nm = NetworkManager.Instance;
            velocityVector = transform.forward * velocity;
#if ClIENT
netObj.OwnerId = originPlayerID;
#endif

            OnEventA += () => Debug.Log("C# Event: OnEventA triggered! " + netObj.OwnerId);
            OnEventB += (value) => Debug.Log($"C# Event: OnEventB({value}) triggered!");
            OnEventC += (text, weight) => Debug.Log($"C# Event: OnEventC(\"{text}\", {weight}) triggered!");

            Debug.Log("Shoot Game");
            ReflectionSystem.Instance.reflection.reflectionCallInvoker.SendMethodMessage(this, nameof(TestMR));
            ReflectionSystem.Instance.reflection.reflectionCallInvoker.SendMethodMessage(this, nameof(TestMRB), false);
            ReflectionSystem.Instance.reflection.reflectionCallInvoker.SendMethodMessage(this, nameof(TestMRI), 3);

            ReflectionSystem.Instance.reflection.reflectionCallInvoker.SendCSharpEventMessage(this, nameof(OnEventA));
            ReflectionSystem.Instance.reflection.reflectionCallInvoker.SendCSharpEventMessage(this, nameof(OnEventB), GetID());
            ReflectionSystem.Instance.reflection.reflectionCallInvoker.SendCSharpEventMessage(this, nameof(OnEventC), "test", 4.2f);

            Debug.Log("Bullet owner: " + GetOwnerID());
        }

        [NetMethod(0, NETAUTHORITY.CLIENT)]
        private void TestMR()
        {
            Debug.Log("Funca");
        }

        [NetMethod(1, NETAUTHORITY.CLIENT)]
        private void TestMRB(bool a)
        {
            Debug.Log("a: " + a);
        }

        [NetMethod(2, NETAUTHORITY.CLIENT)]
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
#if SERVER
            if (collision.transform.TryGetComponent(out PlayerController pc))
            {
                GameManager.OnBulletHit.Invoke(pc.clientID, originPlayerID);
            }
            DestroyBehaviour();
#endif
        }

        private void DestroyBehaviour()
        {
            NetDestroyGO netDestroyGO = new NetDestroyGO(MessagePriority.Default, (GetID(), originPlayerID));
            nm.networkEntity.SendMessage(netDestroyGO.Serialize());
            Debug.Log($"Origin & ID:  {GetOwnerID() } & { GetID()}");
            NetObjFactory.RemoveINetObject(GetID());
            Destroy(gameObject);
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