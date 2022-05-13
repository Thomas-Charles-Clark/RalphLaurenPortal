using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models.SalesHistory
{
    public class SalesHistorySelect
    {
        public int ProductID { get; set; }
        public string Code { get; set; }
        public long ImageID { get; set; }
        public string Name { get; set; }
        public string ProductTypeName { get; set; }
        public string CollectionName { get; set; }
        public string Status { get; set; }
        public string StockMessage { get; set; }
        public decimal SoldQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal WIPQuantity { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Unit { get; set; }


        public int productId 
        {
            get 
            {
                return ProductID;    
            }
            private set 
            {
                
            }
        }

        public long imageId
        {
            get
            {
                return ImageID;
            }
            private set
            {

            }
        }

        public string code
        {
            get
            {
                return Code;
            }
            private set
            {

            }
        }

        public object stockMessage
        {
            get
            {
                return new { unit = Unit };
            }
            private set
            {

            }
        }

        public string statusMessage
        {
            get
            {
                return Status;
            }
            private set
            {

            }
        }

        public string name
        {
            get
            {
                return Name;
            }
            private set
            {

            }
        }
    }
}