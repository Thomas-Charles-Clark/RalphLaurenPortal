using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MertexerV2.Models.Accounts;
using RalphLaurenPortal.Models;
using RalphLaurenPortal.Models.TradeAccount;
using RalphLaurenPortal.Models.Collection;
using RalphLaurenPortal.Models.ProductType;

namespace RalphLaurenPortal.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ProductTypeDropDown ProductTypeDropDown { get; set; }

        public CollectionDropDown CollectionDropDown { get; set; }

        public bool PermitOrderStatus
        {
            get
            {
                return User.AllowOrderStatus;
            }
        }

        public bool PermitSalesHistory
        {
            get
            {
                return User.AllowSalesHistory;
            }
        }

        private HomeViewModel(Member member, string title, DateTime? contentDate) : base(member, title, contentDate)
        {

        }

        public static async Task<HomeViewModel> CreateAsync(Member member, string title, DateTime? contentDate)
        {
            var result = new HomeViewModel(member, title, contentDate);

            await result.Initialise();

            return result;
        }

        private async Task Initialise()
        {
            ProductTypeDropDown = await new ProductType().FetchProductTypes(User.MemberID, User.TradeAccountID, ContentDate);

            CollectionDropDown = await new Collection().FetchCollections(User.MemberID, User.TradeAccountID, null, ContentDate);
        }
    }
}