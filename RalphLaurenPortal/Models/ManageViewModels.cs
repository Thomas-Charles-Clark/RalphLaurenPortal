using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using RalphLaurenPortal.ViewModels;

namespace RalphLaurenPortal.Models
{
    public class IndexViewModel : BaseViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }

        public IndexViewModel() : base(null, null, DateTime.Now)
        {

        }

        public IndexViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class ManageLoginsViewModel : BaseViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }

        public ManageLoginsViewModel() : base(null, null, DateTime.Now)
        {

        }

        public ManageLoginsViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class FactorViewModel : BaseViewModel
    {
        public string Purpose { get; set; }

        public FactorViewModel() : base(null, null, DateTime.Now)
        {

        }

        public FactorViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class SetPasswordViewModel : BaseViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public SetPasswordViewModel() : base(null, null, DateTime.Now)
        {

        }

        public SetPasswordViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class ChangePasswordViewModel : BaseViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public ChangePasswordViewModel() : base(null, null, DateTime.Now)
        {

        }

        public ChangePasswordViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class AddPhoneNumberViewModel : BaseViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }

        public AddPhoneNumberViewModel() : base(null, null, DateTime.Now)
        {

        }

        public AddPhoneNumberViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class VerifyPhoneNumberViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public VerifyPhoneNumberViewModel() : base(null, null, DateTime.Now)
        {

        }

        public VerifyPhoneNumberViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }

    public class ConfigureTwoFactorViewModel : BaseViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }

        public ConfigureTwoFactorViewModel() : base(null, null, DateTime.Now)
        {

        }

        public ConfigureTwoFactorViewModel(Member user, string title, DateTime contentDate) : base(user, title, contentDate)
        {

        }
    }
}