using System;
using RabbitMQ.Client;
using System.Text;
using System.Threading;

class Program
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using(var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            string message = string.Empty;

            for (int i = 0; i < 1000; i++)
            {
                message = "{ \"type\": \"VS\", \"sensorid\" : 2, \"user\": \"b8-27-eb-97-3d-b6\", \"value\":" + i + ", \"ts\": \"3497855005850\" }";

                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

                Thread.Sleep(20);
            }
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
