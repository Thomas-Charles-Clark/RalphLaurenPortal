using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models.SalesHistory
{
    public class PassSalesHistorySelect
    {
        public short dateRange { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public bool allTradeAccounts { get; set; }
    }
}