using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
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

            GlobalConfiguration.Configure(WebApiConfig.Register);
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

            var genericIdentity = new GenericIdentity(authTicket.Name, "Forms");
            var genericPrincipal = new GenericPrincipal(genericIdentity, new[] { authTicket.UserData });

            // Set the context user
            Thread.CurrentPrincipal = genericPrincipal;
            Context.User = genericPrincipal;

            //var claim = new Claim(ClaimTypes.Name, genericIdentity.Name);
            //var claims = new List<Claim>() {claim};
            //var claimsIdentity = new ClaimsIdentity(claims);
            //var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //Thread.CurrentPrincipal = claimsPrincipal;
        }
    }
}