using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RalphLaurenPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace RalphLaurenPortal.Utilities
{
    public static class Extensions
    {
        public static string GetFullName(this IIdentity identity)
        {
            var user = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>().FindById(Convert.ToInt32(identity.GetUserId()));
            return (user != null) ? user.FullName ?? user.UserName : string.Empty;
        }

        public static string GetEmail(this IIdentity identity)
        {
            var user = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>().FindById(Convert.ToInt32(identity.GetUserId()));
            return (user != null) ? user.Email : string.Empty;
        }

        public static ICollection<Member_Role> GetRoles(this IIdentity identity)
        {
            var user = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>().FindById(Convert.ToInt32(identity.GetUserId()));
            return (user != null) ? user.Roles : new List<Member_Role>();
        }
    }
}