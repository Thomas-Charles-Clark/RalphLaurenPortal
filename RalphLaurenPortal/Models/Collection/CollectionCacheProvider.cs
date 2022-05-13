using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Configuration;

namespace RalphLaurenPortal.Models.Collection
{
    public class CollectionCacheProvider
    {
        private ObjectCache Cache { get { return MemoryCache.Default; } }

        public CollectionDropDown Get(int memberId, int tradeAccountId, byte? productTypeId, DateTime useDate)
        {
            var key = buildKey(memberId, tradeAccountId, productTypeId, useDate);
            return (CollectionDropDown)Cache[key];
        }

        public void Set(int memberId, int tradeAccountId, byte? productTypeId, DateTime useDate, CollectionDropDown data)
        {
            if (Boolean.TryParse(WebConfigurationManager.AppSettings["collectionCacheEnabled"], out bool cacheEnabled))
            {
                var policy = new CacheItemPolicy();

                if (Int16.TryParse(WebConfigurationManager.AppSettings["collectionCacheDuration"], out short cacheDurationMinutes))
                {
                    policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheDurationMinutes);
                }
                else
                {
                    policy.AbsoluteExpiration = DateTime.Today.AddDays(1).Date;
                }

                var key = buildKey(memberId, tradeAccountId, productTypeId, useDate);

                Cache.Add(new CacheItem(key, data), policy);
            }
        }

        public bool IsSet(int memberId, int tradeAccountId, byte? productTypeId, DateTime useDate)
        {
            var key = buildKey(memberId, tradeAccountId, productTypeId, useDate);
            return (Cache[key] != null);
        }

        public void Invalidate(int memberId, int tradeAccountId, byte? productTypeId, DateTime useDate)
        {
            var key = buildKey(memberId, tradeAccountId, productTypeId, useDate);
            Cache.Remove(key);
        }

        private string buildKey(int memberId, int tradeAccountId, byte? productTypeId, DateTime useDate)
        {
            return String.Format($"COLLECTIONS~{memberId}~{tradeAccountId}~{productTypeId}~{useDate:g}");
        }
    }
}