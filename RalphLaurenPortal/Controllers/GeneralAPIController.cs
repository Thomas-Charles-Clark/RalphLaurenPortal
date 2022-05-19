using MertexerV2.Accounts;
using MertexerV2.Models.Accounts;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using RalphLaurenPortal.Models;
using RalphLaurenPortal.Models.ProductSearch;
using RalphLaurenPortal.Models.ProductType;
using RalphLaurenPortal.Models.Resource;
using RalphLaurenPortal.Models.SalesHistory;
using RalphLaurenPortal.Models.StockInfo;
using RalphLaurenPortal.Models.TechInfo;
using RalphLaurenPortal.Models.TradeAccount;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
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
        public IHttpActionResult KeepSessionAlive()
        {
            HttpContext.Current.Session["KeepSessionAlive"] = DateTime.Now.ToString();

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult UpdateTradeAccountID([FromBody]PassTradeAccountID objTradeAccountId)
        {
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());

            var tradeAccount = user.AvailableTradeAccounts.SingleOrDefault(a => a.TradeAccountID == objTradeAccountId.tradeAccountId);

            if (tradeAccount != null)
            {
                HttpContext.Current.Session["TradeAccountID"] = objTradeAccountId.tradeAccountId;
                HttpContext.Current.Session["DisplayTradePrice"] = tradeAccount.DisplayTradePrice;
                HttpContext.Current.Session["DisplayRetailPrice"] = tradeAccount.DisplayRetailPrice;
                return Ok(new { success = true });
            }

            return Ok(new { success = false });
        }

        [HttpPost]
        public IHttpActionResult FetchResources([FromBody]PassResourceFetch objFetch)
        {
            if (objFetch.useDate == DateTime.MinValue)
                objFetch.useDate = DateTime.Now;

            var resourceDefinition = new Resource().FetchResources(
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
        public IHttpActionResult FetchCollections([FromBody]PassProductTypeID objProductTypeId)
        {
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());
            
            var results = PortalDbContext.FetchCollections(
                user.MemberID,
                user.TradeAccountID,
                objProductTypeId.productTypeId);

            return Ok(results);
        }

        [HttpPost]
        public IHttpActionResult ProductSearch([FromBody]PassProductSearch objSearch)
        {
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());

            var tradeAccountId = user.TradeAccountID;

            if (Int32.TryParse((string)HttpContext.Current.Session["TradeAccountID"]?.ToString(), out int tid))
            {
                tradeAccountId = tid;
            }

            var searchResults = PortalDbContext.ProductSearch(objSearch, user.MemberID, tradeAccountId);

            var deserialized = JObject.Parse(searchResults ?? "{ products: [] }")
                .SelectToken("products")
                .Children()
                .Select(p => p.SelectToken("product").ToObject<ProductSearchResult>())
                .ToList();

            if(Convert.ToBoolean(HttpContext.Current.Session["DisplayTradePrice"]) == false)
            {
                deserialized.ForEach(sr =>
                {
                    sr.rLTradePrice = null;
                });
            }

            if (Convert.ToBoolean(HttpContext.Current.Session["DisplayRetailPrice"]) == false)
            {
                deserialized.ForEach(sr =>
                {
                    sr.rLRRP = null;
                });
            }

            return Ok(deserialized);
        }

        [HttpPost]
        public IHttpActionResult FetchTechInfo([FromBody]PassTechInfoFetch objFetch)
        {
            var result = PortalDbContext.FetchTechInfo(objFetch, RequestContext.Principal.Identity.GetUserId<int>());

            return Ok(JArray.Parse(result));
        }

        [HttpPost]
        public IHttpActionResult FetchStockInfo([FromBody]PassStockInfoFetch objFetch)
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

            return Ok(PortalDbContext.FetchStockInfo(objFetch, user.TradeAccountID, user.DotNetCulture, useDate));
        }

        [HttpPost]
        public IHttpActionResult OutstandingOrderSearch([FromBody]PassOutstandingOrdersSearch objSearch)
        {
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());

            var tradeAccountId = user.TradeAccountID;

            if (Int32.TryParse((string)HttpContext.Current.Session["TradeAccountID"]?.ToString(), out int tid))
            {
                tradeAccountId = tid;
            }

            var uniVerseDataSources = user.AvailableTradeAccounts.Single(p => p.TradeAccountID == tradeAccountId).TradeAccount.UniVerseDataSources;

            var results = AccountFunctions.OutstandingOrders(uniVerseDataSources, objSearch);

            return Ok(results.Results);
        }

        [HttpPost]
        public IHttpActionResult GetParcelCount([FromBody]PassParcelCount objParcel)
        {
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());

            var tradeAccountId = user.TradeAccountID;

            if (Int32.TryParse((string)HttpContext.Current.Session["TradeAccountID"]?.ToString(), out int tid))
            {
                tradeAccountId = tid;
            }

            var uniVerseDataSources = user.AvailableTradeAccounts.Single(p => p.TradeAccountID == tradeAccountId).TradeAccount.UniVerseDataSources;

            var results = AccountFunctions.CheckParcelCount(uniVerseDataSources, objParcel);

            return Ok(results);
        }

        [HttpPost]
        public IHttpActionResult GetOrderDeliveryInfo([FromBody] PassOrderNumber objOrderNumber)
        {
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());

            var tradeAccountId = user.TradeAccountID;

            if (Int32.TryParse((string)HttpContext.Current.Session["TradeAccountID"]?.ToString(), out int tid))
            {
                tradeAccountId = tid;
            }

            var uniVerseDataSources = user.AvailableTradeAccounts.Single(p => p.TradeAccountID == tradeAccountId).TradeAccount.UniVerseDataSources;

            var results = AccountFunctions.OrderDeliveryInfo(uniVerseDataSources, objOrderNumber);

            results.OrderLines.ForEach(ol => {
                if (DateTime.TryParse(ol.DespatchStatus, out DateTime despatchStatus))
                {
                    ol.DespatchStatus = despatchStatus.ToString("MM/dd/yyyy");
                }
            });

            return Ok(results);
        }

        [HttpPost]
        public IHttpActionResult FetchProductDownload([FromBody]PassProductDownloadFetch objFetch)
        {
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());

            var tradeAccountId = user.TradeAccountID;

            if (Int32.TryParse((string)HttpContext.Current.Session["TradeAccountID"]?.ToString(), out int tid))
            {
                tradeAccountId = tid;
            }

            var bytes = PortalDbContext.ProductFeedStream(objFetch, user.MemberID, tradeAccountId).ToArray();

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(bytes);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            //var result = new HttpResponseMessage(HttpStatusCode.OK)
            //{
            //    Content = new StreamContent(PortalDbContext.ProductFeedStream(objFetch, user.MemberID))
            //};

#if DEBUG
            System.IO.File.WriteAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/RL-Testing.xlsx"), bytes);
#endif

            return Ok(new { fileContents = Convert.ToBase64String(bytes) });
        }

        [HttpPost]
        public IHttpActionResult SalesHistorySearch([FromBody]PassSalesHistorySelect objSelect)
        {
            var user = UserManager.FindById<Member, int>(RequestContext.Principal.Identity.GetUserId<int>());

            var tradeAccountId = (int?)user.TradeAccountID;

            if (Int32.TryParse((string)HttpContext.Current.Session["TradeAccountID"]?.ToString(), out int tid))
            {
                tradeAccountId = tid;
            }

            if (objSelect.allTradeAccounts)
            {
                tradeAccountId = null;
            }

            var results = PortalDbContext.FetchSalesHistory(user.MemberID, tradeAccountId, objSelect);

            return Ok(results);
        }
    }
}