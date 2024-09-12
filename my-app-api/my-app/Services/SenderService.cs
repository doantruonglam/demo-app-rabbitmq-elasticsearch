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
            StartListening();
        }

        private void StartListening()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "root",
                VirtualHost = "/"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "students",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var student = JsonConvert.DeserializeObject<StudentConsumer>(message);

                await HandleStudentAction(student);
            };

            channel.BasicConsume(queue: "students",
                                 autoAck: true,
                                 consumer: consumer);
        }

        private async Task HandleStudentAction(StudentConsumer student)
        {
            switch (student.Action)
            {
                case "create":
                    await _elasticClient.IndexDocumentAsync(student);
                    break;
                case "update":
                    await _elasticClient.UpdateAsync<Student>(student.Id, u => u.Doc(student));
                    break;
                case "delete":
                    await _elasticClient.DeleteAsync<Student>(student.Id);
                    break;
            }
        }

        public async Task PutNotification(Student std, string notiType)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "admin",
                Password = "root",
                VirtualHost = "/"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: notiType,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var stdConsumer = new StudentConsumer
            {
                Action = notiType,
                Address = std.Address,
                Class = std.Class,
                CreatedAt = std.CreatedAt,
                Dob = std.Dob,
                Id = std.Id,
                Name = std.Name,
                UpdatedAt = std.UpdatedAt
            };

            string message = JsonConvert.SerializeObject(stdConsumer);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "",
                                 routingKey: notiType,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
