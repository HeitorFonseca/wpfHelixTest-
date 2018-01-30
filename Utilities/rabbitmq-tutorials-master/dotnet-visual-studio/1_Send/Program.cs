using System;
using RabbitMQ.Client;
using System.Text;
using System.Threading;
using System.Globalization;

class Program
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        factory.Port = 5672;
        factory.UserName = "userTest";
        factory.Password = "userTest";
        factory.Protocol = Protocols.DefaultProtocol;

        using (var connection = factory.CreateConnection())
        using(var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

            string message = string.Empty;
            Random rnd = new Random();

            for (int i = 0; i < 40; i++)
            {
                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToString();
                
                
                message = "{ \"type\": \"HPS\", \"sensorid\" : 1, \"user\": \"b8-27-eb-97-3d-b6\", \"value\":[" + 
                        Convert.ToString(i, CultureInfo.InvariantCulture) + "," + 
                        Convert.ToString(i + rnd.Next(-10, 11), CultureInfo.InvariantCulture) + "],\"ts\": \"" + Timestamp + "\" }";

                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

                message = "{ \"type\": \"HPS\", \"sensorid\" : 2, \"user\": \"b8-27-eb-97-3d-b6\", \"value\":[" + 
                    Convert.ToString(i , CultureInfo.InvariantCulture) + "," + 
                    Convert.ToString(i + rnd.Next(-10, 11), CultureInfo.InvariantCulture) + "],\"ts\": \"" + 
                    Timestamp + "\" }";
                body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

                Thread.Sleep(500);
            }
            Console.WriteLine(" [x] Sent {0}", message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
