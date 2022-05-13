using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;

namespace RalphLaurenPortal.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    [Table("Member", Schema = "RalphLauren")]
    public class Member : IdentityUser<int, Member_Login, Member_Role, Member_Claim>
    {
        public Member()
        {
            Roles = new List<Member_Role>();
        }

        public string FullName { get; set; }

        public int MemberID
        {
            get
            {
                return Id;
            }
        }

        public int TradeAccountID { get; set; }

        public string DotNetCulture { get; set; }

        public short CountryID { get; set; }

        public byte LanguageID { get; set; }

        public short RRPPriceListID { get; set; }

        public short TradePriceListID { get; set; }

        public DateTime? DeletedDT { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool Deleted { get; set; }

        public bool AllowOrderStatus { get; set; }

        public bool AllowSalesHistory { get; set; }

        [JsonIgnore]
        public override ICollection<Member_Role> Roles { get; }

        [ForeignKey("TradeAccountID")]
        public virtual TradeAccount.TradeAccount TradeAccount { get; set; }

        [JsonIgnore]
        [InverseProperty("Member")]
        public virtual ICollection<Member_AvailableTradeAccount> AvailableTradeAccounts { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    [Table("Member_TradeAccount", Schema = "RalphLauren")]
    public class Member_AvailableTradeAccount // Users have access to multiple trade accounts
    {
        [Key]
        [Column(Order = 1)]
        public int MemberID { get; set; }

        [Key]
        [Column(Order = 2)]
        public int TradeAccountID { get; set; }

        public bool DisplayTradePrice { get; set; }

        public bool DisplayRetailPrice { get; set; }

        [ForeignKey("MemberID")]
        public virtual Member Member { get; set; }

        [ForeignKey("TradeAccountID")]
        public virtual TradeAccount.TradeAccount TradeAccount { get; set; }
    }


    // New derived classes
    [Table("Member_Role", Schema = "RalphLauren")]
    public class Member_Role : IdentityUserRole<int> // user belonging to a role
    {
        public virtual Member Member { get; set; }
        public virtual Role Role { get; set; }
    }

    [Table("Member_Claim", Schema = "RalphLauren")]
    public class Member_Claim : IdentityUserClaim<int> // one specific user claim
    {

    }

    [Table("Member_Login", Schema = "RalphLauren")]
    public class Member_Login : IdentityUserLogin<int> // type for a user's login (i.e. facebook, google)
    {

    }

    [Table("Role", Schema = "RalphLauren")]
    public class Role : IdentityRole<int, Member_Role> // a role entity
    {
        public Role()
        {

        }

        public Role(string name)
        {
            Name = name;
        }
    }

    public class UserStore : UserStore<Member, Role, int, Member_Login, Member_Role, Member_Claim>
    {
        public UserStore(PortalDbContext context) : base(context)
        {

        }
    }

    public class RoleStore : RoleStore<Role, int, Member_Role>
    {
        public RoleStore(PortalDbContext context) : base(context)
        {

        }
    }
}