using RalphLaurenPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RalphLaurenPortal.Controllers
{
    public class BaseController : Controller
    {
        protected ApplicationUserManager UserManager;

        protected PortalDbContext PortalDb;

        public BaseController(ApplicationUserManager userManager, PortalDbContext portalDbContext)
        {
            this.UserManager = userManager;
            this.PortalDb = portalDbContext;
        }
    }
}