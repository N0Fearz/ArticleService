using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ArticleService.Services;

public class RabbitMqSenderOrganization : BackgroundService
{
    private IModel _channel;
    private readonly IConfiguration _configuration;
    private string _reply;
    private QueueDeclareOk _replyQueue;
    
    public RabbitMqSenderOrganization(IConfiguration configuration)
    {
        _configuration = configuration;
        InitRabbitMQ();
    }
    
    private void InitRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };

        // Establish connection and create a channel
        var _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        
        _replyQueue = _channel.QueueDeclare(queue: "", exclusive: true);
        _channel.QueueDeclare("request-queue", exclusive: false);
        }
    

    
    public async Task<string> SendMessage(string message)
    {
        
        var body = Encoding.UTF8.GetBytes(message);
        var routingKey = "request-queue";
        var properties = _channel.CreateBasicProperties();
        properties.ReplyTo = _replyQueue.QueueName;
        properties.CorrelationId = Guid.NewGuid().ToString();
        _channel.BasicPublish(
            exchange: "", // The topic exchange
            routingKey: routingKey, // Routing key to target specific queues
            basicProperties: properties, // Message properties (can add headers, etc.)
            body: body);

        Console.WriteLine($"Message sent to {routingKey}: {message}");

        return await Task.FromResult(_reply);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            _reply = Encoding.UTF8.GetString(body);
            
            Console.WriteLine($"received reply: {_reply}");
        };

        _channel.BasicConsume(queue: _replyQueue.QueueName,
            autoAck: true,
            consumer: consumer);
    
        return Task.CompletedTask;
    }
}