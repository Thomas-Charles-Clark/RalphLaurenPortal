using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Configuration;

namespace RalphLaurenPortal.Models.Resource
{
    public class ResourceCacheProvider
    {
        private ObjectCache Cache { get { return MemoryCache.Default; } }

        public List<ResourceSelect> Get(short countryId, byte languageId, DateTime useDate)
        {
            var key = buildKey(countryId, languageId, useDate);
            return (List<ResourceSelect>)Cache[key];
        }

        public void Set(short countryId, byte languageId, DateTime useDate, List<ResourceSelect> data)
        {
            if (Boolean.TryParse(WebConfigurationManager.AppSettings["resourceCacheEnabled"], out bool cacheEnabled))
            {
                var policy = new CacheItemPolicy();

                if (Int16.TryParse(WebConfigurationManager.AppSettings["resourceCacheDuration"], out short cacheDurationMinutes))
                {
                    policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheDurationMinutes);
                }
                else
                {
                    policy.AbsoluteExpiration = DateTime.Today.AddDays(1).Date;
                }

                var key = buildKey(countryId, languageId, useDate);

                Cache.Add(new CacheItem(key, data), policy);
            }
        }

        public bool IsSet(short countryId, byte languageId, DateTime useDate)
        {
            var key = buildKey(countryId, languageId, useDate);
            return (Cache[key] != null);
        }

        public void Invalidate(short countryId, byte languageId, DateTime useDate)
        {
            var key = buildKey(countryId, languageId, useDate);
            Cache.Remove(key);
        }


        private string buildKey(short countryId, byte languageId, DateTime useDate)
        {
            return String.Format($"RESOURCES~{countryId}~{languageId}");
        }
    }
}