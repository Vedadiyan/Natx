namespace Natx.Client;

public class AsyncSubscription
{
    private readonly Natx.Abstraction.IEncodedConnection encodedConnection;
    private readonly Type typeOfResponse;
    public NATS.Client.IAsyncSubscription Subscription { get; }
    public event EventHandler<MsgHandlerEventArgs>? MessageHandler;
    public AsyncSubscription(Natx.Abstraction.IEncodedConnection encodedConnection, NATS.Client.IAsyncSubscription subscription, Type typeOfResponse)
    {
        this.encodedConnection = encodedConnection;
        this.typeOfResponse = typeOfResponse;
        Subscription = subscription;
        subscription.MessageHandler += onMessage;
    }
    private void onMessage(object? sender, NATS.Client.MsgHandlerEventArgs e)
    {
        MessageHandler?.Invoke(sender, new MsgHandlerEventArgs(encodedConnection, e.Message, typeOfResponse));
    }
    public void Start()
    {
        Subscription.Start();
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