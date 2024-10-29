using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SOP.RabbitMQ;

public class RabbitMqConsumer
{
    private readonly RabbitMqService _rabbitMqService;

    public RabbitMqConsumer(RabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_rabbitMqService.Channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Получено сообщение: {message}");
        };

        _rabbitMqService.Channel.BasicConsume(queue: "wagons",
            autoAck: true,
            consumer: consumer);
    }
}