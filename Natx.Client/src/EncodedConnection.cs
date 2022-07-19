using NATS.Client;

namespace Natx.Client;

public sealed class EncodedConnection
{
    public IConnection Connection { get; }
    private Natx.Abstraction.IEncodedConnection encodedConnection;
    public EncodedConnection(Natx.Abstraction.IEncodedConnection encodedConnection)
    {
        Connection = new ConnectionFactory().CreateConnection();
        this.encodedConnection = encodedConnection;
    }
    public void Publish(string subject, object data)
    {
        byte[] bytes = encodedConnection.Encode(data);
        Connection.Publish(subject, bytes);
    }
    public void Publish(Msg msg)
    {
        msg.Message.Data = encodedConnection.Encode(msg.Data);
        Connection.Publish(msg.Message);
    }
    public void Publish(string subject, string reply, object data)
    {
        byte[] bytes = encodedConnection.Encode(data);
        Connection.Publish(subject, reply, bytes);
    }
    public Msg Request(string subject, object data)
    {
        byte[] bytes = encodedConnection.Encode(data);
        NATS.Client.Msg msg = Connection.Request(subject, bytes);
        return new Msg(msg, encodedConnection.Decode(msg.Data));
    }
    public Msg Request(Msg msg)
    {
        msg.Message.Data = encodedConnection.Encode(msg.Data);
        NATS.Client.Msg response = Connection.Request(msg.Message);
        return new Msg(response, encodedConnection.Decode(response.Data));
    }
    public Msg Request(Msg msg, TimeSpan timeout)
    {
        msg.Message.Data = encodedConnection.Encode(msg.Data);
        NATS.Client.Msg response = Connection.Request(msg.Message, (int)timeout.TotalMilliseconds);
        return new Msg(response, encodedConnection.Decode(response.Data));
    }
    public Msg Request(string subject, object data, TimeSpan timeout)
    {
        byte[] bytes = encodedConnection.Encode(data);
        NATS.Client.Msg msg = Connection.Request(subject, bytes, (int)timeout.TotalMilliseconds);
        return new Msg(msg, encodedConnection.Decode(msg.Data));
    }
    public SyncSubscription SubscribeSync(string subject)
    {
        ISyncSubscription syncSubscription = Connection.SubscribeSync(subject);
        return new SyncSubscription(encodedConnection, syncSubscription);
    }
    public SyncSubscription SubscribeSync(string subject, string queue)
    {
        ISyncSubscription syncSubscription = Connection.SubscribeSync(subject, queue);
        return new SyncSubscription(encodedConnection, syncSubscription);
    }
    public AsyncSubscription SubscribeAsync(string subject)
    {
        IAsyncSubscription asyncSubscription = Connection.SubscribeAsync(subject);
        return new AsyncSubscription(encodedConnection, asyncSubscription);
    }
    public AsyncSubscription SubscribeAsync(string subject, string queue)
    {
        IAsyncSubscription asyncSubscription = Connection.SubscribeAsync(subject, queue);
        return new AsyncSubscription(encodedConnection, asyncSubscription);
    }
}