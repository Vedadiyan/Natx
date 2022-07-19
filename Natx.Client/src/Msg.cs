namespace Natx.Client;
public readonly struct Msg
{
    public NATS.Client.Msg Message { get; }
    public object Data { get; }
    public Msg(NATS.Client.Msg Message, object data)
    {
        this.Message = Message;
        Data = data;
    }
}