using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Auth
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["uyeOturum"] == null || Session["uyeAdmin"].ToString() != "1")
            {
                filterContext.Result = new RedirectResult("~/Uyeler/OturumAc");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}