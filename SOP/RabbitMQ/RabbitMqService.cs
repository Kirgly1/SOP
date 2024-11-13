using System.Text;
using RabbitMQ.Client;

namespace SOP.RabbitMQ
{
    public class RabbitMqService
    {
        private readonly IModel _channel;

        public RabbitMqService(IConnectionFactory connectionFactory)
        {
            var connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "wagons", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public IModel Channel => _channel;

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; 

            _channel.BasicPublish(exchange: "", routingKey: "wagons", basicProperties: properties, body: body);
            Console.WriteLine($"Отправлено сообщение: {message}");
        }
    }
}