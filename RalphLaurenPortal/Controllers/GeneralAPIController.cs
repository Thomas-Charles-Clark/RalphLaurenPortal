using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using RalphLaurenPortal.Models;
using RalphLaurenPortal.Models.ProductSearch;
using RalphLaurenPortal.Models.Resource;
using RalphLaurenPortal.Models.StockInfo;
using RalphLaurenPortal.Models.TechInfo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RalphLaurenPortal.Controllers
{
    [Authorize]
    public class GeneralAPIController : ApiController
    {
        private ApplicationUserManager UserManager { get; set; }
        private PortalDbContext PortalDbContext { get; set; }

        public GeneralAPIController(ApplicationUserManager userManager, PortalDbContext portalDbContext)
        {
            UserManager = userManager;
            PortalDbContext = portalDbContext;
        }

        public GeneralAPIController()
        {

        }

        [HttpPost]
        public IHttpActionResult FetchResources([FromBody]PassResourceFetch objFetch)
        {
            var resourceDefinition = new Resource().loadResourcesFromCms(
                objFetch.countryId,
                objFetch.languageId,
                objFetch.useDate);

            var result = new List<ResourceResult>();

            foreach (var r in objFetch.resources)
            {
                result.Add(new ResourceResult()
                {
                    Name = r,
                    Value = resourceDefinition.Result.Resources
                                                     .Where(p => p.Name == r)
                                                     .Select(p => p.Value)
                                                     .SingleOrDefault()
                });
            }

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult ProductSearch([FromBody]PassProductSearch objSearch)
        {
            var searchResults = PortalDbContext.ProductSearch(objSearch, RequestContext.Principal.Identity.GetUserId<int>());

            return Ok(JObject.Parse(searchResults ?? "{ products: [] }").SelectToken("products").Children());
        }

        [HttpPost]
        public IHttpActionResult FetchTechInfo([FromBody]PassTechInfoFetch objFetch)
        {
            return Ok(PortalDbContext.FetchTechInfo(objFetch, RequestContext.Principal.Identity.GetUserId<int>()));
        }

        [HttpPost]
        public IHttpActionResult GetPieceLevelStockInfo([FromBody]PassStockInfoFetch objFetch)
        {
            //  Has any future date been passed
            var useDate = DateTime.Now;
            //useDate = new DateTime(useDate.Year, useDate.Month, useDate.Day);
            //if (Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["productionVersion"]) == true) { }
            //else
            //{
            //    var queryStrings = Request.GetQueryNameValuePairs();
            //    var match = queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, "dateOverride", true) == 0);
            //    if (string.IsNullOrEmpty(match.Value)) { }
            //    else
            //    {
            //        if (match.Value.Length == "ddMMyyyy".Length)
            //        {
            //            bool dateConvertSuccess = DateTime.TryParseExact(match.Value,
            //                                           6                       "ddMMyyyy",
            //                                                                  CultureInfo.InvariantCulture,
            //                                                                  DateTimeStyles.None,
            //                                                                  out useDate);
            //            if (dateConvertSuccess == true) { }
            //            else
            //            {
            //                useDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //            }
            //        }
            //    }
            //}
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());

            return Ok(PortalDbContext.FetchStockInfo(objFetch, user.TradeAccountID, useDate));
        }
    }
}