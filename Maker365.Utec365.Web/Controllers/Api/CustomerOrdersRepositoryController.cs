using Maker365.Customers365.Commands;
using Maker365.Customers365.Contracts;
using Microsoft.AspNet.SignalR;
using System.Web.Mvc;

namespace Maker365.Utec365.Web.Controllers
{
    public class CustomerOrdersRepositoryController : Controller
    {
        private IHubContext _notify;
        private ICustomerOrderRepository _customerOrderRepository;

        public CustomerOrdersRepositoryController(ICustomerOrderRepository customerOrderRepository, IHubContext notify)
        {
            _notify = notify;
            _customerOrderRepository = customerOrderRepository;
        }
    }
}