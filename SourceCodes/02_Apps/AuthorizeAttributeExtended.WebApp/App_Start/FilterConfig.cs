using System.Web.Mvc;

namespace Aliencube.AuthorizeAttribute.Extended.WebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new Web.Mvc.Extended.AuthorizeAttribute());
        }
    }
}