using UnityEngine;
using Net;
namespace Game
{

    public class bulletBehaviour : MonoBehaviour, INetObj
    {
        [SerializeField] float velocity;
        [SerializeField] float gravity = 9.8f;  // Valor de la gravedad
        private Vector3 velocityVector;

        int originPlayerID = -1;

        NetObj netObj = new NetObj(-1, -1);

        NetworkManager nm;

        private void Start()
        {
            nm = NetworkManager.Instance;

            Destroy(gameObject, 5.0f);

            velocityVector = transform.forward * velocity;
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
            if (collision.transform.TryGetComponent(out PlayerController pc))
            {
                if (pc.clientID != originPlayerID)
                {
                    pc.health--;
                    if (pc.health <= 0)
                    {
                        NetIDMessage netDisconnection = new NetIDMessage(MessagePriority.Default, pc.clientID);
                        nm.networkEntity.SendMessage(netDisconnection.Serialize());
                        nm.networkEntity.RemoveClient(pc.clientID);
                    }
                }
            }

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
    }
}