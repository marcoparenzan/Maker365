using Newtonsoft.Json.Linq;
using System;

namespace Maker365.Customers365.Contracts
{
    public interface ICustomerOrderQueries
    {
        JObject Page(int pageNumber, int pageSize);
    }
}
