namespace Natx.Client;

public abstract class SecureEncodedConnection : EncodedConnection
{
    public SecureEncodedConnection(string url) : base(new NATS.Client.ConnectionFactory().CreateSecureConnection(url))
    {

    }

}