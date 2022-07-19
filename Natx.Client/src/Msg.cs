namespace Natx.Client;
public readonly struct Msg
{
    public NATS.Client.Msg Message { get; }
    public object Data { get; }
    private readonly EncodedConnection? encodedConnection;
    public Msg(NATS.Client.Msg Message, object data)
    {
        this.Message = Message;
        Data = data;
        encodedConnection = null;
    }
    public Msg(NATS.Client.Msg Message, object data, EncodedConnection? encodedConnection)
    {
        this.Message = Message;
        Data = data;
        this.encodedConnection = encodedConnection;
    }
    public void Respond(object data)
    {
        Message.Respond(encodedConnection?.Encode(data));
    }
}