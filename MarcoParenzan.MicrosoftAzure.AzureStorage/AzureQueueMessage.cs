using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.MicrosoftAzure.AzureStorage
{
    public class AzureQueueMessage
    {
        public string QueueName { get; set; }
        public string PartitionKey { get; set; }
        public JObject Content { get; set; }
    }
}
