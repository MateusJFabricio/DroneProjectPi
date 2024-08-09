namespace CRSF
{
    public interface ICRSFPackage{
        public void Decode(byte[] data);
        public byte[] Encode();
    }
}