namespace KIOS.Integration.Application.Commands
{
    public class CreatePrintSalesOrderTransCCRequest
    {
        #nullable disable
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public string Comment { get; set; }
        public bool Iscancelled { get; set; }
    }
}
