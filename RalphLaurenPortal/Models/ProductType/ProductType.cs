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
        public async Task<ProductTypeDropDown> FetchProductTypes(short countryId, byte languageId, DateTime useDate)
        {
            // Is page in cache?
            var definition = new ProductTypeDropDown();

            var objProductTypeCache = new ProductTypeCacheProvider();

            definition = objProductTypeCache.Get(countryId, languageId, useDate);

            if (definition == null)
            {
                definition = await Task.FromResult(loadProductTypesFromDatabase(countryId, languageId, useDate));
                objProductTypeCache.Set(countryId, languageId, useDate, definition);
            }

            return definition;
        }

        private ProductTypeDropDown loadProductTypesFromDatabase(short countryId, byte languageId, DateTime useDate)
        {
            return new PortalDbContext().FetchProductTypes(countryId, languageId, useDate);
        }
    }
}