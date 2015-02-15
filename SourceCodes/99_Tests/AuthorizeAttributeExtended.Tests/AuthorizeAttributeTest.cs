using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aliencube.AuthorizeAttribute.Extended.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Aliencube.AuthorizeAttribute.Extended.Tests
{
    [TestFixture]
    public class AuthorizeAttributeTest
    {
        private HttpContextBase _httpContext;
        private ControllerContext _controllerContext;
        private ActionDescriptor _actionDescriptor;
        private AuthorizationContext _authorizationContext;
        private AuthorizeAttributeHelper _attribute;

        [SetUp]
        public void Init()
        {
            this._httpContext = Substitute.For<HttpContextBase>();
            this._controllerContext = Substitute.For<ControllerContext>();
            this._actionDescriptor = Substitute.For<ActionDescriptor>();

            this._attribute = new AuthorizeAttributeHelper();
        }

        [TearDown]
        public void Cleanup()
        {
        }

        [Test]
        [TestCase("username", "role1", "role1", true)]
        [TestCase("username", "role1,role2", "role1", true)]
        [TestCase("username", "role1,role2", "role2", true)]
        [TestCase("username", "role1,role2", "role3", false)]
        public void GetAuthorised_GivenUser_ReturnAuthorised(string user, string roles, string role, bool authorised)
        {
            this._httpContext.User.Identity.IsAuthenticated.Returns(true);
            this._httpContext.User.Identity.Name.Returns(user);

            var isInRole = roles.Split(',').Contains(role, StringComparer.InvariantCultureIgnoreCase);
            this._httpContext.User.IsInRole(Arg.Any<string>()).Returns(isInRole);

            this._attribute.Roles = roles;

            var result = this._attribute.PublicAuthorizeCore(this._httpContext);
            result.Should().Be(authorised);
        }

        [Test]
        public void GetHttpForbiddenResult_GivenContext_ReturnHttpForbiddenResult()
        {
            this._authorizationContext = new AuthorizationContext(this._controllerContext, this._actionDescriptor);

            this._attribute.PublicHandleForbiddenRequest(this._authorizationContext);
            this._authorizationContext.Result.Should().BeOfType<HttpForbiddenResult>();
        }

        [Test]
        [TestCase("username", "role1", "role1", true)]
        [TestCase("username", "role1,role2", "role1", true)]
        [TestCase("username", "role1,role2", "role2", true)]
        [TestCase("username", "role1,role2", "role3", false)]
        public void OnAuthorization_GivenContext_ReturnResult(string user, string roles, string role, bool authorised)
        {
            this._httpContext.User.Identity.IsAuthenticated.Returns(true);
            this._httpContext.User.Identity.Name.Returns(user);

            var isInRole = roles.Split(',').Contains(role, StringComparer.InvariantCultureIgnoreCase);
            this._httpContext.User.IsInRole(Arg.Any<string>()).Returns(isInRole);

            this._authorizationContext = new AuthorizationContext(this._controllerContext, this._actionDescriptor);
            this._authorizationContext.HttpContext = this._httpContext;

            this._attribute.Roles = roles;

            this._attribute.OnAuthorization(this._authorizationContext);
            this._attribute.IsAuthorized.Should().Be(authorised);
        }
    }
}