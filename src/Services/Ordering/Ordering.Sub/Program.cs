// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
};
var conn = connectionFactory.CreateConnection();
using var channel = conn.CreateModel();
channel.QueueDeclare("orders", exclusive: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    var message = Encoding.UTF8.GetString(args.Body.ToArray());
    Console.WriteLine($"Received message: {message}");
};

channel.BasicConsume(queue: "orders", autoAck:true, consumer: consumer);
Console.ReadKey();