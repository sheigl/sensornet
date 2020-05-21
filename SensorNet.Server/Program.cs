using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Server;

namespace SensorNet.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (src, e) => 
            {
                Console.WriteLine("Shutting down");
                cts.Cancel();
            };

            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.ApplicationMessageReceivedHandler = new ApplicationMessageRecievedHandler();
            await mqttServer.StartAsync(new MqttServerOptions());
            Console.WriteLine($"Running MQTT server on port '{mqttServer.Options.DefaultEndpointOptions.Port}'");

            while (!cts.Token.IsCancellationRequested)
            {
                
            }

            await mqttServer.StopAsync();
        }
    }
}
