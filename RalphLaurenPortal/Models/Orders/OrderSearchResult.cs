using MertexerV2.Models.Accounts;
using RalphLaurenPortal.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models.Orders
{
    public class OrderSearchResult : OutstandingOrderResult
    {
        public string OrderDateDisplay { get; set; }

        public string DespatchStatusDateDisplay { get; set; }

        public string ReservedUntilDisplay { get; set; }

        public OrderSearchResult() : base("MERTEX")
        {

        }

        public void ApplyCulture(string culture)
        {
            var ci = new System.Globalization.CultureInfo(culture);

            if (OrderDate.HasValue)
            {
                OrderDateDisplay = Framework.GetShortDateString(ci, OrderDate.Value);
            }

            if (DespatchStatusDate.HasValue)
            {
                DespatchStatusDateDisplay = Framework.GetShortDateString(ci, OrderDate.Value);
            }
            
            if (DateTime.TryParse(DespatchStatusDateMessage, out DateTime sd))
            {
                DespatchStatusDateDisplay = Framework.GetShortDateString(ci, sd);
            }

            if (ReservedUntil.HasValue)
            {
                ReservedUntilDisplay = Framework.GetShortDateString(ci, OrderDate.Value);
            }
        }
    }
}