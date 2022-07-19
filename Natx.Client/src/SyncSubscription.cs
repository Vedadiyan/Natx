namespace Natx.Client;

public class SyncSubscription
{
    private readonly Natx.Abstraction.IEncodedConnection encodedConnection;
    private readonly Type typeOfResponse;
    public NATS.Client.ISyncSubscription Subscription { get; }
    public SyncSubscription(Natx.Abstraction.IEncodedConnection encodedConnection, NATS.Client.ISyncSubscription subscription, Type typeOfResponse)
    {
        this.encodedConnection = encodedConnection;
        this.typeOfResponse = typeOfResponse;
        Subscription = subscription;
    }
    public Msg NextMessage()
    {
        NATS.Client.Msg msg = Subscription.NextMessage();
        return new Msg(msg, encodedConnection.Decode(msg.Data, typeOfResponse));
    }
    public Msg NextMessage(TimeSpan timeout)
    {
        NATS.Client.Msg msg = Subscription.NextMessage((int)timeout.TotalMilliseconds);
        return new Msg(msg, encodedConnection.Decode(msg.Data, typeOfResponse));
    }
    public void Unsubscribe()
    {
        Subscription.Unsubscribe();
    }
    public void AutoUnsubscribe(int max)
    {
        Subscription.AutoUnsubscribe(max);
    }
}