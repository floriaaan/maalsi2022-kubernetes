﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMqProductAPI.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        protected readonly IConfiguration Configuration;

        public RabbitMQProducer(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void SendProductMessage<T>(T message)
        {
            //Here we specify the Rabbit MQ Server. we use rabbitmq docker image and use it
            var factory = new ConnectionFactory
            {
                HostName = Configuration["RabbitMQ:Hostname"],
                UserName = Configuration["RabbitMQ:Username"],
                Password = Configuration["RabbitMQ:Password"]
            };

            //Create the RabbitMQ connection using connection factory details as i mentioned above
            var connection = factory.CreateConnection();

            //Here we create channel with session and model
            using var channel = connection.CreateModel();

            //declare the queue after mentioning name and a few property related to that
            channel.QueueDeclare("product", exclusive: false);

            //Serialize the message
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            //put the data on to the product queue
            channel.BasicPublish(exchange: "", routingKey: "product", body: body);
        }
    }
}
