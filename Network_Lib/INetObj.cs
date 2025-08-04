namespace Net
{
    /// <summary>
    /// Represents a Transform with position, rotation, scale, and active state.
    /// </summary>
    public class TRS
    {
        public (float, float, float) position;
        public (float, float, float, float) rotation;
        public (float, float, float) scale;
        public bool isActive;
    }

    /// <summary>
    /// Represents an INetObj that will be used to operate different net actions.
    /// </summary>
    public interface INetObj
    {
        int GetID();

        int GetOwnerID();

        NetObj GetNetObj();

        TRS GetTRS();

        void SetTRS(TRS trs, NetTRS.SYNC sync);
    }
}