namespace CRSF
{
    public interface ICRSFPackage{
        void Decode(byte[] data);
        byte[] Encode();
    }
}