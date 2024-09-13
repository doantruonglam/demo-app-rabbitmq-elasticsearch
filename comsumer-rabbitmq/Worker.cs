using Nest;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace comsumer_rabbitmq;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private IConnection _connection;
    private IModel _channel;
    private readonly ElasticClient _elasticClient;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "create", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "update", durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueDeclare(queue: "delete", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var elasticsearchSettings = new ConnectionSettings(new Uri("localhost:9200")).DefaultIndex("students");
        _elasticClient = new ElasticClient(elasticsearchSettings);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumerCreate = new EventingBasicConsumer(_channel);
        consumerCreate.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var student = JsonConvert.DeserializeObject<Student>(message);

            if (student != null)
                await _elasticClient.IndexDocumentAsync(student);
        };

        _channel.BasicConsume(queue: "create",
                             autoAck: true,
                             consumer: consumerCreate);

        var consumerUpdate = new EventingBasicConsumer(_channel);
        consumerUpdate.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var student = JsonConvert.DeserializeObject<Student>(message);

            if(student != null)
                await _elasticClient.UpdateAsync<Student>(student.Id, u => u.Doc(student));
        };

        _channel.BasicConsume(queue: "update",
                             autoAck: true,
                             consumer: consumerUpdate);

        var consumerDelete = new EventingBasicConsumer(_channel);
        consumerDelete.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var student = JsonConvert.DeserializeObject<Student>(message);

            if(student != null)
                await _elasticClient.DeleteAsync<Student>(student.Id);
        };

        _channel.BasicConsume(queue: "delete",
                             autoAck: true,
                             consumer: consumerDelete);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
