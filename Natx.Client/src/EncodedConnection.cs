namespace Natx.Client;

public abstract class EncodedConnection : Natx.Abstraction.IEncodedConnection
{
    public NATS.Client.IConnection Connection { get; }
    public EncodedConnection()
    {
        Connection = new NATS.Client.ConnectionFactory().CreateConnection();
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
    public Msg Request(string subject, object data)
    {
        byte[] bytes = Encode(data);
        NATS.Client.Msg msg = Connection.Request(subject, bytes);
        return new Msg(msg, Decode(msg.Data));
    }
    public Msg Request(Msg msg)
    {
        msg.Message.Data = Encode(msg.Data);
        NATS.Client.Msg response = Connection.Request(msg.Message);
        return new Msg(response, Decode(response.Data));
    }
    public Msg Request(Msg msg, TimeSpan timeout)
    {
        msg.Message.Data = Encode(msg.Data);
        NATS.Client.Msg response = Connection.Request(msg.Message, (int)timeout.TotalMilliseconds);
        return new Msg(response, Decode(response.Data));
    }
    public Msg Request(string subject, object data, TimeSpan timeout)
    {
        byte[] bytes = Encode(data);
        NATS.Client.Msg msg = Connection.Request(subject, bytes, (int)timeout.TotalMilliseconds);
        return new Msg(msg, Decode(msg.Data));
    }
    public SyncSubscription SubscribeSync(string subject)
    {
        NATS.Client.ISyncSubscription syncSubscription = Connection.SubscribeSync(subject);
        return new SyncSubscription(this, syncSubscription);
    }
    public SyncSubscription SubscribeSync(string subject, string queue)
    {
        NATS.Client.ISyncSubscription syncSubscription = Connection.SubscribeSync(subject, queue);
        return new SyncSubscription(this, syncSubscription);
    }
    public AsyncSubscription SubscribeAsync(string subject)
    {
        NATS.Client.IAsyncSubscription asyncSubscription = Connection.SubscribeAsync(subject);
        return new AsyncSubscription(this, asyncSubscription);
    }
    public AsyncSubscription SubscribeAsync(string subject, string queue)
    {
        NATS.Client.IAsyncSubscription asyncSubscription = Connection.SubscribeAsync(subject, queue);
        return new AsyncSubscription(this, asyncSubscription);
    }

    public abstract object Decode(byte[] data);
    public abstract byte[] Encode(object data);
}

public class ServerTemplate
{
    private readonly EncodedConnection encodedConnection;
    private readonly AsyncSubscription subscription;
    private readonly Func<object, object> handler;
    public ServerTemplate(EncodedConnection encodedConnection, Func<object, object> handler)
    {
        this.encodedConnection = encodedConnection;
        subscription = encodedConnection.SubscribeAsync("", "");
        this.handler = handler;
        subscription.MessageHandler += onMessage;
    }
    private void onMessage(object? sender, MsgHandlerEventArgs e)
    {
        try
        {
            object response = handler(e.Message.Data);
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

public class ClientTemplate
{
    private readonly EncodedConnection encodedConnection;
    public ClientTemplate(EncodedConnection encodedConnection)
    {
        this.encodedConnection = encodedConnection;
    }
    public object Request(object request)
    {
        return encodedConnection.Request("", request, TimeSpan.FromMinutes(1));
    }
}