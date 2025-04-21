using MediatR;
using POS_Integration_CommonCore.Enums;
using POS_IntegrationCommonDTO.Request;
using POS_IntegrationCommonInfrastructure.Model;

namespace KIOS.Integration.Application.Commands
{

    public class CreateRetailTransactionSalesTransCommand : IRequest<RetailTransactionSalesTrans>
    {
        public string? TransactionId { get; set; }
        public string? Store { get; set; }
        public string? ItemId { get; set; }
        public string? ItemName { get; set; }
        public decimal Linenum { get; set; }
        public decimal Quantity { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountInclTax { get; set; }
        public DateTime TransdDate { get; set; }
        public decimal Price { get; set; }
        public decimal NetPrice { get; set; }
        public string LineComment { get; set; }
        public decimal? DiscAmount { get; set; }
        public decimal? DiscAmountWithOutTax { get; set; }
        public decimal? PeriodicDiscAmount { get; set; }
        public decimal? PeriodicPercentaGeDisc { get; set; }
        public string? LastTransactionId { get; set; }
    }
}
