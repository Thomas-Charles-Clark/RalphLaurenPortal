using RalphLaurenPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RalphLaurenPortal.Controllers
{
    public class HoldingController : Controller
    {
        // GET: Holding
        public ActionResult Index()
        {
            return View(new BaseViewModel(null, "Ralph Lauren", DateTime.Now));
        }
    }
}