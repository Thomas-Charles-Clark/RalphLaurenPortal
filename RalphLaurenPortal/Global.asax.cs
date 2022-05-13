using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using Unity.AspNet.Mvc;

namespace RalphLaurenPortal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UnityConfig.RegisterTypes(UnityConfig.Container);
            DependencyResolver.SetResolver(new UnityDependencyResolver(UnityConfig.Container));
        }

        protected void Session_Start()
        {
            HttpContext.Current.Session.Add("TradeAccountID", (int?)null);
            HttpContext.Current.Session.Add("KeepSessionAlive", DateTime.Now.ToString());
            HttpContext.Current.Session.Add("DisplayTradePrice", false);
            HttpContext.Current.Session.Add("DisplayRetailPrice", false);
        }
    }
}
