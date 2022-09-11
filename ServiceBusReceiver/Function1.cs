using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ServiceBusReceiver
{
    public class Function1
    {
        [FunctionName("SbReceiver")]
        public void Run([ServiceBusTrigger("personqueue", Connection = "SbConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
            Console.WriteLine(myQueueItem);
        }
    }
}
