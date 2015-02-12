using System.Web;
using Aliencube.AuthorizeAttribute.Extended.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Aliencube.AuthorizeAttribute.Extended.Tests
{
    [TestFixture]
    public class AuthenticateAttributeTest
    {
        private HttpContextBase _context;
        private AuthenticateAttributeHelper _attribute;

        [SetUp]
        public void Init()
        {
            this._context = Substitute.For<HttpContextBase>();
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
        public void GetAuthorised_GivenUser_ReturnAuthorised(string users, string user, bool isAuthenticated, bool authenticated)
        {
            this._attribute.Users = users;
            this._context.User.Identity.IsAuthenticated.Returns(isAuthenticated);
            this._context.User.Identity.Name.Returns(user);

            var result = this._attribute.PublicAuthorizeCore(this._context);
            result.Should().Be(authenticated);
        }
    }
}