using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
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

namespace MarcoParenzan.MicrosoftAzure.AzureStorage
{
    public abstract class AzureStorageHandler<T>
        where T : AzureStorageHandler<T>
    {
        private CloudStorageAccount _account;
        private PartitionKey _partitionKey;

        protected PartitionKey PartitionKey
        {
            get { return _partitionKey; }
        }

        public AzureStorageHandler(CloudStorageAccount account, PartitionKey partitionKey)
        {
            _account = account;
            _partitionKey = partitionKey;
        }

        protected CloudStorageAccount Account
        {
            get { return _account; }
        }

        protected CloudQueueClient NewQueueClient()
        {
            return Account.CreateCloudQueueClient();
        }

        protected CloudTableClient NewTableClient()
        {
            return Account.CreateCloudTableClient();
        }

        protected CloudBlobClient NewBlobClient()
        {
            return Account.CreateCloudBlobClient();
        }

        protected CloudFileClient NewFileClient()
        {
            return Account.CreateCloudFileClient();
        }

        protected CloudFileDirectory GoToDirectory(string containerName, params string[] path)
        {
            var container = NewFileClient().GetShareReference(containerName);
            container.CreateIfNotExists();
            var currentDir = container.GetRootDirectoryReference();
            currentDir.CreateIfNotExists();
            foreach (var pathPart in path)
            {
                currentDir = currentDir.GetDirectoryReference(pathPart);
                currentDir.CreateIfNotExists();
            }
            return currentDir;
        }
        protected T EnsureFile<TContent>(TContent content, string contentName, string containerName, params string[] path)
        {
            var currentDir = GoToDirectory(containerName, path);
            EnsureFile(content, contentName, currentDir);
            return (T) this;
        }

        protected T EnsureFile<TContent>(TContent content, string contentName, CloudFileDirectory currentDir)
        {
            var entityRef = currentDir.GetFileReference(contentName);
            entityRef.DeleteIfExists();
            if (content is string)
            {
                entityRef.UploadText((string)(object)content);
            }
            else if (content is Stream)
            {
                entityRef.UploadFromStream((Stream)(object)content);
            }
            else
            {
                throw new NotSupportedException("Cannot handle this kind of content");
            }
            return (T)this;
        }

        protected T EnsureBlob(string content, string contentName, string containerName)
        {
            var containerRef = NewBlobClient().GetContainerReference(containerName);
            var entityRef = containerRef.GetBlockBlobReference(contentName);
            entityRef.DeleteIfExists();
            entityRef.UploadText(content);
            return (T)this;
        }

        protected T InsertTableIndex<TIndex>(TIndex index, string tableName)
            where TIndex: ITableEntity
        {
            var op = TableOperation.Insert(index);
            var table = NewTableClient().GetTableReference(tableName);
            table.CreateIfNotExists();
            table.Execute(op);
            InvalidateIndexFolder<TIndex>(tableName);
            return (T)this;
        }

        protected void InvalidateIndexFolder<TIndex>(string tableName)
                where TIndex : ITableEntity
        {
            var d = GoToDirectory("accounts", PartitionKey, tableName, "indexes", typeof(TIndex).Name);
            foreach (var f in d.ListFilesAndDirectories())
            {
                d.GetFileReference(f.Uri.Segments.Last()).Delete();
            }
        }

        protected void ReplaceTableIndex<TIndex>(TIndex index, string tableName)
            where TIndex : ITableEntity
        {
            var op = TableOperation.Replace(index);
            var table = NewTableClient().GetTableReference(tableName);
            table.CreateIfNotExists();
            table.Execute(op);
            InvalidateIndexFolder<TIndex>(tableName);
        }

        protected JObject PageOfTableEntries<TTableEntity>(int pageNumber, int pageSize, string containerName, Func<int,int,string> indexNameOf)
            where TTableEntity : ITableEntity, new()
        {
            var currentDir = GoToDirectory("accounts", PartitionKey, containerName, "indexes", typeof(TTableEntity).Name);
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
                .Where(xx => xx.PartitionKey == this.PartitionKey)
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

        protected JObject GetTableEntry(Guid id, string tableName)
        {
            var op = TableOperation.Retrieve(this.PartitionKey, id.ToString());
            var table = NewTableClient().GetTableReference(tableName);
            var result = table.Execute(op);
            return (JObject)JsonConvert.DeserializeObject(result.Result.ToString());
        }
    }
}
