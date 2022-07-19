namespace Natx.Client;

public class SyncSubscription
{
    private readonly Natx.Abstraction.IEncodedConnection encodedConnection;
    public NATS.Client.ISyncSubscription Subscription { get; }
    public SyncSubscription(Natx.Abstraction.IEncodedConnection encodedConnection, NATS.Client.ISyncSubscription subscription)
    {
        this.encodedConnection = encodedConnection;
        Subscription = subscription;
    }
    public Msg NextMessage()
    {
        NATS.Client.Msg msg = Subscription.NextMessage();
        return new Msg(msg, encodedConnection.Decode(msg.Data));
    }
    public Msg NextMessage(TimeSpan timeout)
    {
        NATS.Client.Msg msg = Subscription.NextMessage((int)timeout.TotalMilliseconds);
        return new Msg(msg, encodedConnection.Decode(msg.Data));
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