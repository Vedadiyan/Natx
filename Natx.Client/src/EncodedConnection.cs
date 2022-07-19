namespace Natx.Client;

public abstract class EncodedConnection : Natx.Abstraction.IEncodedConnection
{
    public NATS.Client.IConnection Connection { get; private protected set; }
    protected EncodedConnection(NATS.Client.IConnection connection)
    {
        Connection = connection;
    }
    public void Publish(string subject, object data)
    {
        byte[] bytes = Encode(data);
        Connection.Publish(subject, bytes);
    }
    public void Publish(Msg msg)
    {
        msg.Message.Data = Encode(msg.Data);
        Connection.Publish(msg.Message);
    }
    public void Publish(string subject, string reply, object data)
    {
        byte[] bytes = Encode(data);
        Connection.Publish(subject, reply, bytes);
    }
    public Msg Request(string subject, object data, Type typeOfResponse)
    {
        byte[] bytes = Encode(data);
        NATS.Client.Msg msg = Connection.Request(subject, bytes);
        return new Msg(msg, Decode(msg.Data, typeOfResponse));
    }
    public Msg Request(Msg msg, Type typeOfResponse)
    {
        msg.Message.Data = Encode(msg.Data);
        NATS.Client.Msg response = Connection.Request(msg.Message);
        return new Msg(response, Decode(response.Data, typeOfResponse));
    }
    public Msg Request(Msg msg, TimeSpan timeout, Type typeOfResponse)
    {
        msg.Message.Data = Encode(msg.Data);
        NATS.Client.Msg response = Connection.Request(msg.Message, (int)timeout.TotalMilliseconds);
        return new Msg(response, Decode(response.Data, typeOfResponse));
    }
    public Msg Request(string subject, object data, TimeSpan timeout, Type typeOfResponse)
    {
        byte[] bytes = Encode(data);
        NATS.Client.Msg msg = Connection.Request(subject, bytes, (int)timeout.TotalMilliseconds);
        return new Msg(msg, Decode(msg.Data, typeOfResponse));
    }
    public SyncSubscription SubscribeSync(string subject, Type typeOfResponse)
    {
        NATS.Client.ISyncSubscription syncSubscription = Connection.SubscribeSync(subject);
        return new SyncSubscription(this, syncSubscription, typeOfResponse);
    }
    public SyncSubscription SubscribeSync(string subject, string queue, Type typeOfResponse)
    {
        NATS.Client.ISyncSubscription syncSubscription = Connection.SubscribeSync(subject, queue);
        return new SyncSubscription(this, syncSubscription, typeOfResponse);
    }
    public AsyncSubscription SubscribeAsync(string subject, Type typeOfResponse)
    {
        NATS.Client.IAsyncSubscription asyncSubscription = Connection.SubscribeAsync(subject);
        return new AsyncSubscription(this, asyncSubscription, typeOfResponse);
    }
    public AsyncSubscription SubscribeAsync(string subject, string queue, Type typeOfResponse)
    {
        NATS.Client.IAsyncSubscription asyncSubscription = Connection.SubscribeAsync(subject, queue);
        return new AsyncSubscription(this, asyncSubscription, typeOfResponse);
    }

    public abstract object Decode(byte[] data, Type type);
    public abstract byte[] Encode(object data);
}