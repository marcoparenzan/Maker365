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


namespace Maker365.Utec365.Web
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
            _container.Register(Component.For<IUtecCustomerOrderQueries>().ImplementedBy<UtecCustomerOrderQueries>());
            _container.Register(
                Classes
                .FromThisAssembly()
                .InNamespace("Maker365.Utec365.Web.Controllers", true)
                .LifestylePerWebRequest()
            );
            _container.Register(
                Classes
                .FromAssembly(typeof(MarcoParenzan.Office365.API.Controllers.Office365Controller).Assembly)
                .InNamespace("MarcoParenzan.Office365.API.Controllers", true)
                .LifestylePerWebRequest()
            );
        }

        public static void Register()
        {
            ControllerBuilder.Current.SetControllerFactory(new IoCConfig());
        }

        IController IControllerFactory.CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            if (controllerName.StartsWith("Office365"))
            {
                try
                {
                    return _container.Resolve<IController>("MarcoParenzan.Office365.API.Controllers." + controllerName + "Controller");
                }
                catch
                {
                    return null;
                }
            }
            try
            {
                return _container.Resolve<IController>("Maker365.Utec365.Web.Controllers." + controllerName + "Controller");
            }
            catch
            {
                return null;
            }
        }

        System.Web.SessionState.SessionStateBehavior IControllerFactory.GetControllerSessionBehavior(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            return System.Web.SessionState.SessionStateBehavior.Default;
        }

        void IControllerFactory.ReleaseController(IController controller)
        {
        }

    }
}
