using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Aliencube.AuthorizeAttribute.Extended.Tests
{
    /// <summary>
    /// This represents the attribute entity for authentication.
    /// </summary>
    public class AuthenticateAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private static readonly char[] _splitParameter = new[] { ',' };

        /// <summary>
        /// Provides an entry point for custom authorization checks.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>Returns <c>True</c>, if the user is authenticated; otherwise returns <c>False</c>.</returns>
        /// <remarks>This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.</remarks>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            var users = SplitString(this.Users);
            if (users.Length > 0 && !users.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles cache validation.
        /// </summary>
        /// <param name="context">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <param name="data">The user-supplied data to be passed.</param>
        /// <param name="validationStatus">A reference to the validation status.</param>
        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = this.OnCacheAuthorization(new HttpContextWrapper(context));
        }

        /// <summary>
        /// Called when a process requests authorization
        /// </summary>
        /// <param name="filterContext">The filter context, which encapsulates information for using <c>Aliencube.AuthorizeAttribute.Extended.AuthenticateAttribute</c>.</param>
        /// <remarks>
        /// Basically, the <c>Aliencube.AuthorizeAttribute.Extended.AuthenticateAttribute</c> class inherits the <c>System.Web.Mvc.AuthorizeAttribute</c> class and is only responsible for permission check for requests. If the permission is not enough, this returns the 401 (unauthorised) error.
        /// </remarks>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
            {
                // If a child action cache block is active, we need to fail immediately, even if authorization
                // would have succeeded. The reason is that there's no way to hook a callback to rerun
                // authorization before the fragment is served from the cache, so we can't guarantee that this
                // filter will be re-run on subsequent requests.
                throw new InvalidOperationException("AuthorizeAttribute cannot be used within a child action caching block.");
            }

            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                                     || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
            {
                return;
            }

            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(this.CacheValidateHandler, null /* data */);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }

        /// <summary>
        /// Called when the caching module requests authorization.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param>
        /// <returns>Returns the validation status.</returns>
        /// <remarks>This method must be thread-safe since it is called by the caching module.</remarks>
        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            bool isAuthorized = this.AuthorizeCore(httpContext);
            return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }

        /// <summary>
        /// Splits the given string into an array.
        /// </summary>
        /// <param name="original">Original string.</param>
        /// <returns>Returns an array containing the string split.</returns>
        private static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(_splitParameter)
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}