using System.Web.Http;

namespace Aliencube.AuthorizeAttribute.Extended.WebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new Aliencube.Web.Http.Extended.AuthorizeAttribute());
        }
    }
}