using Newtonsoft.Json.Linq;
using System;

namespace Maker365.Customers365.Contracts
{
    public interface IUtecCustomerOrderQueries
    {
        JObject Page(int pageNumber, int pageSize);
    }
}
