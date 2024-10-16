using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RabbitMqExample.Services;

public class SenderService
{
    private readonly ConnectionFactory _factory;

    public SenderService()
    {
        _factory = new ConnectionFactory() { HostName = "localhost" };
    }

    public async Task SendMessage(string message)
    {
        using var connection = await _factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync("orders", durable: true, exclusive: false, autoDelete: false, arguments: null);
        
        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);
        
        await channel.BasicPublishAsync(exchange: "", routingKey: "orders", body: body);


    }
}