using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Aliencube.AuthorizeAttribute.Extended.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
            {
                return;
            }

            //Extract the forms authentication cookie
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            if (authTicket == null)
            {
                return;
            }

            var identity = new GenericIdentity(authTicket.Name, "Forms");
            var principal = new GenericPrincipal(identity, new[] { authTicket.UserData });

            // Set the context user
            Context.User = principal;
        }
    }
}