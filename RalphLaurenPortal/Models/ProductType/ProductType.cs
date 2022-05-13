using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RalphLaurenPortal.Models.ProductType
{
    public class ProductTypeSelect
    {
        public byte ProductTypeID { get; set; }
        public string Name { get; set; }
    }
    
    public class ProductTypeDropDown
    {
        public IEnumerable<ProductTypeSelect> ProductTypes { get; set; }
    }

    public class ProductType
    {
        public async Task<ProductTypeDropDown> FetchProductTypes(int memberId, int tradeAccountId, DateTime useDate)
        {
            // Is page in cache?
            var definition = new ProductTypeDropDown();

            var objProductTypeCache = new ProductTypeCacheProvider();

            definition = objProductTypeCache.Get(memberId, tradeAccountId, useDate);

            if (definition == null)
            {
                definition = await Task.FromResult(loadProductTypesFromDatabase(memberId, tradeAccountId, useDate));
                objProductTypeCache.Set(memberId, tradeAccountId, useDate, definition);
            }

            return definition;
        }

        private ProductTypeDropDown loadProductTypesFromDatabase(int memberId, int tradeAccountId, DateTime useDate)
        {
            return new PortalDbContext().FetchProductTypes(memberId, tradeAccountId);
        }
    }
}