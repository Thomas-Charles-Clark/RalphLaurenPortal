using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RalphLaurenPortal.Models.Resource
{
    public class ResourceSelect
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ResourceDefinition
    {
        public List<ResourceSelect> Resources { get; set; }
    }

    public class ResourceResult
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PassResourceFetch
    {
        public string[] resources { get; set; }
        public short countryId { get; set; }
        public byte languageId { get; set; }
        public DateTime useDate { get; set; }
    }

    public class Resource
    {
        public async Task<ResourceDefinition> FetchResources(short countryId, byte languageId, DateTime useDate)
        {
            // Is page in cache?
            var resourceStore = new ResourceDefinition();

            var objResourceCache = new ResourceCacheProvider();

            resourceStore.Resources = objResourceCache.Get(countryId, languageId, useDate);

            if (resourceStore.Resources == null)
            {
                resourceStore = await Task.FromResult(loadResourcesFromDatabase(countryId, languageId, useDate));
                objResourceCache.Set(countryId, languageId, useDate, resourceStore.Resources);
            }

            return resourceStore;
        }

        private ResourceDefinition loadResourcesFromDatabase(short countryId, byte languageId, DateTime useDate)
        {
            return new ResourceDefinition
            {
                Resources = new PortalDbContext().FetchResources(countryId, languageId, useDate).ToList()
            };
        }
    }
}