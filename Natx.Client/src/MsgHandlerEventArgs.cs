namespace Natx.Client;


public class MsgHandlerEventArgs : EventArgs
{
    private readonly Natx.Abstraction.IEncodedConnection encodedConnection;
    public Msg Message { get; }
    public MsgHandlerEventArgs(Natx.Abstraction.IEncodedConnection encodedConnection, NATS.Client.Msg message)
    {
        this.encodedConnection = encodedConnection;
        Message = new Msg(message, encodedConnection.Decode(message.Data));
    }
}