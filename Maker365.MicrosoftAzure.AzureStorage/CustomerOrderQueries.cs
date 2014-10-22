using Maker365.Customers365.Contracts;
using MarcoParenzan.MicrosoftAzure.AzureStorage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker365.MicrosoftAzure.AzureStorage
{
    public class CustomerOrderQueries : AzureStorageHandler<CustomerOrderQueries>, ICustomerOrderQueries
    {
        private string _containerName = "customerorders";

        public CustomerOrderQueries(CloudStorageAccount account, PartitionKey partitionKey)
            : base(account, partitionKey)
        {
        }

        JObject ICustomerOrderQueries.Page(int pageNumber, int pageSize)
        {
            return PageOfTableEntries<CustomerOrderIndexTableEntity>(pageNumber, pageSize, _containerName, (c, p) => string.Format("CustomerOrderIndexTableEntity-{0}-by-{1}.json", c, p));
        }
    }
}
