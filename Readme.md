# RabbitMQ Messaging with ASP.NET Core

This project demonstrates how to implement a basic message queue system using RabbitMQ in ASP.NET Core. It consists of two separate projects:

1. **Sender** - Sends messages to a RabbitMQ queue.
2. **Consumer** - Consumes messages from a RabbitMQ queue using `IHostedService`.

## Prerequisites

Before you start, make sure you have the following installed:

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)
- [RabbitMQ Server](https://www.rabbitmq.com/download.html)
- [RabbitMQ.Client NuGet Package](https://www.nuget.org/packages/RabbitMQ.Client/)

## Setup RabbitMQ

1. Download and install RabbitMQ server from the official website.
2. Run RabbitMQ server on your machine (default configuration uses `localhost:5672`).

You can verify that RabbitMQ is running by navigating to the management interface at `http://localhost:15672/`. Use default credentials (`guest` / `guest`).

## Project Structure

- **Sender Project**: Sends messages to the `orders` queue.
- **Consumer Project**: Listens for messages from the `orders` queue and processes them.

## Running the Projects

Follow the steps below to run both projects.

### 1. Sender Project

The sender sends messages to the RabbitMQ queue. You can configure and run it as follows:

#### Installation

1. Navigate to the **Sender** project directory.
2. Restore dependencies:

    ```bash
    dotnet restore
    ```

3. Build the project:

    ```bash
    dotnet build
    ```

4. Run the project:

    ```bash
    dotnet run
    ```

#### Code Overview

The `Sender` project sends a message to the RabbitMQ `orders` queue.

```csharp
using RabbitMQ.Client;
using System;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "orders",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

string message = "Hello World! Order created.";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: "",
                     routingKey: "orders",
                     basicProperties: null,
                     body: body);

Console.WriteLine($" [x] Sent {message}");
```

#### Output
```csharp
[x] Sent Hello World! Order created.
```


### 2. Consumer Project

The consumer listens for messages from the RabbitMQ orders queue using IHostedService and processes them.

1. Installation
2. Navigate to the Consumer project directory.

Restore dependencies:

```csharp
dotnet restore
```
3. Build the project:
```csharp
dotnet build
```

4 .Run the project:

```csharp
dotnet run
```

## Code Overview
The Consumer project listens to the RabbitMQ orders queue and prints the received message to the console.
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
```csharp
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

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
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }
}
```

#### Output:

Once a message is sent by the sender, the consumer will receive and print it to the console:

```csharp
[x] Received Hello World! Order created.
```
### 3. Running Both Projects Together
1. First, start the Consumer project by running dotnet run in the consumer directory. This will start listening for messages.
2. n another terminal window, start the Sender project by running dotnet run in the sender directory. This will send a message to the orders queue.
3. Observe the message being received and printed in the consumer's console.


### Stopping the Projects
To stop the Consumer, press Ctrl+C in the terminal where it's running.
The Sender will automatically stop after sending the message.
### Customization
Queue name: You can change the queue name by modifying the queue parameter in both the Sender and Consumer projects.
Message content: The message sent by the sender can be customized in the Sender project.
