using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Queries;
using MediatR;
using Microsoft.Extensions.Configuration;
using POS_Integration_CommonCore.Helpers;
using POS_Integration_CommonCore.Response;
using POS_IntegrationCommonInfrastructure.Database;
using POS_IntegrationCommonInfrastructure.Model;
using System.Data;

namespace KIOS.Integration.Application.Handlers.CommandHandler
{
    public class CreatePrintSalesOrderCCHandler : IRequestHandler<CreatePrintSalesOrderCCCommand, PrintSalesOrderCC>
    {
        private readonly AppDbContext _appDbContext;
        private string _connectionString_KFC;

        public CreatePrintSalesOrderCCHandler(IConfiguration configuration, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _connectionString_KFC = configuration.GetConnectionString("AppDbConnection");
        }

     
        public async Task<PrintSalesOrderCC> Handle(CreatePrintSalesOrderCCCommand request, CancellationToken cancellationToken)
        {

            ResponseModelWithClass<CreateSalesOrderResponse> response = new ResponseModelWithClass<CreateSalesOrderResponse>();
            CreateSalesOrderResponse responseModel = new CreateSalesOrderResponse();
            PrintSalesOrderCC printSalesOrderCC = null;
            PrintSalesOrderTransCC printSalesOrderTransCC = null;

            try
            {
                foreach (var item in request.CreatePrintSalesOrderTransCCCommand)
                {
                    printSalesOrderTransCC = new PrintSalesOrderTransCC
                    {
                        SalesId = request.SalesId,
                        ItemId = item.ItemId,
                        ItemName = item.ItemName,
                        Qty = item.Qty,
                        UnitPrice = item.UnitPrice,
                        Comment =  item.Comment,
                        iscancelled =  item.Iscancelled,
                        CreatedBy = request.CreatedBy,
                        CreatedOn = DateTime.Now

                    };
                    await _appDbContext.PrintSalesOrderTransCCs.AddAsync(printSalesOrderTransCC);
                    await _appDbContext.SaveChangesAsync();
                }

                InsertPrintSaleOrderCC(request);

                printSalesOrderCC = new PrintSalesOrderCC
                {
                    SalesId = request.SalesId,
                };

            }
            catch (Exception ex)
            {
                _appDbContext.PrintSalesOrderTransCCs.Remove(printSalesOrderTransCC);
                _appDbContext.SaveChanges();
                DeletePrintSaleOrderCC(request);
                //_appDbContext.PrintSalesOrderTransCCs.Remove(printSalesOrderTransCC);
                throw;
            }

            return printSalesOrderCC;
        }

        private int InsertPrintSaleOrderCC(CreatePrintSalesOrderCCCommand request)
        {
            int affectedRows = 0;
            string storeProcedureName = "usp_insert_PrintSaleOrderCC";

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "SalesId", request.SalesId },
                  { "SalesTaker", request.SalesTaker },
                  { "warehouseName", request.WarehouseName },
                  { "customerName", request.customerName },
                  { "TaxTotal", request.TaxTotal },
                  { "ModeofDelivery", request.ModeofDelivery },
                  { "NetAmount", request.NetAmount },
                  { "CreatedBy", request.CreatedBy },
                  { "CreatedOn", DateTime.Now },
                  { "Address", request.Address },
                  { "Phone", request.Phone },
                  { "DeliveryName", request.DeliveryName },
                  { "Custref", request.Custref },
                  { "AmountExclTax", request.AmountExclTax },
                  { "TaxAmount", request.TaxAmount},
                  { "Iscancelled", request.Iscancelled}
              };

            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, storeProcedureName, CommandType.StoredProcedure, parameters);

            //}

            return affectedRows;
        }


        private int DeletePrintSaleOrderCC(CreatePrintSalesOrderCCCommand request)
        {
            int affectedRows = 0;
            string query = "Delete from ext.PrintSalesOrderCC where SalesId = '" + request.SalesId + "";

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "SalesId", request.SalesId }
              };

            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KFC, query, CommandType.Text, parameters);

            //}

            return affectedRows;
        }
    }
}
