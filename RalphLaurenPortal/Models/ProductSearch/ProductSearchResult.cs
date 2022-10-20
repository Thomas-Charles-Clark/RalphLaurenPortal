using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models.ProductSearch
{
    public class ProductSearchResult
    {
        public string AKA { get; set; }
        public string code { get; set; }
        public Controls controls { get; set; }
        public decimal? displayPrice { get; set; }
        public decimal? displaySalePrice { get; set; }
        public long imageId { get; set; }
        //public Sample largeSample { get; set; }
        public string name { get; set; }
        public string orderButtonText { get; set; }
        public string priceMessage { get; set; }
        public int productId { get; set; }
        public string rLRRP { get; set; }
        public string rLTradePrice { get; set; }
        public string rRPMessage { get; set; }
        public string reserveButtonText { get; set; }
        public string reserveWithBatchSampleButtonText { get; set; }
        //public Sample sample { get; set; }
        public string size { get; set; }
        public string statusMessage { get; set; }
        public StockMessage stockMessage { get; set; }
        public int templateId { get; set; }
        public string url { get; set; }
        public string urlSemantics { get; set; }
    }

    public class Controls
    {
        public bool allowBackInStockReminder { get; set; }
        public bool allowLargeSample { get; set; }
        public bool allowOrder { get; set; }
        public bool allowReserve { get; set; }
        public bool allowReserveWithBatchSample { get; set; }
        public bool allowSample { get; set; }
        public bool disableOrder { get; set; }
        public bool disableSample { get; set; }
    }

    public class Sample
    {
        public decimal? displayPrice { get; set; }
        public decimal? displaySalePrice { get; set; }
        public string orderButtonText { get; set; }
        public string priceMessage { get; set; }
        public int productId { get; set; }
        public StockMessage stockMessage { get; set; }
    }

    public class StockMessage
    {
        public bool allowBackInStockReminder { get; set; }
        public bool allowStockCheck { get; set; }
        public List<BasketQuantityThresholds> basketQuantityThresholds { get; set; }
        public decimal? freeStock { get; set; }
        public DateTime? nextPODueDate { get; set; }
        public decimal? nextPOFreeStock { get; set; }
        public decimal? pieceBreakdownMaximum { get; set; }
        public decimal? pieceBreakdownMinimum { get; set; }
        public byte? productTypeID { get; set; }
        public int? productTypeQuantityMaximum { get; set; }
        public decimal? quantityIncrement { get; set; }
        public decimal? quantityMaximum { get; set; }
        public decimal? quantityMinimum { get; set; }
        public int? reserveQuantityMaximum { get; set; }
        public decimal? totalPOFreeStock { get; set; }
        public string unit { get; set; }
    }

    public class BasketQuantityThresholds
    {
        public decimal? maximum { get; set; }
        public decimal? minimum { get; set; }
        public List<BasketQuantityThresholdStockMessage> stockMessages { get; set; }
    }

    public class BasketQuantityThresholdStockMessage
    {
        public string @class { get; set; }
        public int stockBreakdownTab { get; set; }
        public string stockMessage { get; set; }
    }
}