using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Aliencube.AuthorizeAttribute.Extended.WebApp.Models;

namespace Aliencube.AuthorizeAttribute.Extended.WebApp.Controllers
{
    [Authorize]
    public partial class HomeController : Controller
    {
        [AllowAnonymous]
        public virtual async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> LoginAdmin(LoginViewModel model)
        {
            var validated = await this.ValidateAsync(model, "Admin");

            return RedirectToAction(validated ? "MyProfile" : "Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> LoginUser(LoginViewModel model)
        {
            var validated = await this.ValidateAsync(model, "User");

            return RedirectToAction("MyProfile");
        }

        private async Task<bool> ValidateAsync(LoginViewModel model, string role)
        {
            if (model.Email != "test@aliencube.org" || model.Password != "password")
            {
                return false;
            }

            var now = DateTime.UtcNow;

            var ticket = new FormsAuthenticationTicket(
                                             1,                  // ticket version
                                             model.Email,        // authenticated username
                                             now,                // issueDate
                                             now.AddMinutes(30), // expiryDate
                                             model.RememberMe,   // true to persist across browser sessions
                                             role,        // can be used to store additional user data
                                             FormsAuthentication.FormsCookiePath);  // the path for the cookie

            // Encrypt the ticket using the machine key
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            // Add the cookie to the request to save it
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) { HttpOnly = true, };

            Response.Cookies.Add(cookie);

            return true;
        }

        [Authorize(Roles = "Admin")]
        public virtual async Task<ActionResult> MyProfile()
        {
            var vm = new MyProfileViewModel() { Email = this.HttpContext.User.Identity.Name };
            return View(vm);
        }

        public virtual async Task<ActionResult> Logout()
        {
            FormsAuthentication.SignOut();

            return this.Redirect("~/");
        }
    }
}