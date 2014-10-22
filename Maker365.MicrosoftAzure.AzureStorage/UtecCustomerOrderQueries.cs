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
    public class UtecCustomerOrderQueries : AzureStorageHandler<CustomerOrderQueries>, IUtecCustomerOrderQueries
    {
        private string _containerName = "customerorders";

        public UtecCustomerOrderQueries(CloudStorageAccount account, PartitionKey partitionKey)
            : base(account, partitionKey)
        {
        }


        protected JObject xx<TTableEntity>(int pageNumber, int pageSize, string containerName, Func<int, int, string> indexNameOf)
            where TTableEntity : ITableEntity, new()
        {
            var currentDir = GoToDirectory("management", "Utec", containerName, "indexes", typeof(TTableEntity).Name);
            var indexRef = currentDir.GetFileReference(indexNameOf(pageNumber, pageSize));
            if (indexRef.Exists())
            {
                var page = JObject.Parse(indexRef.DownloadText());
                return page;
            };

            var table = NewTableClient().GetTableReference(containerName);

            var query =
                table
                .CreateQuery<TTableEntity>()
            ;

            var result = query.ToList();
            // result = result.OrderByDescending(xx => xx.Description).ToList();

            var currentPage = pageNumber;
            var pageCount = result.Count / pageSize;
            var cursor = result.Skip(0);
            while (true)
            {
                var taken = cursor.Take(pageSize);
                if (taken.Count() == 0) break;

                var indexName = indexNameOf(currentPage, pageSize);
                EnsureFile(JsonConvert.SerializeObject(new
                {
                    pageNumber = currentPage
                    ,
                    pageCount = pageCount
                    ,
                    pageSize = pageSize
                    ,
                    isEmpty = false
                    ,
                    content = cursor.Take(pageSize)
                }), indexName, currentDir);

                if (taken.Count() < pageSize) break;
                cursor = cursor.Skip(pageSize);
            }

            if (indexRef.Exists())
            {
                var page = JObject.Parse(indexRef.DownloadText());
                return page;
            }
            else
            {
                var page = JObject.FromObject(new
                {
                    PageNumber = currentPage
                    ,
                    PageCount = pageCount
                    ,
                    PageSize = 0
                    ,
                    Empty = true
                    ,
                    Content = cursor.Take(pageSize)
                });
                return page;
            }
        }

        JObject IUtecCustomerOrderQueries.Page(int pageNumber, int pageSize)
        {
            return xx<CustomerOrderIndexTableEntity>(pageNumber, pageSize, _containerName, (c, p) => string.Format("CustomerOrderIndexTableEntity-{0}-by-{1}.json", c, p));
        }
    }
}
