using System.Web;
using System.Web.Mvc;
using Aliencube.Web.Mvc.Extended;

namespace Aliencube.AuthorizeAttribute.Extended.Tests.Helpers
{
    /// <summary>
    /// This represents the helper entity for the <c>AuthorizeAttribute</c> class.
    /// </summary>
    public class MvcAuthorizeAttributeHelper : Web.Mvc.Extended.AuthorizeAttribute
    {
        public virtual bool PublicAuthorizeCore(HttpContextBase httpContext, out AuthorizationStatus authorizationStatus)
        {
            return base.AuthorizeCore(httpContext, out authorizationStatus);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext, out AuthorizationStatus authorizationStatus)
        {
            return this.PublicAuthorizeCore(httpContext, out authorizationStatus);
        }

        public virtual HttpValidationStatus PublicOnCacheAuthorization(HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }

        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            return this.PublicOnCacheAuthorization(httpContext);
        }

        public virtual void PublicHandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            this.PublicHandleUnauthorizedRequest(filterContext);
        }

        public virtual void PublicHandleForbiddenRequest(AuthorizationContext filterContext)
        {
            base.HandleForbiddenRequest(filterContext);
        }

        protected override void HandleForbiddenRequest(AuthorizationContext filterContext)
        {
            this.PublicHandleForbiddenRequest(filterContext);
        }

        public new virtual AuthorizationStatus AuthorizationStatus
        {
            get
            {
                return base.AuthorizationStatus;
            }
        }
    }
}