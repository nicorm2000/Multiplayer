
namespace Net
{
    /// <summary>
    /// Represents a network object with an ID and owner ID.
    /// </summary>
    public class NetObj
    {
        int id;
        int ownerId;

        /// <summary>
        /// Initializes a new instance of the NetObj class with the specified ID and owner ID.
        /// </summary>
        /// <param name="netObjId">The network object ID.</param>
        /// <param name="ownerId">The owner's ID.</param>
        public NetObj(int netObjId, int ownerId)
        {
            id = netObjId;
            this.ownerId = ownerId;
        }

        /// <summary>
        /// Sets the ID and owner ID of the network object.
        /// </summary>
        /// <param name="id">The new ID.</param>
        /// <param name="ownerId">The new owner ID.</param>
        public void SetValues(int id, int ownerId)
        {
            this.id = id;
            this.ownerId = ownerId;
        }

        /// <summary>
        /// Gets the owner ID of the network object.
        /// </summary>
        public int OwnerId
        {
            get { return ownerId; }
        }

        /// <summary>
        /// Gets the ID of the network object.
        /// </summary>
        public int ID
        {
            get { return id; }
        }
    }
}