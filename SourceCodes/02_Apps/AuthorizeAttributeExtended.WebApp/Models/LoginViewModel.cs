using System.ComponentModel.DataAnnotations;

namespace Aliencube.AuthorizeAttribute.Extended.WebApp.Models
{
    /// <summary>
    /// This represents the view model entity for login.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the value that specifies whether to store login data or not.
        /// </summary>
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// This represents the view model entity for my-profile.
    /// </summary>
    public class MyProfileViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }
    }
}