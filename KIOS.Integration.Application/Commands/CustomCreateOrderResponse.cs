using MediatR;
using POS_Integration_CommonCore.Enums;
using POS_IntegrationCommonDTO.Request;
using POS_IntegrationCommonInfrastructure.Model;

namespace KIOS.Integration.Application.Commands
{
    public class CustomCreateOrderResponse
    {
        public string FBRInvoiceNo { get; set; }
        public string ReceiptId { get; set; }
    }
}
