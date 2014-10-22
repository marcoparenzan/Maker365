using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcoParenzan.MicrosoftAzure.AzureStorage
{
    public abstract class AzureStorageIndexTableEntity<T>: ITableEntity
        where T : AzureStorageIndexTableEntity<T>
    {
        public static T CreateNew(string partitionKey)
        {
            var t = (T)Activator.CreateInstance(typeof(T));
            t.WithPartitionKey(partitionKey);
            return t;
        }

        public T WithPartitionKey(string partitionKey)
        {
            this._partitionKey = partitionKey;
            return (T) this;
        }

        private string _partitionKey;

        public string PartitionKey
        {
            get { return _partitionKey; }
        }

        public Guid Id { get; set; }
        public string ETag { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        string ITableEntity.ETag
        {
            get
            {
                return ETag;
            }
            set
            {
                this.ETag = value;
            }
        }

        string ITableEntity.PartitionKey
        {
            get
            {
                return _partitionKey;
            }
            set
            {
                _partitionKey = value;
            }
        }
        
        string ITableEntity.RowKey
        {
            get
            {
                return this.Id.ToString();
            }
            set
            {
                this.Id = Guid.Parse(value);
            }
        }

        DateTimeOffset ITableEntity.Timestamp
        {
            get
            {
                return Timestamp;
            }
            set
            {
                Timestamp = value;
            }
        }

        void ITableEntity.ReadEntity(IDictionary<string, EntityProperty> properties, Microsoft.WindowsAzure.Storage.OperationContext operationContext)
        {
            OnReadEntity(properties);
        }

        protected virtual void OnReadEntity(IDictionary<string,EntityProperty> properties)
        {
            if (properties.ContainsKey("Id"))
            {
                this.Id = properties["Id"].GuidValue.Value;
            }
        }

        IDictionary<string, EntityProperty> ITableEntity.WriteEntity(Microsoft.WindowsAzure.Storage.OperationContext operationContext)
        {
            var properties = new Dictionary<string, EntityProperty>();

            OnWriteEntity(properties);

            return properties;
        }

        protected virtual void OnWriteEntity(Dictionary<string,EntityProperty> properties)
        {
            properties.Add("Id", EntityProperty.GeneratePropertyForGuid(this.Id));
        }
    }
}
