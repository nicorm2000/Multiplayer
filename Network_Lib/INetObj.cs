namespace Net
{
    public class TRS
    {
        public (float, float, float) position;
        public (float, float, float, float) rotation;
        public (float, float, float) scale;
        public bool isActive;
    }

    public interface INetObj
    {
        int GetID();

        int GetOwnerID();

        NetObj GetNetObj();

        TRS GetTRS();

        void SetTRS(TRS trs, NetTRS.SYNC sync);
    }
}