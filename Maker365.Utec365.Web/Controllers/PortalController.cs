using System.Web.Mvc;
using Maker365.Customers365.Contracts;
using Microsoft.AspNet.SignalR;
using Maker365.Customers365.Commands;
using System;

namespace Maker365.Utec365.Web.Controllers
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
            return RedirectToAction("CustomerOrders");
        }
        public ActionResult CustomerOrders()
        {
            return View();
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public ActionResult Customers()
        {
            return View();
        }

        public ActionResult Orders()
        {
            return View();
        }

        public ActionResult ManageCustomer(string displayName)
        {
            ViewBag.displayName = displayName;

            return View();
        }


        public ActionResult ManageCustomerOrder(string partitionKey, string modelReferenceName)
        {
            ViewBag.partitionKey = partitionKey;
            ViewBag.modelReferenceName = modelReferenceName;

            return View();
        }
    }
}