using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Maker365.Customers365.Contracts;
using MarcoParenzan.MicrosoftAzure.AzureStorage;
using Microsoft.AspNet.SignalR;
using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Maker365.MicrosoftAzure.AzureStorage;


namespace Maker365.Portal365.Web
{
    public class IoCConfig: IControllerFactory
    {
        //application starts...
        private WindsorContainer _container;

        public IoCConfig()
        {
            _container = new WindsorContainer();
            _container.Register(Component.For<PartitionKey>().Instance(new Func<string>(() => HttpContext.Current.User.Identity.Name)));
            _container.Register(Component.For<CloudStorageAccount>().Instance(new CloudStorageAccount(new StorageCredentials("--storagename--", "--storagekey--"), true)));
            _container.Register(Component.For<IHubContext>().Instance(ClientHub.Default));
            _container.Register(Component.For<ICustomerOrderRepository>().ImplementedBy<CustomerOrderRepository>());
            _container.Register(Component.For<ICustomerOrderQueries>().ImplementedBy<CustomerOrderQueries>());
            _container.Register(
                Classes
                .FromThisAssembly()
                .InNamespace("Maker365.Portal365.Web.Controllers", true)
                .LifestylePerWebRequest()
            );
        }

        public static void Register()
        {
            ControllerBuilder.Current.SetControllerFactory(new IoCConfig());
        }

        IController IControllerFactory.CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            //var type = Type.GetType("Maker365.Portal365.Web.Controllers." + controllerName + "Controller");
            //return (IController) Activator.CreateInstance(type);
            return _container.Resolve<IController>("Maker365.Portal365.Web.Controllers." + controllerName + "Controller");
        }

        System.Web.SessionState.SessionStateBehavior IControllerFactory.GetControllerSessionBehavior(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            return System.Web.SessionState.SessionStateBehavior.Disabled;
        }

        void IControllerFactory.ReleaseController(IController controller)
        {
        }
    }
}
