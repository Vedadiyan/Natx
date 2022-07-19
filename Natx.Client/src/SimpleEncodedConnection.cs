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

public class SearchReq {}
public class SearchRes {}

public delegate SearchRes Search(SearchReq searchReq);

public class SearchServer
{
    private readonly EncodedConnection encodedConnection;
    private readonly AsyncSubscription subscription;
	private readonly Search search; 
    public SearchServer(EncodedConnection encodedConnection, Search search)
    {
        this.encodedConnection = encodedConnection;
        subscription = encodedConnection.SubscribeAsync("internal.hotel.search", "abc", typeof(SearchRes));
        this.search = search;
        subscription.MessageHandler += onMessage;
    }
    private void onMessage(object? sender, MsgHandlerEventArgs e)
    {
        try
        {
            SearchRes response = search((SearchReq) e.Message.Data);
            e.Message.Respond(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public void Start()
    {
        subscription.Start();
    }
    public void Stop()
    {
        subscription.Unsubscribe();
    }
}


public class SearchClient
{
    private readonly EncodedConnection encodedConnection;
	private readonly TimeSpan timeout;
    public SearchClient(EncodedConnection encodedConnection, TimeSpan timeout)
    {
        this.encodedConnection = encodedConnection;
		this.timeout = timeout;
    }
    public SearchRes Request(SearchReq request)
    {
		Msg msg = encodedConnection.Request("internal.hotel.search", request, timeout, typeof(SearchRes));
        return (SearchRes) msg.Data;
    }
}

