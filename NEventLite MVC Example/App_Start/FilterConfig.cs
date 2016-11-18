using System.Web;
using System.Web.Mvc;

namespace NEventLite_MVC_Example
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
