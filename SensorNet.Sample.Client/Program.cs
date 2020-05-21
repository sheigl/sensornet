using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SensorNet.Sample.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost")
                .WithClientId("Sample")
                .Build();

            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();

                Task.Run(() => mqttClient.PublishAsync("hello/world"));
            });

            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                var filter = new MqttTopicFilter();
                filter.Topic = "my/topic";

                await mqttClient.SubscribeAsync(filter);

                Console.WriteLine("### SUBSCRIBED ###");
            });

            await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("my/topic")
                .WithPayload("Hello World1234")
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None); // Since 3.0.5 with CancellationToken

            Console.ReadLine();
        }
    }
}
