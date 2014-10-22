using Maker365.Customers365.Contracts;
using MarcoParenzan.MicrosoftAzure.AzureStorage;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker365.MicrosoftAzure.AzureStorage
{
    public class CustomerOrderRepository : AzureStorageHandler<CustomerOrderRepository>, ICustomerOrderRepository
    {
        private string _entityName = "customerorder";
        private string _containerName = "customerorders";

        public CustomerOrderRepository(CloudStorageAccount account, PartitionKey partitionKey)
            : base(account, partitionKey)
        {
        }

        ICustomerOrderRepository ICustomerOrderRepository.Insert(string modelReferenceName, Stream modelStream, string modelFileName)
        {
            var index = AzureStorageIndexTableEntity<CustomerOrderIndexTableEntity>.CreateNew(PartitionKey).CreateIdIfNew();
            index.ModelReferenceName = modelReferenceName;
            index.ModelFileName = modelFileName;
            index.State = CustomerOrderState.Submitted;
            EnsureContent(modelStream, index);
            InsertTableIndex(index, _containerName);
            return this;
        }

        protected CustomerOrderRepository EnsureContent(Stream modelStream, CustomerOrderIndexTableEntity index)
        {
            EnsureFile(modelStream, index.ModelFileName, "accounts", PartitionKey, _containerName, index.ModelReferenceName);
            return this;
        }

        JObject ICustomerOrderRepository.Get(Guid id)
        {
            return GetTableEntry(id, _containerName);
        }
    }
}
