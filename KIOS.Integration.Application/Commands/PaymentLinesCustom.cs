using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHZ.Integration.Application.Commands
{
    public class PaymentLinesCustom
    {
        public string TenderTypeId { get; set; }  // Represents the type of tender used for the transaction
        public string AmountCur { get; set; }      // Represents the current amount (currency-specific)
        public string AmountMst { get; set; }      // Represents the amount in master currency
        public string AmountTendered { get; set; } // Represents the amount that was tendered
        public string Currency { get; set; }       // Represents the currency used in the transaction
        public string GrossAmount { get; set; }    // Represents the gross amount before any deductions or taxes


    }
}
