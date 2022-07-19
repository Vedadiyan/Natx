namespace Natx.Abstraction;

public interface IEncodedConnection
{
    object Decode(byte[] data);
    byte[] Encode(object data);
}
