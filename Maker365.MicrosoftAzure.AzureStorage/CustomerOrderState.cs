using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maker365.MicrosoftAzure.AzureStorage
{
    public enum CustomerOrderState
    {
        Submitted
        ,
        Managed
        ,
        Production
        ,
        Ready
        ,
        Closed
    }
}