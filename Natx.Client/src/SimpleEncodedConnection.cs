namespace Natx.Client;

public abstract class SimpleEncodedConnection : EncodedConnection
{
    public SimpleEncodedConnection() : base(new NATS.Client.ConnectionFactory().CreateConnection())
    {

    }
    public SimpleEncodedConnection(string url) : base(new NATS.Client.ConnectionFactory().CreateConnection(url))
    {

    }
    public SimpleEncodedConnection(string url, string credentialPath): base(new NATS.Client.ConnectionFactory().CreateConnection(url, credentialPath))
    {
        
    }
    public SimpleEncodedConnection(string url, string jwt, string privateKey) : base(new NATS.Client.ConnectionFactory().CreateConnection(url, jwt, privateKey))
    {

    }
    public SimpleEncodedConnection(NATS.Client.Options options): base(new NATS.Client.ConnectionFactory().CreateConnection(options))
    {
    
    }
}