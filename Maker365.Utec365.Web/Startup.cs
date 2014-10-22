using Owin;
using Microsoft.Owin;

[assembly: OwinStartupAttribute(typeof(Maker365.Utec365.Web.Startup))]
namespace Maker365.Utec365.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
