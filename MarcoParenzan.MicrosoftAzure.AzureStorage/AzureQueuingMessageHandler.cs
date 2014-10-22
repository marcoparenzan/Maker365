using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.MicrosoftAzure.AzureStorage
{
    public class AzureQueuingMessageHandler : AzureStorageHandler<AzureQueuingMessageHandler>
    {
        public AzureQueuingMessageHandler(CloudStorageAccount account, PartitionKey partitionKey): base(account, partitionKey)
        {
        }

        public void Handle(Message message)
        {
            AzureQueueMessage queueMessage = null;
            switch (message.type)
            {
                case "recipy_composition":
                    queueMessage = new AzureQueueMessage
                    {
                        Content = message.content
                        ,
                        QueueName = "recipy_composition"
                        ,
                        PartitionKey = PartitionKey
                    };
                    break;
            }
            var queue = NewQueueClient().GetQueueReference(queueMessage.QueueName);
            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(queueMessage))
            {
            });

        }
    }
}
