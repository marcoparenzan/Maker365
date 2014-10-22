using Maker365.Customers365.Commands;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Maker365.Customers365.Contracts
{
    public interface ICustomerOrderRepositoryIHandleNewCustomerOrder
    {
        void Handle(NewCustomerOrderCommand command);
    }
}
