using Dapper;
using Microsoft.AspNet.Identity.EntityFramework;
using RalphLaurenPortal.Models.ProductSearch;
using RalphLaurenPortal.Models.Resource;
using RalphLaurenPortal.Models.StockInfo;
using RalphLaurenPortal.Models.TechInfo;
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

        internal string ProductSearch(PassProductSearch objSearch, int memberId)
        {
            return this.Database.Connection.ExecuteScalar<string>("[RalphLauren].[Product_Search]", new
            {
                SearchTerm = objSearch.SearchTerm,
                ProductTypeID = objSearch.ProductTypeID,
                MemberID = memberId 
            }, null, 30, CommandType.StoredProcedure);
        }

        internal string FetchTechInfo(
            PassTechInfoFetch objFetch,
            int memberId)
        {
            return this.Database.Connection.ExecuteScalar<string>("[RalphLauren].[Product_TechInfo_Select]", new
            {
                ProductID = objFetch.ProductID,
                MemberID = memberId
            }, null, 30, CommandType.StoredProcedure);
        }


        public ProductStockInfoReturn FetchStockInfo(PassStockInfoFetch objFetch, int? tradeAccountId, DateTime? useDate)
        {
            var result = new ProductStockInfoReturn();

            var results = this.Database.Connection.QueryMultiple("[pub].[Stock_Select]", new {
                TradeAccountID = tradeAccountId,
                ProductID = objFetch.ProductID,
                CountryID = objFetch.CountryID,
                LanguageID = objFetch.LanguageID,
                Date = useDate
            }, null, 30, CommandType.StoredProcedure);

            var preresult = results.Read<ProductStockFetchResult>();

            // pub.Stock_Message piggybacks on portal.Stock_Message which actually outputs what we're after but doesn't
            // bother translating it - and won't ever bother translating it. Skip this result set:
            _ = results.Read();

            // This final result set can be translated. It isn't, at the time of writing, but at least it can be.
            _ = results.Read();

            result.SupplierAvailabilityMessage = results.ReadSingle<string>();

            if (preresult.Any())
            {
                result.Code = preresult.First().ProductCode;
                result.TotalStock = preresult.Select(p => p.AvailableNowQuantity).Sum()?.ToString("N2");
                result.TotalPurchaseOrderLength = preresult.Select(p => p.DueInQuantity).Sum().ToString("N2");

                var batchGrouping = preresult.Where(p => p.StockLocationTypeID == 1 && p.AvailableNowQuantity > 0).GroupBy(p => p.Batch).ToList();

                var supplierBatchGrouping = preresult.Where(p => p.StockLocationTypeID == 2 && p.AvailableNowQuantity > 0).GroupBy(p => p.Batch).ToList();

                result.Batches = batchGrouping.Select(p => new Batch()
                {
                    BatchNumber = p.Key,
                    ItemCount = p.Count().ToString("N2"),
                    Length = p.Select(q => q.AvailableNowQuantity).Sum()?.ToString("N2"),
                    Pieces = p.Select(q => new BatchPiece() { Length = q.AvailableNowQuantity?.ToString("N2"), Quantity = "1" }).ToList()
                }).ToList();

                result.SupplierStock = supplierBatchGrouping.Select(p => new Batch()
                {
                    BatchNumber = p.Key,
                    ItemCount = p.Count().ToString("N2"),
                    Length = p.Select(q => q.AvailableNowQuantity).Sum()?.ToString("N2"),
                    Pieces = p.Select(q => new BatchPiece() { Length = q.AvailableNowQuantity?.ToString("N2"), Quantity = "1" }).ToList()
                }).ToList();

                result.PurchaseOrders = preresult.Where(p => p.DueInQuantity > 0).OrderBy(p => p.DueInDate).Select(p => new PurchaseOrder()
                {
                    PurchaseOrderNumber = null,
                    DueDate = p.DueInDate?.ToString("dd/MM/yyyy"),
                    Length = p.DueInQuantity.ToString("N2"),
                    POConfirmed = p.POConfirmed
                }).ToList();
            }

            return result;
        }
    }
}