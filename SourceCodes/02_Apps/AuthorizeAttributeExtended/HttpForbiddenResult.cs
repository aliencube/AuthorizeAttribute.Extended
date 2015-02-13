using System.Net;
using System.Web.Mvc;

namespace Aliencube.AuthorizeAttribute.Extended
{
    /// <summary>
    /// Represents the result of a forbidden HTTP request.
    /// </summary>
    public class HttpForbiddenResult : HttpStatusCodeResult
    {
        /// <summary>
        /// Initialises a new instance of the <c>Aliencube.AuthorizeAttribute.Extended.HttpForbiddenResult</c> class.
        /// </summary>
        public HttpForbiddenResult()
            : this(null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <c>Aliencube.AuthorizeAttribute.Extended.HttpForbiddenResult</c> class using the status description.
        /// </summary>
        /// <param name="statusDescription">The status description.</param>
        /// <remarks>
        /// Forbidden is equivalent to HTTP status 403, the status code for forbidden access. Other code might intercept this and perform some special logic. For example, the FormsAuthenticationModule looks for 403 responses and instead redirects the user to the login page.
        /// </remarks>
        public HttpForbiddenResult(string statusDescription)
            : base(HttpStatusCode.Forbidden, statusDescription)
        {
        }
    }
}