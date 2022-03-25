using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models.StockInfo
{
    public class PassStockInfoFetch
    {
        public int ProductID { get; set; }
        public short CountryID { get; set; }
        public byte LanguageID { get; set; }
        public DateTime? ContentDate { get; set; }
    }
}