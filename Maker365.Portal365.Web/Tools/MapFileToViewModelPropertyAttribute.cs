using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Maker365.Portal365.Web.Tools
{
    public class MapFileToViewModelPropertyAttribute: ActionFilterAttribute
    {
        public string ParameterName { get; set; }
        public string StreamPropertyName { get; set; }
        public string ModelNameProperty { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var param = filterContext.ActionParameters[ParameterName];
            param.GetType().GetProperty(StreamPropertyName).SetValue(param, filterContext.RequestContext.HttpContext.Request.Files[0].InputStream);
            param.GetType().GetProperty(ModelNameProperty).SetValue(param, filterContext.RequestContext.HttpContext.Request.Files[0].FileName);
        }
    }
}