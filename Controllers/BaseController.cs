using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ForumsUnknown.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Set cache control headers
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            HttpContext.Response.Cache.SetNoStore();
        }
    }
}