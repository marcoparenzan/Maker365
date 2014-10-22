using Maker365.Customers365.Commands;
using Maker365.Customers365.Contracts;
using Maker365.Portal365.Web.Tools;
using Microsoft.AspNet.SignalR;
using System.Web.Mvc;

namespace Maker365.Portal365.Web.Controllers
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

        [MapFileToViewModelProperty(ParameterName="viewModel", StreamPropertyName="ModelStream", ModelNameProperty="ModelFileName")]
        public void NewOrder(NewCustomerOrderCommand viewModel)
        {
            _customerOrderRepository.Insert(viewModel.ModelReferenceName, viewModel.ModelStream, viewModel.ModelFileName);

            _notify.Clients.All.success(string.Format("\"{0}\" inserted.", viewModel.ModelReferenceName));
        }
    }
}