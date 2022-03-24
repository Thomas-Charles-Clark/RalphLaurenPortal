using RalphLaurenPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RalphLaurenPortal.Models
{
    public class ExternalLoginConfirmationViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public ExternalLoginConfirmationViewModel() : base(null, null, DateTime.Now)
        {

        }

        public ExternalLoginConfirmationViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class ExternalLoginListViewModel : BaseViewModel
    {
        public string ReturnUrl { get; set; }

        public ExternalLoginListViewModel() : base(null, null, DateTime.Now)
        {

        }

        public ExternalLoginListViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class SendCodeViewModel : BaseViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }

        public SendCodeViewModel() : base(null, null, DateTime.Now)
        {

        }

        public SendCodeViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class VerifyCodeViewModel : BaseViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }

        public VerifyCodeViewModel() : base(null, null, DateTime.Now)
        {

        }

        public VerifyCodeViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class ForgotViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public ForgotViewModel() : base(null, null, DateTime.Now)
        {

        }

        public ForgotViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class LoginViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public LoginViewModel() : base(null, null, DateTime.Now)
        {

        }
        
        public LoginViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class RegisterViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public RegisterViewModel() : base(null, null, DateTime.Now)
        {

        }

        public RegisterViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class ResetPasswordViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public ResetPasswordViewModel() : base(null, null, DateTime.Now)
        {

        }

        public ResetPasswordViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class ForgotPasswordViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public ForgotPasswordViewModel() : base(null, null, DateTime.Now)
        {

        }

        public ForgotPasswordViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }
}
