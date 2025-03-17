using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIOS.Integration.Application.Commands
{
	public class FoodDetails
	{
        public string itemid { get; set; }
		public string itemname { get; set; }
		public decimal itemprice { get; set; }
		public decimal itemqty { get; set; }
		public decimal TotalAmount { get; set; }
		public DateTime TransDate { get; set; }
		public DateTime TransTime { get; set; }
		public string OrderStatus { get; set; }


	}
}
