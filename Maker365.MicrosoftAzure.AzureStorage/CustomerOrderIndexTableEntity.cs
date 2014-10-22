using MarcoParenzan.MicrosoftAzure.AzureStorage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maker365.MicrosoftAzure.AzureStorage
{
    public class CustomerOrderIndexTableEntity : AzureStorageIndexTableEntity<CustomerOrderIndexTableEntity>
    {
        public string ModelReferenceName { get; set; }
        public string ModelFileName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public CustomerOrderState? State { get; set; }
        protected override void OnReadEntity(IDictionary<string, EntityProperty> properties)
        {
            base.OnReadEntity(properties);
            if (properties.ContainsKey("ModelReferenceName"))
            {
                this.ModelReferenceName = properties["ModelReferenceName"].StringValue;
            }
            if (properties.ContainsKey("ModelFileName"))
            {
                this.ModelFileName = properties["ModelFileName"].StringValue;
            }
            if (properties.ContainsKey("State"))
            {
                this.State = (CustomerOrderState) Enum.Parse(typeof(CustomerOrderState), properties["State"].StringValue);
            }
        }

        protected override void OnWriteEntity(Dictionary<string, EntityProperty> properties)
        {
            base.OnWriteEntity(properties);
            properties.Add("ModelReferenceName", EntityProperty.GeneratePropertyForString(this.ModelReferenceName));
            properties.Add("ModelFileName", EntityProperty.GeneratePropertyForString(this.ModelFileName));
            properties.Add("State", EntityProperty.GeneratePropertyForString(this.State.ToString()));
            properties.Add("Timestamp", EntityProperty.GeneratePropertyForDateTimeOffset(this.Timestamp));
        }

        public CustomerOrderIndexTableEntity CreateIdIfNew()
        {
            this.Id = Guid.NewGuid(); ;
            return this;
        }

        public CustomerOrderIndexTableEntity ReadIdFrom(JObject content)
        {
            JToken id;
            if (content.TryGetValue("_id", out id))
            {
                this.Id = id.Value<Guid>();
            }
            return this;
        }

        public CustomerOrderIndexTableEntity ReadFrom(JObject content)
        {
            this.ModelReferenceName = content.Value<string>("_modelReferenceName");
            this.ModelFileName = content.Value<string>("_modelFileName");
            this.State = content.Value<CustomerOrderState>("_state");
            return this;
        }
    }
}
