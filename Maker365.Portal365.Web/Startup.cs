using Owin;
using Microsoft.Owin;

[assembly: OwinStartupAttribute(typeof(Maker365.Portal365.Web.Startup))]
namespace Maker365.Portal365.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
