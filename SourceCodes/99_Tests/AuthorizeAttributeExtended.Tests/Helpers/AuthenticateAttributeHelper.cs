using System.Web;
using System.Web.Mvc;

namespace Aliencube.AuthorizeAttribute.Extended.Tests.Helpers
{
    /// <summary>
    /// This represents the helper entity for the <c>AuthenticateAttribute</c> class.
    /// </summary>
    public class AuthenticateAttributeHelper : AuthenticateAttribute
    {
        public virtual bool PublicAuthenticateCore(HttpContextBase httpContext)
        {
            return base.AuthenticateCore(httpContext);
        }

        protected override bool AuthenticateCore(HttpContextBase httpContext)
        {
            return this.PublicAuthenticateCore(httpContext);
        }

        public virtual HttpValidationStatus PublicOnCacheAuthentication(HttpContextBase httpContext)
        {
            return base.OnCacheAuthentication(httpContext);
        }

        protected override HttpValidationStatus OnCacheAuthentication(HttpContextBase httpContext)
        {
            return this.PublicOnCacheAuthentication(httpContext);
        }

        public virtual void PublicHandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            this.PublicHandleUnauthorizedRequest(filterContext);
        }

        public virtual new bool IsAuthenticated
        {
            get
            {
                return base.IsAuthenticated;
            }
        }
    }
}