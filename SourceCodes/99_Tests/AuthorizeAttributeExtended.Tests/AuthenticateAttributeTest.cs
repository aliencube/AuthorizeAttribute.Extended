using System.Web;
using System.Web.Mvc;
using Aliencube.AuthorizeAttribute.Extended.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Aliencube.AuthorizeAttribute.Extended.Tests
{
    [TestFixture]
    public class AuthenticateAttributeTest
    {
        private HttpContextBase _httpContext;
        private ControllerContext _controllerContext;
        private ActionDescriptor _actionDescriptor;
        private AuthorizationContext _authorizationContext;
        private AuthenticateAttributeHelper _attribute;

        [SetUp]
        public void Init()
        {
            this._httpContext = Substitute.For<HttpContextBase>();
            this._controllerContext = Substitute.For<ControllerContext>();
            this._actionDescriptor = Substitute.For<ActionDescriptor>();

            this._attribute = new AuthenticateAttributeHelper();
        }

        [TearDown]
        public void Cleanup()
        {
        }

        [Test]
        [TestCase("username1", "username1", true, true)]
        [TestCase("username1,username2", "username2", true, true)]
        [TestCase("username1", "username1", false, false)]
        [TestCase("username1,username2", "username3", true, false)]
        public void GetAuthenticated_GivenUser_ReturnAuthenticated(string users, string user, bool isAuthenticated, bool authenticated)
        {
            this._httpContext.User.Identity.IsAuthenticated.Returns(isAuthenticated);
            this._httpContext.User.Identity.Name.Returns(user);

            this._attribute.Users = users;

            var result = this._attribute.PublicAuthenticateCore(this._httpContext);
            result.Should().Be(authenticated);
        }

        [Test]
        public void GetHttpUnauthorizedResult_GivenContext_ReturnHttpUnauthorizedResult()
        {
            this._authorizationContext = new AuthorizationContext(this._controllerContext, this._actionDescriptor);

            this._attribute.PublicHandleUnauthorizedRequest(this._authorizationContext);
            this._authorizationContext.Result.Should().BeOfType<HttpUnauthorizedResult>();
        }

        [Test]
        [TestCase("username1", "username1", true, true)]
        [TestCase("username1,username2", "username2", true, true)]
        [TestCase("username1", "username1", false, false)]
        [TestCase("username1,username2", "username3", true, false)]
        public void OnAuthorization_GivenContext_ReturnResult(string users, string user, bool isAuthenticated, bool authenticated)
        {
            this._httpContext.User.Identity.IsAuthenticated.Returns(isAuthenticated);
            this._httpContext.User.Identity.Name.Returns(user);

            this._authorizationContext = new AuthorizationContext(this._controllerContext, this._actionDescriptor);
            this._authorizationContext.HttpContext = this._httpContext;

            this._attribute.Users = users;

            this._attribute.OnAuthorization(this._authorizationContext);
            this._attribute.IsAuthenticated.Should().Be(authenticated);
        }
    }
}