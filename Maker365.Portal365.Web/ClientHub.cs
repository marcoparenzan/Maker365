using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Maker365.Portal365.Web
{
    public class ClientHub : Hub
    {
        public static IHubContext Default
        {
            get
            {
                return
                    Microsoft.AspNet.SignalR
                    .GlobalHost
                    .ConnectionManager
                    .GetHubContext<ClientHub>();
            }
        }

        public void Info(string message)
        {
            throw new NotImplementedException("Info not implemented");
        }
    }
}
