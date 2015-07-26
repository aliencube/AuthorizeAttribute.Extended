using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Aliencube.Web.Http.Extended
{
    /// <summary>
    /// This represents the attribute entity for authentication.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : AuthorizationFilterAttribute
    {
        private static readonly string[] _emptyArray = new string[0];

        private readonly object _typeId = new object();

        private string _roles;
        private string[] _rolesSplit = _emptyArray;
        private string _users;
        private string[] _usersSplit = _emptyArray;

        /// <summary>
        /// Gets or sets the authorized roles.
        /// </summary>
        /// <value>
        /// The roles string.
        /// </value>
        /// <remarks>Multiple role names can be specified using the comma character as a separator.</remarks>
        public string Roles
        {
            get { return _roles ?? String.Empty; }
            set
            {
                _roles = value;
                _rolesSplit = SplitString(value);
            }
        }

        /// <summary>
        /// Gets a unique identifier for this <see cref="T:System.Attribute"/>.
        /// </summary>
        /// <returns>The unique identifier for the attribute.</returns>
        public override object TypeId
        {
            get { return _typeId; }
        }

        /// <summary>
        /// Gets or sets the authorized users.
        /// </summary>
        /// <value>
        /// The users string.
        /// </value>
        /// <remarks>Multiple role names can be specified using the comma character as a separator.</remarks>
        public string Users
        {
            get { return _users ?? String.Empty; }
            set
            {
                _users = value;
                _usersSplit = SplitString(value);
            }
        }

        /// <summary>
        /// Gets the authorisation status value.
        /// </summary>
        protected AuthorizationStatus AuthorizationStatus { get; private set; }

        /// <summary>
        /// Determines whether access for this particular request is authorized. This method uses the user <see cref="IPrincipal"/>
        /// returned via <see cref="HttpRequestContext.Principal"/>. Authorization is denied if the user is not authenticated,
        /// the user is not in the authorized group of <see cref="Users"/> (if defined), or if the user is not in any of the authorized
        /// <see cref="Roles"/> (if defined).
        /// </summary>
        /// <param name="actionContext">The context.</param>
        /// <param name="authorizationStatus"><c>AuthorizationStatus</c> value.</param>
        /// <returns><c>true</c> if access is authorized; otherwise <c>false</c>.</returns>
        protected virtual bool IsAuthorized(HttpActionContext actionContext, out AuthorizationStatus authorizationStatus)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            IPrincipal user = actionContext.ControllerContext.RequestContext.Principal;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                authorizationStatus = AuthorizationStatus.Unauthorized;
                return false;
            }

            if (_usersSplit.Length > 0 && !_usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            {
                authorizationStatus = AuthorizationStatus.Unauthorized;
                return false;
            }

            if (_rolesSplit.Length > 0 && !_rolesSplit.Any(user.IsInRole))
            {
                authorizationStatus = AuthorizationStatus.Forbidden;
                return false;
            }

            authorizationStatus = AuthorizationStatus.Accepted;
            return true;
        }

        /// <summary>
        /// Called when an action is being authorized. This method uses the user <see cref="IPrincipal"/>
        /// returned via <see cref="HttpRequestContext.Principal"/>. Authorization is denied if
        /// - the request is not associated with any user.
        /// - the user is not authenticated,
        /// - the user is authenticated but is not in the authorized group of <see cref="Users"/> (if defined), or if the user
        /// is not in any of the authorized <see cref="Roles"/> (if defined).
        ///
        /// If authorization is denied then this method will invoke <see cref="HandleUnauthorizedRequest(HttpActionContext)"/> to process the unauthorized request.
        /// </summary>
        /// <remarks>You can use <see cref="AllowAnonymousAttribute"/> to cause authorization checks to be skipped for a particular
        /// action or controller.</remarks>
        /// <seealso cref="IsAuthorized(HttpActionContext, out Extended.AuthorizationStatus)" />
        /// <param name="actionContext">The context.</param>
        /// <exception cref="ArgumentNullException">The context parameter is null.</exception>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (SkipAuthorization(actionContext))
            {
                return;
            }

            AuthorizationStatus authorizationStatus;
            if (!IsAuthorized(actionContext, out authorizationStatus))
            {
                this.AuthorizationStatus = authorizationStatus;

                switch (this.AuthorizationStatus)
                {
                    case AuthorizationStatus.Unauthorized:
                        this.HandleUnauthorizedRequest(actionContext);
                        break;

                    case AuthorizationStatus.Forbidden:
                        this.HandleForbiddenRequest(actionContext);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid authorization status");
                }
            }
        }

        /// <summary>
        /// Processes requests that fail authorization. This default implementation creates a new response with the
        /// Unauthorized status code. Override this method to provide your own handling for unauthorized requests.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Authorization has been denied for this request.");
        }

        /// <summary>
        /// Processes HTTP requests that fail authentication.
        /// </summary>
        /// <param name="actionContext">The context.</param>
        protected virtual void HandleForbiddenRequest(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "This request has been forbidden.");
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        /// <summary>
        /// Splits the string on commas and removes any leading/trailing whitespace from each result item.
        /// </summary>
        /// <param name="original">The input string.</param>
        /// <returns>An array of strings parsed from the input <paramref name="original"/> string.</returns>
        internal static string[] SplitString(string original)
        {
            if (String.IsNullOrEmpty(original))
            {
                return _emptyArray;
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !String.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}