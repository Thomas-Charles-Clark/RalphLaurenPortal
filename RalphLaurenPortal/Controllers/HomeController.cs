using Microsoft.AspNet.Identity;
using RalphLaurenPortal.Models;
using RalphLaurenPortal.Models.TradeAccount;
using RalphLaurenPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RalphLaurenPortal.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(ApplicationUserManager _userManager, PortalDbContext _mainDbContext) : base(_userManager, _mainDbContext)
        {

        }

        public async Task<ActionResult> Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());

            if (Session["TradeAccountID"] == null)
            {
                Session["TradeAccountID"] = user.TradeAccountID;

                var ta = user.AvailableTradeAccounts.SingleOrDefault(p => p.TradeAccountID == user.TradeAccountID);

                if (ta != null)
                {
                    Session["DisplayTradePrice"] = ta.DisplayTradePrice;
                    Session["DisplayRetailPrice"] = ta.DisplayRetailPrice;
                }
            }

            var model = await HomeViewModel.CreateAsync(user, "Home", DateTime.Now);

            return View(model);
        }

        public ActionResult About()
        {
            var model = new BaseViewModel(UserManager.FindById(User.Identity.GetUserId<int>()), "Home", DateTime.Now);

            return View(model);
        }

        public ActionResult Contact()
        {
            var model = new BaseViewModel(UserManager.FindById(User.Identity.GetUserId<int>()), "Home", DateTime.Now);

            return View(model);
        }
    }
}