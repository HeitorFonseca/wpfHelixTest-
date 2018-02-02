using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfHelixTest
{
    public class ProtocolData
    {
        public string HostName { get; set; }
        
        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string QueueName { get; set; }

        private IModel _channel;
        private IConnection _connect;

        public ProtocolData(string HostName, int Port, string Username, string Password, string QueueName)
        {
            this.HostName = HostName;
            this.Port = Port;
            this.Username = Username;
            this.Password = Password;
            this.QueueName = QueueName;
        }

        public IModel Connect()
        {
            var connectionFactory = new ConnectionFactory()
            {
                HostName = this.HostName,
                //UserName = this.Username,
                //Password = this.Password,
                //Port = this.Port,
                //Protocol = Protocols.DefaultProtocol
                
            };


            _connect = connectionFactory.CreateConnection();

            _channel = _connect.CreateModel();
            // Close connection automatically once the last open channel on the connection closes
            _connect.AutoClose = true;

            return _channel;
        }

        public void ReadEvnt(EventHandler<BasicDeliverEventArgs> MessageReceivedCallback)
        {
            _channel.QueueDeclare(queue: this.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += MessageReceivedCallback;

            _channel.BasicConsume(queue: this.QueueName, autoAck: true, consumer: consumer);           
        }

        public void Disconnect()
        {
            _channel.Close();

            try
            {
                _connect.Close();
            }
            catch{ }
        }
    }
}
