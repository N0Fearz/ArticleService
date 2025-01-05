using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Configuration;
using System.Text;

namespace ArticleService.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly string _queueName = "organization-create-queue";
        private readonly string _routingKey = "organization.create";

        private IConnection _connection;
        private IModel _channel;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQConsumer(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
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
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare a queue (optional for pre-declared queues like amq.topic)
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(_queueName, "amq.topic", _routingKey);
            _channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => StopRabbitMQ());

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Process the message
                await HandleMessageAsync(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: _queueName,
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(string message)
        {
            // Add your message processing logic here
            Console.WriteLine($"Received message: {message}");
            using var scope = _serviceScopeFactory.CreateScope();
            
            var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
            await migrationService.AddSchemaAsync(message);
            await Task.Delay((TimeSpan.FromSeconds(5)));
            await migrationService.MigrateAsync(message);
        }

        private void StopRabbitMQ()
        {
            _channel?.Close();
            _connection?.Close();
        }

        public override void Dispose()
        {
            StopRabbitMQ();
            base.Dispose();
        }
    }
}
