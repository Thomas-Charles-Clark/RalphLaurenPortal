using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models.StockInfo
{
    public class ProductStockInfoReturn
    {
        public ProductStockInfoReturn() { }

        public string Code { get; set; }
        public string TotalStock { get; set; }
        public List<Batch> Batches { get; set; }
        public List<Batch> SupplierStock { get; set; }
        public List<PurchaseOrder> PurchaseOrders { get; set; }
        public string TotalPurchaseOrderLength { get; set; }

        public string SupplierAvailabilityMessage { get; set; }
    }

    public class Batch
    {
        public Batch() { }

        public string BatchNumber { get; set; }
        public string ItemCount { get; set; }
        public string Length { get; set; }
        public List<BatchPiece> Pieces { get; set; }
    }

    public class BatchPiece
    {
        public BatchPiece() { }

        public string Quantity { get; set; }
        public string Length { get; set; }
    }

    public class PurchaseOrder
    {
        public PurchaseOrder() { }

        public string PurchaseOrderNumber { get; set; }
        public string DueDate { get; set; }
        public string Length { get; set; }
        public bool? POConfirmed { get; set; }
    }

    public class BasketItemAllocationStockFetchResult
    {
        public short StockLocationID { get; set; }
        public byte StockLocationTypeID { get; set; }
        public string StockLocationCode { get; set; }
        public string StockLocationName { get; set; }
        public long StockLocationSortOrder { get; set; }
        public string Batch { get; set; }
        public string Piece { get; set; }
        public long SortOrder { get; set; }
        public decimal? AvailableNowQuantity { get; set; }
        public decimal? WaitingToBePutAwayQuantity { get; set; }
        public decimal? BulkQuantity { get; set; }
        public decimal? ThisCustomerRingFencedQuantity { get; set; }
        public decimal? OtherCustomerRingFencedQuantity { get; set; }
        public decimal OnReserveQuantity { get; set; }
        public decimal DueInQuantity { get; set; }
        public DateTime? DueInDate { get; set; }
        public string DueInMessage { get; set; }
    }

    public class ProductStockFetchResult : BasketItemAllocationStockFetchResult
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public bool? POConfirmed { get; set; }
    }
}