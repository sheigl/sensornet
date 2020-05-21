using MQTTnet;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SensorNet.Server
{
    internal class ApplicationMessageRecievedHandler : IMqttApplicationMessageReceivedHandler
    {
        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            Console.WriteLine($"Recieved message from {eventArgs.ClientId}");
            return Task.CompletedTask;
        }
    }
}
