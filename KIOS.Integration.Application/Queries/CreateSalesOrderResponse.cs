using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIOS.Integration.Application.Queries
{
    public class CreateSalesOrderResponse
    {
        public string SalesID { get; set; }
        public string FBRInvoiceNo { get; set; }
        public string RecipteId { get; set; }
    }
}
