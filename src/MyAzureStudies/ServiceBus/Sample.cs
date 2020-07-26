using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Xunit.Abstractions;

namespace MyAzureStudies.ServiceBus
{
    /// <summary>
    /// This samples was taken from
    /// <br/>
    /// https://docs.microsoft.com/pt-br/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues
    /// </summary>
    public class Sample
    {
        private ITestOutputHelper output { get; }
        private IQueueClient queueClient { get; }

        public Sample(string connectionString, string queueName, ITestOutputHelper output)
        {
            this.output = output;
            output.WriteLine("Initiaing QueueClient with default mode");
            queueClient = queueClient = new QueueClient(connectionString, queueName);
        }

        public async Task Execute()
        {
            output.WriteLine("=========================================================");
            output.WriteLine("Press ENTER key to exit after receiving all the messages.");
            output.WriteLine("=========================================================");
            
            RegisterOnMessageHandlerAndReceiveMessages();

            await queueClient.CloseAsync();
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            output.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            output.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            output.WriteLine("Exception context for troubleshooting:");
            output.WriteLine($"- Endpoint: {context.Endpoint}");
            output.WriteLine($"- Entity Path: {context.EntityPath}");
            output.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
