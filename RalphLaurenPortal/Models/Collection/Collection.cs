using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RalphLaurenPortal.Models.Collection
{
    public class CollectionSelect
    {
        public short CollectionID { get; set; }
        public string Name { get; set; }
    }
    
    public class CollectionDropDown
    {
        public IEnumerable<CollectionSelect> Collections { get; set; }
    }

    public class Collection
    {
        public async Task<CollectionDropDown> FetchCollections(int memberId, int tradeAccountId, byte? productTypeId, DateTime useDate)
        {
            // Is page in cache?
            var definition = new CollectionDropDown();

            var objCollectionCache = new CollectionCacheProvider();

            definition = objCollectionCache.Get(memberId, tradeAccountId, productTypeId, useDate);

            if (definition == null)
            {
                definition = await Task.FromResult(loadCollectionsFromDatabase(memberId, tradeAccountId, productTypeId));
                objCollectionCache.Set(memberId, tradeAccountId, productTypeId, useDate, definition);
            }

            return definition;
        }

        private CollectionDropDown loadCollectionsFromDatabase(int memberId, int tradeAccountId, byte? productTypeId)
        {
            return new PortalDbContext().FetchCollections(memberId, tradeAccountId, productTypeId);
        }
    }
}