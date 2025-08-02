using System.Collections.Generic;
using System;

namespace Net
{
    /// <summary>
    /// Provides factory methods for creating and managing network objects.
    /// </summary>
    public static class NetObjFactory
    {
        static NetworkEntity entity;
        static readonly Dictionary<int, INetObj> NetObjectsInstances = new Dictionary<int, INetObj>();
        public static Action<byte[], int> OnDataSend;

        /// <summary>
        /// Gets a list of all active network objects.
        /// </summary>
        /// <returns>A list of INetObj instances.</returns>
        public static List<INetObj> NetObjects()
        {
            List<INetObj> aux  = new List<INetObj>();
            foreach (INetObj obj in NetObjectsInstances.Values)
            {
                if (obj != null)
                    aux.Add(obj);
            }
            return aux;
        }

        /// <summary>
        /// Gets the count of network objects.
        /// </summary>
        public static int NetObjectsCount{ get; private set; }

        /// <summary>
        /// Sets the network entity for sending messages.
        /// </summary>
        /// <param name="networkEntity">The network entity to use.</param>
        public static void SetNetworkEntity(NetworkEntity networkEntity)
        {
            entity = networkEntity;
        }

        /// <summary>
        /// Adds a network object to the factory.
        /// </summary>
        /// <param name="key">The key to associate with the object.</param>
        /// <param name="netObj">The network object to add.</param>
        public static void AddINetObject(int key, INetObj netObj)
        {
            NetObjectsInstances[key] = netObj;
        }

        /// <summary>
        /// Removes a network object from the factory.
        /// </summary>
        /// <param name="key">The key of the object to remove.</param>
        public static void RemoveINetObject(int key)
        {
            if (NetObjectsInstances.ContainsKey(key))
            {
                NetObjectsInstances.Remove(key);
            }
        }

        /// <summary>
        /// Removes all network objects from the factory.
        /// </summary>
        public static void RemoveAllINetObject()
        {
            NetObjectsInstances.Clear();
        }

        /// <summary>
        /// Gets a network object by its key.
        /// </summary>
        /// <param name="key">The key of the object to retrieve.</param>
        /// <returns>The network object, or null if not found.</returns>
        public static INetObj GetINetObject(int key)
        {
            if (NetObjectsInstances.ContainsKey(key))
            {
                return NetObjectsInstances[key];
            }

            return null;
        }

        /// <summary>
        /// Creates and sends a network instance request.
        /// </summary>
        /// <param name="gameObjectToIntanciateID">The ID of the game object to instantiate.</param>
        /// <param name="positionX">The X position.</param>
        /// <param name="positionY">The Y position.</param>
        /// <param name="positionZ">The Z position.</param>
        /// <param name="rotationX">The X rotation.</param>
        /// <param name="rotationY">The Y rotation.</param>
        /// <param name="rotationZ">The Z rotation.</param>
        /// <param name="rotationW">The W rotation.</param>
        /// <param name="scaleX">The X scale.</param>
        /// <param name="scaleY">The Y scale.</param>
        /// <param name="scaleZ">The Z scale.</param>
        /// <param name="parentGameObjectID">The parent game object ID.</param>
        /// <param name="owner">The owner ID.</param>
        public static void NetInstance(int gameObjectToIntanciateID, float positionX, float positionY, float positionZ,
                                                                     float rotationX, float rotationY, float rotationZ, float rotationW,
                                                                     float scaleX, float scaleY, float scaleZ,
                                                                     int parentGameObjectID, int owner = -1)
        {
            InstanceRequestPayload instanceRequestPayload = new InstanceRequestPayload(gameObjectToIntanciateID,
                                                                       positionX, positionY, positionZ,
                                                                       rotationX, rotationY, rotationZ, rotationW,
                                                                       scaleX, scaleY, scaleZ,
                                                                       parentGameObjectID);

            InstanceRequestMenssage instanceRequest = new InstanceRequestMenssage(MessagePriority.NonDisposable, instanceRequestPayload);
            byte[] dataToSend = instanceRequest.Serialize();
            OnDataSend?.Invoke(dataToSend, owner);
            entity.SendMessage(dataToSend);
            NetObjectsCount++;
        }
    }
}