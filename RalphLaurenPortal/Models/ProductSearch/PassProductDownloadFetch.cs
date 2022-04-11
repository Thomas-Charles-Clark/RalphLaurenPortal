using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models.ProductSearch
{
    public class PassProductDownloadFetch
    {
        public string code { get; set; }
        public string name { get; set; }
        public string searchTerm { get; set; }
        public byte? productTypeId { get; set; }
    }
}