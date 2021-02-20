using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace SatTrackerDataGenerator
{
    public class SendMessage
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        public SendMessage()
        {
            factory = new ConnectionFactory();
            factory.UserName = "Worker";
            factory.Password = "workerPassword";
            factory.HostName = "localhost";

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare("SatWorker", ExchangeType.Topic, true);
            channel.QueueDeclare("WorkIn", true, false,false);

        }

        public void ProcessMessages(List<string> workList)
        {
            channel.BasicPublish("SatWorker", "In", null, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(workList)));
        }

        public void Disconnect()
        {
            channel.Close();
            connection.Close();
        }
    }
}
