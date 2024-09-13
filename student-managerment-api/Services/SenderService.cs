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
