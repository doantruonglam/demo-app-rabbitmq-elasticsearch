using Microsoft.EntityFrameworkCore;
using my_app.Context;
using my_app.Models;
using my_app.Models.EF;
using Nest;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Linq;
using System.Net;
using System.Text;

namespace my_app.Services
{
    public interface ISenderService
    {
        Task PutNotification(Student std, string notiType);
    }

    public class SenderService : ISenderService
    {
        private readonly myappContext _context;
        private readonly IElasticClient _elasticClient;

        public SenderService(myappContext context, IElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
            StartConfigCreate();
            StartConfigUpdate();
            StartConfigDelete();
        }

        private void StartConfigCreate()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "host.docker.internal",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "create",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var student = JsonConvert.DeserializeObject<Student>(message);

                await _elasticClient.IndexDocumentAsync(student);
            };

            channel.BasicConsume(queue: "create",
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void StartConfigUpdate()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "host.docker.internal",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "update",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var student = JsonConvert.DeserializeObject<Student>(message);

                await _elasticClient.UpdateAsync<Student>(student.Id, u => u.Doc(student));
            };

            channel.BasicConsume(queue: "update",
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void StartConfigDelete()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "host.docker.internal",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "delete",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var student = JsonConvert.DeserializeObject<Student>(message);

                await _elasticClient.DeleteAsync<Student>(student.Id);
            };

            channel.BasicConsume(queue: "delete",
                                 autoAck: true,
                                 consumer: consumer);
        }

        public async Task PutNotification(Student std, string notiType)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "host.docker.internal",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: notiType,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = JsonConvert.SerializeObject(std);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "",
                                 routingKey: notiType,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
