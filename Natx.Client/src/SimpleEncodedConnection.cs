using System.Collections.Concurrent;
using Google.Protobuf;

namespace Natx.Client;

public abstract class SimpleEncodedConnection : EncodedConnection
{
    public SimpleEncodedConnection() : base(new NATS.Client.ConnectionFactory().CreateConnection())
    {

    }
    public SimpleEncodedConnection(string url) : base(new NATS.Client.ConnectionFactory().CreateConnection(url))
    {

    }
    public SimpleEncodedConnection(string url, string credentialPath) : base(new NATS.Client.ConnectionFactory().CreateConnection(url, credentialPath))
    {

    }
    public SimpleEncodedConnection(string url, string jwt, string privateKey) : base(new NATS.Client.ConnectionFactory().CreateConnection(url, jwt, privateKey))
    {

    }
    public SimpleEncodedConnection(NATS.Client.Options options) : base(new NATS.Client.ConnectionFactory().CreateConnection(options))
    {

    }
}

public class Test : SimpleEncodedConnection
{
    private static ConcurrentDictionary<Type, Func<byte[], object?>> typeMap = new ConcurrentDictionary<Type, Func<byte[], object?>>();
    public override object Decode(byte[] data, Type type)
    {
        if (typeMap.ContainsKey(type))
        {
            var parserProperty = type.GetProperty("Parser");
            if (parserProperty == null)
            {
                throw new ArgumentException($"Property `Parser` not found on type `{type.Name}`");
            }
            var parseInstance = parserProperty.GetValue(null);
            var parseFromMethod = parserProperty.PropertyType.GetMethod("ParseFrom", new Type[] { typeof(byte[]) });
            if (parseFromMethod == null)
            {
                throw new ArgumentException($"Method `ParseFrom` not found on type `{type.Name}`");
            }
            Func<byte[], object?> func = (x) => parseFromMethod.Invoke(parseInstance, new object[] { x });
            typeMap.TryAdd(type, func);
        }
        using (ZstdNet.Decompressor decompressor = new ZstdNet.Decompressor())
        {
            var unwrappedData = decompressor.Unwrap(data);
            object? output = typeMap[type](unwrappedData);
            if (output == null)
            {
                throw new Exception($"Failed to deserialize to type `{type.Name}`");
            }
            return output;
        }
    }

    public override byte[] Encode(object data)
    {
        using (MemoryStream mems = new MemoryStream())
        {
            if (!(data is IMessage message))
            {
                return null!;
            }
            message.WriteTo(mems);
            using (ZstdNet.Compressor compressor = new ZstdNet.Compressor())
            {
                return compressor.Wrap(mems.ToArray());
            }
        }
    }
}