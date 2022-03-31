//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Net.Mail;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Microsoft.AspNet.Identity.Owin;
//using Microsoft.Owin;
//using Microsoft.Owin.Security;
//using RalphLaurenPortal.Models;

//namespace RalphLaurenPortal
//{
//    public class EmailService : IIdentityMessageService
//    {
//        public Task SendAsync(IdentityMessage message)
//        {
//            throw new NotImplementedException("Use the Foundation send");
//        }

//        public async Task SendFoundationAsync(IdentityMessage message, string replyTo = null)
//        {
//            // Plug in your email service here to send an email.
//            try
//            {
//                message.Destination = message.Destination.Replace(";", ",");

//                using (var Message = new MailMessage("portal.noreply@designersguild.com", message.Destination, message.Subject, message.Body))
//                {
//                    Message.To.Clear();

//                    foreach (var address in message.Destination.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
//                    {
//                        Message.To.Add(address);
//                    }

//                    Message.IsBodyHtml = true;

//                    Message.Body = message.Body;

//                    if (!String.IsNullOrEmpty(replyTo))
//                    {
//                        Message.ReplyToList.Add(replyTo.Replace(";", ","));
//                    }

//                    Message.Bcc.Add("tclark@designersguild.com");
//                    //Message.Bcc.Add("mfernandez@designersguild.com");

//                    using (var Client = new SmtpClient("smtp.office365.com"))
//                    {
//                        Client.Credentials = new System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");
//                        Client.EnableSsl = true;
//                        Client.Port = 587;

//                        await Client.SendMailAsync(Message);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//    }

//    public class SmsService : IIdentityMessageService
//    {
//        public Task SendAsync(IdentityMessage message)
//        {
//            // Plug in your SMS service here to send a text message.
//            return Task.FromResult(0);
//        }
//    }

//    public class PasswordHasher : IPasswordHasher
//    {
//        public string HashPassword(string password)
//        {
//            //  Rfc2898DeriveBytes is used, rather than MD5 or SHA, as this is "algorithmic slow" (harder to crack with a brute force attack)
//            System.Security.Cryptography.Rfc2898DeriveBytes hasher = new System.Security.Cryptography.Rfc2898DeriveBytes(password, System.Text.Encoding.Default.GetBytes(getSalt()), 10000);

//            return Convert.ToBase64String(hasher.GetBytes(25));
//        }

//        private string getSalt()
//        {
//            return "t!W6J39q1m3ULMK35pg2HsW_L8&fPJ(dpJn32(90B9loMblgd(-cM%5ZA0qwKzXc";
//        }

//        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
//        {
//            if (hashedPassword.Equals(HashPassword(providedPassword)))
//                return PasswordVerificationResult.Success;
//            else return PasswordVerificationResult.Failed;
//        }
//    }

//    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
//    public class ApplicationUserManager : UserManager<Member, int>
//    {
//        public ApplicationUserManager(UserStore store)
//            : base(store)
//        {
//            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("Portal");

//            PasswordHasher = new PasswordHasher();
//            UserTokenProvider = new DataProtectorTokenProvider<Member, int>(provider.Create("EmailConfirmation"));
//            EmailService = new EmailService();
//            SmsService = new SmsService();
//            UserValidator = new UserValidator<Member, int>(this)
//            {
//                RequireUniqueEmail = true,
//                AllowOnlyAlphanumericUserNames = false
//            };
//        }

//        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
//        {
//            var manager = new ApplicationUserManager(new UserStore(context.Get<PortalDbContext>()));

//            var userValidator = new UserValidator<Member, Int32>(manager);
//            userValidator.RequireUniqueEmail = true;
//            userValidator.AllowOnlyAlphanumericUserNames = false;

//            manager.UserValidator = userValidator;

//            // Configure validation logic for passwords
//            manager.PasswordValidator = new PasswordValidator
//            {
//                RequiredLength = 4,
//                RequireNonLetterOrDigit = false,
//                RequireDigit = false,
//                RequireLowercase = false,
//                RequireUppercase = false
//            };


//            // Configure user lockout defaults
//            manager.UserLockoutEnabledByDefault = true;
//            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
//            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

//            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
//            // You can write your own provider and plug it in here.
//            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<Member, Int32>
//            {
//                MessageFormat = "Your security code is {0}"
//            });
//            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<Member, Int32>
//            {
//                Subject = "Security Code",
//                BodyFormat = "Your security code is {0}"
//            });
//            manager.EmailService = new EmailService();
//            manager.SmsService = new SmsService();

//            var dataProtectionProvider = options.DataProtectionProvider;

//            if (dataProtectionProvider != null)
//            {
//                manager.UserTokenProvider =
//                    new DataProtectorTokenProvider<Member, Int32>(dataProtectionProvider.Create("ASP.NET Identity"));
//            }

//            return manager;
//        }
//    }

//    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
//    public class ApplicationRoleManager : RoleManager<IdentityRole>
//    {
//        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
//            : base(roleStore)
//        {
//        }

//        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
//        {
//            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<PortalDbContext>()));

//            return manager;
//        }
//    }


//    // This is useful if you do not want to tear down the database each time you run the application.
//    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
//    // This example shows you how to create a new database if the Model changes
//    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<PortalDbContext>
//    {
//        protected override void Seed(PortalDbContext context)
//        {
//            InitializeIdentityForEF(context);
//            base.Seed(context);
//        }

//        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
//        public static void InitializeIdentityForEF(PortalDbContext db)
//        {
//            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
//            const string name = "admin@admin.com";
//            const string password = "Admin@123456";
//            const string roleName = "Admin";

//            //Create Role Admin if it does not exist
//            var role = roleManager.FindByName(roleName);
//            if (role == null)
//            {
//                role = new IdentityRole(roleName);
//                var roleresult = roleManager.Create(role);
//            }

//            var user = userManager.FindByName(name);
//            if (user == null)
//            {
//                user = new Member { Email = name };
//                var result = userManager.Create(user, password);
//                result = userManager.SetLockoutEnabled(user.Id, false);
//            }

//            // Add user admin to Role Admin if not already added
//            var rolesForUser = userManager.GetRoles(user.Id);
//            if (!rolesForUser.Contains(role.Name))
//            {
//                var result = userManager.AddToRole(user.Id, role.Name);
//            }
//        }
//    }

//    // Configure the application sign-in manager which is used in this application.
//    public class ApplicationSignInManager : SignInManager<Member, Int32>
//    {
//        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
//            : base(userManager, authenticationManager)
//        {
//        }

//        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Member user)
//        {
//            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
//        }

//        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
//        {
//            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
//        }
//    }
//}


using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Net.Mail;
using System.IO;
using System.Collections.Generic;
using RalphLaurenPortal.Utilities;
using RalphLaurenPortal.Models;
using System.Data.Entity.SqlServer.Utilities;

namespace RalphLaurenPortal
{
    public class EmailService : IIdentityMessageService
    {
        public async System.Threading.Tasks.Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            try
            {
                message.Destination = message.Destination.Replace(";", ",");

                using (var Message = new MailMessage("portal.noreply@designersguild.com", message.Destination, message.Subject, message.Body))
                {
                    Message.To.Clear();

                    foreach (var address in message.Destination.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Message.To.Add(address);
                    }

                    Message.IsBodyHtml = true;

                    Message.Body = $@"
                        <html>
                        <head>
                        <style type='text/css'>
                            {Framework.EmailCSS}
                        </style>
                        </head>
                        <body>
                        <div>
                            <img class='header-logo' width='800' alt='Designers Guild' src='http://hosting-source.bm23.com/37124/public/Transactional/header-logo.jpg' title='Designers Guild' />
                        </div>
                        { Message.Body}
                        <div>
                            <img class='footer-logo' width='200' alt='Designers Guild' src='http://hosting-source.bm23.com/37124/public/Transactional/footer-logo.jpg' title='Designers Guild' />
                        </div>
                        </body>
                        </html>";

                    Message.Bcc.Add("tclark@designersguild.com");
                    //Message.Bcc.Add("mfernandez@designersguild.com");

                    using (var Client = new SmtpClient("smtp.office365.com"))
                    {
                        Client.Credentials = new System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");
                        Client.EnableSsl = true;
                        Client.Port = 587;

                        await Client.SendMailAsync(Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async System.Threading.Tasks.Task SendAsyncWithBCC(IdentityMessage message, string bcc)
        {
            // Plug in your email service here to send an email.
            try
            {
                message.Destination = message.Destination.Replace(";", ",");

                using (var Message = new MailMessage("portal.noreply@designersguild.com", message.Destination, message.Subject, message.Body))
                {
                    Message.To.Clear();

                    foreach (var address in message.Destination.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Message.To.Add(address);
                    }

                    Message.IsBodyHtml = true;

                    Message.Body = $@"
                        <html>
                        <head>
                        <style type='text/css'>
                            {Framework.EmailCSS}
                        </style>
                        </head>
                        <body>
                        <div>
                            <img class='header-logo' width='800' alt='Designers Guild' src='http://hosting-source.bm23.com/37124/public/Transactional/header-logo.jpg' title='Designers Guild' />
                        </div>
                        { Message.Body}
                        <div>
                            <img class='footer-logo' width='200' alt='Designers Guild' src='http://hosting-source.bm23.com/37124/public/Transactional/footer-logo.jpg' title='Designers Guild' />
                        </div>
                        </body>
                        </html>";

                    Message.Bcc.Add("tclark@designersguild.com");
                    Message.Bcc.Add(bcc);
                    //Message.Bcc.Add("mfernandez@designersguild.com");

                    using (var Client = new SmtpClient("smtp.office365.com"))
                    {
                        Client.Credentials = new System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");
                        Client.EnableSsl = true;
                        Client.Port = 587;

                        await Client.SendMailAsync(Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async System.Threading.Tasks.Task SendFoundationEmailWithAttachmentsAsync(IdentityMessage message, Dictionary<string, MemoryStream> attachments, string replyTo = null)
        {
            // Plug in your email service here to send an email.
            try
            {
                message.Destination = message.Destination.Replace(";", ",");

                using (var Message = new MailMessage("portal.noreply@designersguild.com", message.Destination, message.Subject, message.Body))
                {
                    Message.To.Clear();

                    foreach (var address in message.Destination.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Message.To.Add(address);
                    }

                    Message.IsBodyHtml = true;

                    Message.Body = message.Body;

                    if (!String.IsNullOrEmpty(replyTo))
                    {
                        Message.ReplyToList.Add(replyTo.Replace(";", ","));
                    }

                    Message.Bcc.Add("tclark@designersguild.com");

                    foreach (KeyValuePair<String, MemoryStream> attachment in attachments)
                    {
                        Message.Attachments.Add(new Attachment(attachment.Value, attachment.Key));
                    }

                    using (var Client = new SmtpClient("smtp.office365.com"))
                    {
                        Client.Credentials = new System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");
                        Client.EnableSsl = true;
                        Client.Port = 587;

                        await Client.SendMailAsync(Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async System.Threading.Tasks.Task SendFoundationEmailAsync(IdentityMessage message, string replyTo = null)
        {
            // Plug in your email service here to send an email.
            try
            {
                message.Destination = message.Destination.Replace(";", ",");

                using (var Message = new MailMessage("portal.noreply@designersguild.com", message.Destination, message.Subject, message.Body))
                {
                    Message.To.Clear();

                    foreach (var address in message.Destination.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Message.To.Add(address);
                    }

                    Message.IsBodyHtml = true;

                    Message.Body = message.Body;

                    if (!String.IsNullOrEmpty(replyTo))
                    {
                        Message.ReplyToList.Add(replyTo.Replace(";", ","));
                    }

                    Message.Bcc.Add("tclark@designersguild.com");
                    //Message.Bcc.Add("mfernandez@designersguild.com");

                    using (var Client = new SmtpClient("smtp.office365.com"))
                    {
                        Client.Credentials = new System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");
                        Client.EnableSsl = true;
                        Client.Port = 587;

                        await Client.SendMailAsync(Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public System.Threading.Tasks.Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return System.Threading.Tasks.Task.FromResult(0);
        }
    }

    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            //  Rfc2898DeriveBytes is used, rather than MD5 or SHA, as this is "algorithmic slow" (harder to crack with a brute force attack)
            System.Security.Cryptography.Rfc2898DeriveBytes hasher = new System.Security.Cryptography.Rfc2898DeriveBytes(password, System.Text.Encoding.Default.GetBytes(getSalt()), 10000);

            return Convert.ToBase64String(hasher.GetBytes(25));
        }

        private string getSalt()
        {
            return "t!W6J39q1m3ULMK35pg2HsW_L8&fPJ(dpJn32(90B9loMblgd(-cM%5ZA0qwKzXc";
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(HashPassword(providedPassword)))
                return PasswordVerificationResult.Success;
            else return PasswordVerificationResult.Failed;
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<Member, Int32>
    {
        public ApplicationUserManager(IUserStore<Member, int> store)
            : base(store)
        {
            var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("RalphLaurenPortal");

            PasswordHasher = new PasswordHasher();
            UserTokenProvider = new DataProtectorTokenProvider<Member, int>(provider.Create("EmailConfirmation"));
            EmailService = new EmailService();
            SmsService = new SmsService();
            UserValidator = new UserValidator<Member, int>(this)
            {
                RequireUniqueEmail = true,
                AllowOnlyAlphanumericUserNames = false
            };
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore(context.Get<PortalDbContext>()));

            var userValidator = new UserValidator<Member, Int32>(manager);
            userValidator.RequireUniqueEmail = true;
            userValidator.AllowOnlyAlphanumericUserNames = false;

            manager.UserValidator = userValidator;

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };


            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<Member, Int32>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<Member, Int32>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;

            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<Member, Int32>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<PortalDbContext>()));

            return manager;
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<PortalDbContext>
    {
        protected override void Seed(PortalDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(PortalDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@admin.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new Member { Email = name };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<Member, Int32>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Member user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        //public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        //{
        //    SignInStatus signInStatus;

        //    if (this.UserManager != null)
        //    {
        //        /// changed to username instead of email address
        //        var user = await this.UserManager.FindByNameAsync(userName);

        //        if (user != null)
        //        {
        //            var lockoutEnabled = await this.UserManager.GetLockoutEnabledAsync(user.Id);

        //            if (lockoutEnabled)
        //            {
        //                var lockedOutCurrently = await this.UserManager.IsLockedOutAsync(user.Id);

        //                if (!lockedOutCurrently)
        //                {
        //                    var passwordCheck = await this.UserManager.CheckPasswordAsync(user, password);

        //                    if (!passwordCheck)
        //                    {
        //                        if (shouldLockout)
        //                        {
        //                            var accessFailedCount = await this.UserManager.AccessFailedAsync(user.Id);

        //                            var lockedOutNow = await this.UserManager.IsLockedOutAsync(user.Id);

        //                            if (lockedOutNow)
        //                            {
        //                                signInStatus = SignInStatus.LockedOut;
        //                                return signInStatus;
        //                            }
        //                        }

        //                        signInStatus = SignInStatus.Failure;
        //                    }
        //                    else
        //                    {
        //                        await this.UserManager.ResetAccessFailedCountAsync(user.Id);
        //                        signInStatus = SignInStatus.Success;
        //                    }
        //                }
        //                else
        //                {
        //                    signInStatus = SignInStatus.LockedOut;
        //                }
        //            }
        //            else
        //            {
        //                var flockPasswordCheck = await this.UserManager.CheckPasswordAsync(user, password);

        //                if (!flockPasswordCheck)
        //                {
        //                    signInStatus = SignInStatus.Failure;
        //                }
        //                else
        //                {
        //                    signInStatus = SignInStatus.Success;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            signInStatus = SignInStatus.Failure;
        //        }
        //    }
        //    else
        //    {
        //        signInStatus = SignInStatus.Failure;
        //    }

        //    return signInStatus;
        //}

        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            if (UserManager == null)
            {
                return SignInStatus.Failure;
            }

            var user = await UserManager.FindByNameAsync(userName).WithCurrentCulture();

            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
            {
                return SignInStatus.LockedOut;
            }
            if (await UserManager.CheckPasswordAsync(user, password).WithCurrentCulture())
            {
                if (!await IsTwoFactorEnabled(user))
                {
                    await UserManager.ResetAccessFailedCountAsync(user.Id).WithCurrentCulture();
                }
                return await SignInOrTwoFactor(user, isPersistent).WithCurrentCulture();
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user.Id).WithCurrentCulture();
                if (await UserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
                {
                    return SignInStatus.LockedOut;
                }
            }
            return SignInStatus.Failure;
        }

        private async Task<bool> IsTwoFactorEnabled(Member user)
        {
            return await UserManager.GetTwoFactorEnabledAsync(user.Id).WithCurrentCulture()
                && (await UserManager.GetValidTwoFactorProvidersAsync(user.Id).WithCurrentCulture()).Count > 0;
        }

        private async Task<SignInStatus> SignInOrTwoFactor(Member user, bool isPersistent)
        {
            var id = Convert.ToString(user.Id);
            if (await IsTwoFactorEnabled(user) && !await AuthenticationManager.TwoFactorBrowserRememberedAsync(id).WithCurrentCulture())
            {
                var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
                AuthenticationManager.SignIn(identity);
                return SignInStatus.RequiresVerification;
            }
            await SignInAsync(user, isPersistent, false).WithCurrentCulture();
            return SignInStatus.Success;
        }
    }
}