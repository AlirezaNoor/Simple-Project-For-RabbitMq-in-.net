namespace ConsumerProject.Service;

using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMQConsumerHostedService : IHostedService
{
    private IConnection _connection;
    private IModel _channel;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // ایجاد اتصال به RabbitMQ
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();  // اتصال را ایجاد و نگه‌دار
        _channel = _connection.CreateModel();  // کانال را ایجاد و نگه‌دار

        _channel.QueueDeclare(queue: "orders",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
        };

        _channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // بستن کانال و اتصال هنگام توقف سرویس
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }
}
