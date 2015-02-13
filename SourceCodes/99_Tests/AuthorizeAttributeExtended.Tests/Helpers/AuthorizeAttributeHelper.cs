using System.Web;
using System.Web.Mvc;

namespace Aliencube.AuthorizeAttribute.Extended.Tests.Helpers
{
    /// <summary>
    /// This represents the helper entity for the <c>AuthorizeAttribute</c> class.
    /// </summary>
    public class AuthorizeAttributeHelper : AuthorizeAttribute
    {
        public virtual bool PublicAuthorizeCore(HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return this.PublicAuthorizeCore(httpContext);
        }

        public virtual HttpValidationStatus PublicOnCacheAuthorization(HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }

        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            return this.PublicOnCacheAuthorization(httpContext);
        }

        public virtual void PublicHandleForbiddenRequest(AuthorizationContext filterContext)
        {
            base.HandleForbiddenRequest(filterContext);
        }

        protected override void HandleForbiddenRequest(AuthorizationContext filterContext)
        {
            this.PublicHandleForbiddenRequest(filterContext);
        }

        public virtual new bool IsAuthorized
        {
            get
            {
                return base.IsAuthorized;
            }
        }
    }
}