using RabbitMQ.Client;
using System.Text;

namespace SOP.RabbitMQ;
public class MessageProducer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageProducer()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "wagons_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: "wagons_queue", basicProperties: null, body: body);
        Console.WriteLine(" [x] Sent {0}", message);
    }

    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}