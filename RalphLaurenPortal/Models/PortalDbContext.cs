using Dapper;
using Microsoft.AspNet.Identity.EntityFramework;
using RalphLaurenPortal.Models.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models
{
    public partial class PortalDbContext : IdentityDbContext<Member, Role, int, Member_Login, Member_Role, Member_Claim>
    {
        public PortalDbContext() : base("VM-SQL1")
        {
            // for debugging with - this throws every time fetchAlerts is called on the signalR hub...
            // but that can be ignored. Very handy way to get dbContext output, this:
            //this.Database.Log = (s) => System.Diagnostics.Debug.WriteLine(s);
        }

        public PortalDbContext(string connectionName = "VM-SQL1")
            : base(connectionName)
        {
        }

        static PortalDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            System.Data.Entity.Database.SetInitializer<PortalDbContext>(new ApplicationDbInitializer());
        }

        public static PortalDbContext Create()
        {
            return new PortalDbContext();
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var rlSchema = "RalphLauren";

            var member = modelBuilder.Entity<Member>().HasKey(p => p.Id)
                .ToTable("Member", rlSchema);

            member.Ignore(p => p.MemberID);
            member.Ignore(p => p.AccessFailedCount);
            member.Ignore(p => p.PhoneNumberConfirmed);
            member.Ignore(p => p.TwoFactorEnabled);

            member.Property(p => p.Email).HasColumnName("Email");
            member.Property(p => p.EmailConfirmed).HasColumnName("EmailConfirmed");
            member.Property(p => p.UserName).HasColumnName("UserName").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            member.Property(p => p.Deleted).HasColumnName("Deleted").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            member.Property(p => p.Id).HasColumnName("MemberID");
            member.Property(p => p.FullName).HasColumnName("FullName");
            member.Property(p => p.LockoutEnabled).HasColumnName("LockoutEnabled");
            member.Property(p => p.LockoutEndDateUtc).HasColumnName("LockoutEndDT");
            member.Property(p => p.SecurityStamp).HasColumnName("SecurityStamp");
            member.Property(p => p.PasswordHash).HasColumnName("Password");
            member.Property(p => p.PhoneNumber).HasColumnName("PhoneNumber");

            member.HasMany(p => p.Roles)
                .WithRequired(p => p.Member)
                .HasForeignKey(p => p.UserId);

            var member_role = modelBuilder.Entity<Member_Role>().HasKey(p => new { p.UserId, p.RoleId }).ToTable("Member_Role", rlSchema);

            member_role.Property(p => p.UserId).HasColumnName("MemberID");

            member_role.Property(p => p.RoleId).HasColumnName("RoleID");

            member_role.HasRequired(p => p.Member)
                .WithMany(p => p.Roles)
                .HasForeignKey(p => p.UserId);

            member_role.HasRequired(p => p.Role)
                .WithMany(p => p.Users)
                .HasForeignKey(p => p.RoleId);

            var role = modelBuilder.Entity<Role>().ToTable("Role", rlSchema);

            role.Property(p => p.Id).HasColumnName("RoleID");

            var member_login = modelBuilder.Entity<Member_Login>().ToTable("MemberLogin", rlSchema);

            member_login.Property(p => p.UserId).HasColumnName("MemberID");

            var member_claim = modelBuilder.Entity<Member_Claim>().ToTable("MemberClaim", rlSchema);

            member_claim.Property(p => p.UserId).HasColumnName("MemberID");

            member_claim.Property(p => p.Id).HasColumnName("ClaimID");
        }

        internal IEnumerable<ResourceSelect> FetchResources(
            short countryId,
            byte languageId,
            DateTime contentDate)
        {
            return this.Database.Connection.Query<ResourceSelect>("[pub].[Resource_Select]", new
            {
                CountryID = countryId,
                LanguageID = languageId,
                Date = contentDate
            }, null, true, 30, CommandType.StoredProcedure);
        }
    }
}