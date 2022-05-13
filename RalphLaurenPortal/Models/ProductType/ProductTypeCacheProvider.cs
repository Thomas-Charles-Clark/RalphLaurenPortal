using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Configuration;

namespace RalphLaurenPortal.Models.ProductType
{
    public class ProductTypeCacheProvider
    {
        private ObjectCache Cache { get { return MemoryCache.Default; } }

        public ProductTypeDropDown Get(int memberId, int tradeAccountId, DateTime useDate)
        {
            var key = buildKey(memberId, tradeAccountId, useDate);
            return (ProductTypeDropDown)Cache[key];
        }

        public void Set(int memberId, int tradeAccountId, DateTime useDate, ProductTypeDropDown data)
        {
            if (Boolean.TryParse(WebConfigurationManager.AppSettings["productTypeCacheEnabled"], out bool cacheEnabled))
            {
                var policy = new CacheItemPolicy();

                if (Int16.TryParse(WebConfigurationManager.AppSettings["productTypeCacheDuration"], out short cacheDurationMinutes))
                {
                    policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheDurationMinutes);
                }
                else
                {
                    policy.AbsoluteExpiration = DateTime.Today.AddDays(1).Date;
                }

                var key = buildKey(memberId, tradeAccountId, useDate);

                Cache.Add(new CacheItem(key, data), policy);
            }
        }

        public bool IsSet(int memberId, int tradeAccountId, DateTime useDate)
        {
            var key = buildKey(memberId, tradeAccountId, useDate);
            return (Cache[key] != null);
        }

        public void Invalidate(int memberId, int tradeAccountId, DateTime useDate)
        {
            var key = buildKey(memberId, tradeAccountId, useDate);
            Cache.Remove(key);
        }

        private string buildKey(int memberId, int tradeAccountId, DateTime useDate)
        {
            return String.Format($"PRODUCTTYPES~{memberId}~{tradeAccountId}");
        }
    }
}