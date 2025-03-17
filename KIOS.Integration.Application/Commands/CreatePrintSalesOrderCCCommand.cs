using MediatR;
using POS_IntegrationCommonInfrastructure.Model;

namespace KIOS.Integration.Application.Commands
{
    public class CreatePrintSalesOrderCCCommand : IRequest<PrintSalesOrderCC>
    {
        #nullable disable
        public string SalesId { get; set; }
        public string CreatedBy { get; set; }
        public string SalesTaker { get; set; }
        public string WarehouseName { get; set; }
        public string customerName { get; set; }
        public decimal TaxTotal { get; set; }
        public string ModeofDelivery { get; set; }
        public decimal NetAmount { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string DeliveryName { get; set; }
        public string Custref { get; set; }
        public decimal AmountExclTax { get; set; }
        public decimal TaxAmount { get; set; }
        public bool Iscancelled { get; set; }
        public IList<CreatePrintSalesOrderTransCCRequest> CreatePrintSalesOrderTransCCCommand { get; set; }

        public CreatePrintSalesOrderCCCommand()
        {
            CreatePrintSalesOrderTransCCCommand = new List<CreatePrintSalesOrderTransCCRequest>();
        }
    }
}
