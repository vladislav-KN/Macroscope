using System.Buffers;

namespace Player
{
    public interface IStream
    {
        public void WriteImage(System.DateTime time, byte[] imageBytes);
    }
}