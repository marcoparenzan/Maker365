using System.Web.Mvc;
using Maker365.Customers365.Contracts;
using Microsoft.AspNet.SignalR;
using Maker365.Customers365.Commands;
using System;

namespace Maker365.Portal365.Web.Controllers
{
    public class PortalController : Controller
    {
        private IHubContext _notify;
        private ICustomerOrderRepository _customerOrderRepository;
        public PortalController(ICustomerOrderRepository customerOrderRepository, IHubContext notify)
        {
            _notify = notify;
            _customerOrderRepository = customerOrderRepository;
        }

        // GET: Portal
        public ActionResult Index()
        {
            return RedirectToAction("YourOrders");
        }
        public ActionResult YourOrders()
        {
            return View();
        }
        public ActionResult NewOrder()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(NewCustomerOrderCommand viewModel)
        {
            if (Request.Files.Count == 0)
                throw new Exception(); // divided by zero

            _customerOrderRepository.Insert(viewModel.ModelReferenceName, Request.Files[0].InputStream, Request.Files[0].FileName);

            _notify.Clients.All.info(string.Format("\"{0}\" inserted.", viewModel.ModelReferenceName));

            return RedirectToAction("YourOrders");
        }
    }
}