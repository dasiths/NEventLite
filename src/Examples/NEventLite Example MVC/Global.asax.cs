using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NEventLite_Example.Util;
using DependencyResolver = NEventLite_Example.Util.DependencyResolver;

namespace NEventLite_Example_MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private DependencyResolver _applicationContainerInstance;

        public DependencyResolver GetDependencyResolver()
        {
            return _applicationContainerInstance ?? (_applicationContainerInstance = new DependencyResolver());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            _applicationContainerInstance?.Dispose();
        }
    }
}
