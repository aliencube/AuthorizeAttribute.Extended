using System.Web;

namespace Aliencube.AuthorizeAttribute.Extended.Tests.Helpers
{
    /// <summary>
    /// This represents the helper entity for the <c>AuthenticateAttribute</c> class.
    /// </summary>
    public class AuthenticateAttributeHelper : AuthenticateAttribute
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
    }
}