using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Maker365.Customers365.Contracts
{
    public interface ICustomerOrderRepository
    {
        JObject Get(Guid id);
        ICustomerOrderRepository Insert(string modelReferenceName, Stream modelStream, string modelName);
    }
}
