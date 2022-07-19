namespace Natx.Abstraction;

public interface IEncodedConnection
{
    object Decode(byte[] data, Type type);
    byte[] Encode(object data);
}
