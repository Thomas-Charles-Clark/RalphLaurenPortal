using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models.TradeAccount
{
    public class TradeAccount
    {
        public int TradeAccountID { get; set; }
        public int? TradeAccountGroupID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string CountryCode { get; set; }
        public string Market { get; set; }
        public short TradePriceListID { get; set; }
        public string Email { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Postcode { get; set; }
        public string HAULCODE { get; set; }
        public string CARRCODE { get; set; }
        public string SampleHAULCODE { get; set; }
        public string SampleCARRCODE { get; set; }
        public string VATCode { get; set; }
        public string VATNumber { get; set; }
        public string SampleLetterLanguageCode { get; set; }
        public string PhoneNumber { get; set; }

        [NotMapped]
        public string Phone
        {
            get
            {
                return PhoneNumber;
            }
        }

        public DateTime? SalesLastUpdatedDT { get; set; }
        public decimal? SalesLY_FullMonth { get; set; }
        public decimal? GrowthLY_FullMonth { get; set; }
        public decimal? SalesTY_FullMonth { get; set; }
        public decimal? GrowthTY_FullMonth { get; set; }
        public decimal? SalesLY_MonthToDate { get; set; }
        public decimal? GrowthLY_MonthToDate { get; set; }
        public decimal? SalesTY_MonthToDate { get; set; }
        public decimal? GrowthTY_MonthToDate { get; set; }
        public decimal? SalesLY_FullFinancialYear { get; set; }
        public decimal? GrowthLY_FullFinancialYear { get; set; }
        public decimal? SalesTY_FullFinancialYear { get; set; }
        public decimal? GrowthTY_FullFinancialYear { get; set; }
        public decimal? SalesLY_FinancialYearToDate { get; set; }
        public decimal? GrowthLY_FinancialYearToDate { get; set; }
        public decimal? SalesTY_FinancialYearToDate { get; set; }
        public decimal? GrowthTY_FinancialYearToDate { get; set; }
        public decimal? SalesLY_FullCalendarYear { get; set; }
        public decimal? GrowthLY_FullCalendarYear { get; set; }
        public decimal? SalesTY_FullCalendarYear { get; set; }
        public decimal? GrowthTY_FullCalendarYear { get; set; }
        public decimal? SalesLY_CalendarYearToDate { get; set; }
        public decimal? GrowthLY_CalendarYearToDate { get; set; }
        public decimal? SalesTY_CalendarYearToDate { get; set; }
        public decimal? GrowthTY_CalendarYearToDate { get; set; }
        public short? SalesRepresentativeID { get; set; }
        public DateTime? SalesCurrLastUpdatedDT { get; set; }
        public decimal? SalesCurrLY_FullMonth { get; set; }
        public decimal? GrowthCurrLY_FullMonth { get; set; }
        public decimal? SalesCurrTY_FullMonth { get; set; }
        public decimal? GrowthCurrTY_FullMonth { get; set; }
        public decimal? SalesCurrLY_MonthToDate { get; set; }
        public decimal? GrowthCurrLY_MonthToDate { get; set; }
        public decimal? SalesCurrTY_MonthToDate { get; set; }
        public decimal? GrowthCurrTY_MonthToDate { get; set; }
        public decimal? SalesCurrLY_FullFinancialYear { get; set; }
        public decimal? GrowthCurrLY_FullFinancialYear { get; set; }
        public decimal? SalesCurrTY_FullFinancialYear { get; set; }
        public decimal? GrowthCurrTY_FullFinancialYear { get; set; }
        public decimal? SalesCurrLY_FinancialYearToDate { get; set; }
        public decimal? GrowthCurrLY_FinancialYearToDate { get; set; }
        public decimal? SalesCurrTY_FinancialYearToDate { get; set; }
        public decimal? GrowthCurrTY_FinancialYearToDate { get; set; }
        public decimal? SalesCurrLY_FullCalendarYear { get; set; }
        public decimal? GrowthCurrLY_FullCalendarYear { get; set; }
        public decimal? SalesCurrTY_FullCalendarYear { get; set; }
        public decimal? GrowthCurrTY_FullCalendarYear { get; set; }
        public decimal? SalesCurrLY_CalendarYearToDate { get; set; }
        public decimal? GrowthCurrLY_CalendarYearToDate { get; set; }
        public decimal? SalesCurrTY_CalendarYearToDate { get; set; }
        public decimal? GrowthCurrTY_CalendarYearToDate { get; set; }
        public byte? CurrencyID { get; set; }
        public int? SalesRepresentativeRank { get; set; }
        public int? SalesRepresentativeAccounts { get; set; }
        public int? MarketRank { get; set; }
        public int? MarketAccounts { get; set; }
        public byte? SalesMarketID { get; set; }
        public int? GlobalRank { get; set; }
        public int? GlobalAccounts { get; set; }
        public string Website { get; set; }
        public decimal? CreditLimit { get; set; }
        public DateTime? OpenedDate { get; set; }
        public short TradeAreaID { get; set; }
        public byte? LanguageID { get; set; }
        public short? TradeAccountTypeID { get; set; }
        public decimal? SalesLast12Months { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public bool AllowTradePrice { get; set; }
        public byte? MarginBoosterPercentage { get; set; }
        public byte? RetailOnlyDiscountPercentage { get; set; }
        public short CountryID { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public bool Open { get; set; }
        public DateTime? AddressUpdatedDT { get; set; }
        public DateTime? CoordinatesUpdatedDT { get; set; }
        public string Memo { get; set; }
        public string SampleOwnHaulierAccNo { get; set; }
        public string DeliveryType { get; set; }
        public string UPSAccountNumber { get; set; }
        public string DHLAccountNumber { get; set; }
        public long? DefaultTradeAccountContactID { get; set; }
        public byte? PaymentTermsID { get; set; }
        public long? DefaultInvoiceTradeAccountAddressID { get; set; }
        public long? DefaultDeliveryTradeAccountAddressID { get; set; }
        public long? DefaultVisitTradeAccountAddressID { get; set; }
        public bool MiscellaneousAccount { get; set; }
        public bool HasEndCustomers { get; set; }
        public bool RetailTill { get; set; }

        public string BeforeOrderNotification { get; set; }

        public string UniVerseDataSources { get; set; }

        public bool RetailCustomer { get; set; }



        [JsonIgnore]
        [InverseProperty("TradeAccount")]
        public virtual ICollection<Member> RalphLaurenMembers { get; set; }








        //[NotMapped]
        //public string Label
        //{
        //    get
        //    {
        //        return $"{Code} - {Name} - (Rep: {(SalesRepresentative?.Name ?? "Unknown Rep")})";
        //    }
        //}

        //[ForeignKey("DefaultInvoiceTradeAccountAddressID")]
        //public virtual TradeAccountAddress DefaultInvoiceAddress { get; set; }

        //[ForeignKey("DefaultDeliveryTradeAccountAddressID")]
        //public virtual TradeAccountAddress DefaultDeliveryAddress { get; set; }

        //[ForeignKey("DefaultVisitTradeAccountAddressID")]
        //public virtual TradeAccountAddress DefaultVisitAddress { get; set; }

        //[ForeignKey("TradeAccountGroupID")]
        //public virtual TradeAccountGroup TradeAccountGroup { get; set; }
        //public virtual TradePriceList TradePriceList { get; set; }
        //public virtual SalesRepresentative SalesRepresentative { get; set; }
        //public virtual Currency Currency { get; set; }
        //public virtual TradeArea TradeArea { get; set; }
        //public virtual Language Language { get; set; }
        //public virtual TradeAccountType TradeAccountType { get; set; }
        //public virtual Country Country { get; set; }
        //public virtual TradeLanguage TradeLanguage { get; set; }
        //public virtual PaymentTerms PaymentTerms { get; set; }

        //[NotMapped]
        //public string CompanyName
        //{
        //    get
        //    {
        //        return Name;
        //    }
        //}

        //[NotMapped]
        //public string CurrencyCulture
        //{
        //    get
        //    {
        //        return Currency?.FormattingCulture;
        //    }
        //}

        //[NotMapped]
        //public string CurrencyCode
        //{
        //    get
        //    {
        //        return Currency?.Code;
        //    }
        //}

        ////[JsonIgnore]
        ////public virtual ICollection<Member> Members { get; set; }

        //[JsonIgnore]
        //public virtual ICollection<TradeAccount_Competitor> Competitors { get; set; }

        //[JsonIgnore]
        //public virtual ICollection<TradeAccountPotential> TradeAccountPotential { get; set; }

        //[JsonIgnore]
        //public virtual ICollection<TradeAccount_TradeAdvertiser> TradeAdvertisers { get; set; }

        //public virtual ICollection<TradeAccountVisit> Visits { get; set; }

        //[JsonIgnore]
        //public virtual ICollection<TradeAccount_Tag> Tags { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<Member_TradeAccount> RelatedMembers { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<ProjectStakeholder> ProjectStakeholdings { get; set; }

        //public virtual ICollection<TradeAccountNote> Notes { get; set; }

        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<TradeAccountContact> Contacts { get; set; }

        //[ForeignKey("DefaultTradeAccountContactID")]
        //public virtual TradeAccountContact DefaultContact { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<SalesRepresentative_TradeAccount> MiscellaneousTradeAccounts { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<Member_Basket> Member_Baskets { get; set; }

        //[NotMapped]
        //public string AdScheme
        //{
        //    get
        //    {
        //        return "Pattern book lite [Dummy Data]";
        //    }
        //}

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<Basket> Baskets { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<TradeDeliveryContractPrice> TradeDeliveryContractPrices { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<TradeAccount_TradeHaulier_Override> TradeHaulierOverrides { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<RingFencePreAllocation> RingFencePreAllocations { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<Return> Returns { get; set; }

        //[JsonIgnore]
        //[InverseProperty("TradeAccount")]
        //public virtual ICollection<Tuffnells.TuffnellsConsignment> TuffnellsConsignments { get; set; }
    }
}