using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RalphLaurenPortal.Models
{
	public class ClientErrorDTO
	{
		public string name { get; set; }
		public string message { get; set; }
		public string stack { get; set; }
		public string lineNo { get; set; }
		public string colNo { get; set; }
		public string error { get; set; }
		public string url { get; set; }
	}
}