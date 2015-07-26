using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aliencube.AuthorizeAttribute.Extended.Tests.Helpers;
using Aliencube.Web.Mvc.Extended;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Aliencube.AuthorizeAttribute.Extended.Tests
{
    [TestFixture]
    public class MvcAuthorizeAttributeTest
    {
        private HttpContextBase _httpContext;
        private ControllerContext _controllerContext;
        private ActionDescriptor _actionDescriptor;
        private AuthorizationContext _authorizationContext;
        private MvcAuthorizeAttributeHelper _attribute;

        [SetUp]
        public void Init()
        {
            this._httpContext = Substitute.For<HttpContextBase>();
            this._controllerContext = Substitute.For<ControllerContext>();
            this._actionDescriptor = Substitute.For<ActionDescriptor>();

            this._attribute = new MvcAuthorizeAttributeHelper();
        }

        [TearDown]
        public void Cleanup()
        {
        }
        [Test]
        [TestCase("username1", "username1", true, true, AuthorizationStatus.Accepted)]
        [TestCase("username1,username2", "username2", true, true, AuthorizationStatus.Accepted)]
        [TestCase("username1", "username1", false, false, AuthorizationStatus.Unauthorized)]
        [TestCase("username1,username2", "username3", true, false, AuthorizationStatus.Unauthorized)]
        public void GetAuthenticated_GivenUser_ReturnAuthenticated(string users, string user, bool isAuthenticated, bool authenticated, AuthorizationStatus expectedStatus)
        {
            this._httpContext.User.Identity.IsAuthenticated.Returns(isAuthenticated);
            this._httpContext.User.Identity.Name.Returns(user);

            this._attribute.Users = users;

            AuthorizationStatus authorizationStatus;
            var result = this._attribute.PublicAuthorizeCore(this._httpContext, out authorizationStatus);
            result.Should().Be(authenticated);
            authorizationStatus.Should().Be(expectedStatus);
        }

        [Test]
        [TestCase("username", "role1", "role1", true, AuthorizationStatus.Accepted)]
        [TestCase("username", "role1,role2", "role1", true, AuthorizationStatus.Accepted)]
        [TestCase("username", "role1,role2", "role2", true, AuthorizationStatus.Accepted)]
        [TestCase("username", "role1,role2", "role3", false, AuthorizationStatus.Forbidden)]
        public void GetAuthorised_GivenUser_ReturnAuthorised(string user, string roles, string role, bool authorised, AuthorizationStatus expectedStatus)
        {
            this._httpContext.User.Identity.IsAuthenticated.Returns(true);
            this._httpContext.User.Identity.Name.Returns(user);

            var isInRole = roles.Split(',').Contains(role, StringComparer.InvariantCultureIgnoreCase);
            this._httpContext.User.IsInRole(Arg.Any<string>()).Returns(isInRole);

            this._attribute.Roles = roles;

            AuthorizationStatus authorizationStatus;
            var result = this._attribute.PublicAuthorizeCore(this._httpContext, out authorizationStatus);
            result.Should().Be(authorised);
            authorizationStatus.Should().Be(expectedStatus);
        }

        [Test]
        public void GetHttpUnauthorizedResult_GivenContext_ReturnHttpUnauthorizedResult()
        {
            this._authorizationContext = new AuthorizationContext(this._controllerContext, this._actionDescriptor);

            this._attribute.PublicHandleUnauthorizedRequest(this._authorizationContext);
            this._authorizationContext.Result.Should().BeOfType<HttpUnauthorizedResult>();
        }

        [Test]
        public void GetHttpForbiddenResult_GivenContext_ReturnHttpForbiddenResult()
        {
            this._authorizationContext = new AuthorizationContext(this._controllerContext, this._actionDescriptor);

            this._attribute.PublicHandleForbiddenRequest(this._authorizationContext);
            this._authorizationContext.Result.Should().BeOfType<HttpForbiddenResult>();
        }

        [Test]
        [TestCase("username1", "username1", true, true, AuthorizationStatus.Accepted)]
        [TestCase("username1,username2", "username2", true, true, AuthorizationStatus.Accepted)]
        [TestCase("username1", "username1", false, false, AuthorizationStatus.Unauthorized)]
        [TestCase("username1,username2", "username3", true, false, AuthorizationStatus.Unauthorized)]
        public void OnAuthorization_GivenContext_ReturnAuthenticated(string users, string user, bool isAuthenticated, bool authenticated, AuthorizationStatus expectedStatus)
        {
            this._httpContext.User.Identity.IsAuthenticated.Returns(isAuthenticated);
            this._httpContext.User.Identity.Name.Returns(user);

            this._authorizationContext = new AuthorizationContext(this._controllerContext, this._actionDescriptor);
            this._authorizationContext.HttpContext = this._httpContext;

            this._attribute.Users = users;

            this._attribute.OnAuthorization(this._authorizationContext);

            this._attribute.AuthorizationStatus.Should().Be(expectedStatus);
            if (this._attribute.AuthorizationStatus != AuthorizationStatus.Accepted)
            {
                this._authorizationContext.Result.Should().BeOfType<HttpUnauthorizedResult>();
            }
        }

        [Test]
        [TestCase("username", "role1", "role1", true, AuthorizationStatus.Accepted)]
        [TestCase("username", "role1,role2", "role1", true, AuthorizationStatus.Accepted)]
        [TestCase("username", "role1,role2", "role2", true, AuthorizationStatus.Accepted)]
        [TestCase("username", "role1,role2", "role3", false, AuthorizationStatus.Forbidden)]
        public void OnAuthorization_GivenContext_ReturnAuthorised(string user, string roles, string role, bool authorised, AuthorizationStatus expectedStatus)
        {
            this._httpContext.User.Identity.IsAuthenticated.Returns(true);
            this._httpContext.User.Identity.Name.Returns(user);

            var isInRole = roles.Split(',').Contains(role, StringComparer.InvariantCultureIgnoreCase);
            this._httpContext.User.IsInRole(Arg.Any<string>()).Returns(isInRole);

            this._authorizationContext = new AuthorizationContext(this._controllerContext, this._actionDescriptor);
            this._authorizationContext.HttpContext = this._httpContext;

            this._attribute.Roles = roles;

            this._attribute.OnAuthorization(this._authorizationContext);

            this._attribute.AuthorizationStatus.Should().Be(expectedStatus);
            if (this._attribute.AuthorizationStatus != AuthorizationStatus.Accepted)
            {
                this._authorizationContext.Result.Should().BeOfType<HttpForbiddenResult>();
            }
        }
    }
}