using KIOS.Integration.Application.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using POS_Integration_CommonCore.Helpers;
using POS_Integration_CommonCore.Response;
using POS_IntegrationCommonDTO.Response;
using POS_IntegrationCommonInfrastructure.Database;
using POS_IntegrationCommonInfrastructure.Model;
using System.Data;

namespace KIOS.Integration.Application.Handlers.CommandHandler
{
    public class UpdateKDSOrderHandler : IRequestHandler<UpdateKDSOrderCommand, CreateKDSOrderResponse>
    {
        private readonly AppDbContext _appDbContext;
        private string _connectionString_KBJS;
        private string _itemName;

        public UpdateKDSOrderHandler(IConfiguration configuration, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _connectionString_KBJS = configuration.GetConnectionString("AppDbConnection");
        }


        public async Task<CreateKDSOrderResponse> Handle(UpdateKDSOrderCommand request, CancellationToken cancellationToken)
        {

            ResponseModelWithClass<CreateKDSOrderResponse> response = new ResponseModelWithClass<CreateKDSOrderResponse>();
            CreateKDSOrderResponse responseModel = new CreateKDSOrderResponse();

            IList<PrintSalesOrderTransCC> printSalesOrderTransCC = null;

            try
            {
                printSalesOrderTransCC = await _appDbContext.PrintSalesOrderTransCCs.Where(x=> x.SalesId == request.ThirdPartyOrderId).ToListAsync();

                if (printSalesOrderTransCC != null)
                {
                    foreach (var item in printSalesOrderTransCC)
                    {
                        item.iscancelled = true;
                        item.Comment = request.Reason;
                        item.OrderStatus = 2; // 2 is cancel
                        item.ModifiedOn = DateTime.Now; // 2 is cancel
                        item.ModifiedBy = "Indolge_KDS_Order_Cancel";
                        item.OrderStatus = 2; 

                        _appDbContext.PrintSalesOrderTransCCs.Update(item);
                        await _appDbContext.SaveChangesAsync();
                    }
                   

                }

                InsertPrintSaleOrderCC(request);

                responseModel.ThirdPartyOrderId = request.ThirdPartyOrderId;
                responseModel.OrderStatus = OrderStatus.Confirm;


            }
            catch (Exception ex)
            {
                _appDbContext.SaveChanges();
                DeletePrintSaleOrderCC(request);
                //_appDbContext.PrintSalesOrderTransCCs.Remove(printSalesOrderTransCC);
                throw;
            }

            return responseModel;
        }

        private int InsertPrintSaleOrderCC(UpdateKDSOrderCommand request)
        {
            int affectedRows = 0;
            string storeProcedureName = "usp_update_KDSOrder";

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "ThirdPartyOrderId", request.ThirdPartyOrderId},
                  { "iscancelled", false },
                  { "ModifiedOn", DateTime.Now },
                  { "ModifiedBy", "Indolge_KDS_Order_Cancel" },
                  { "OrderStatus", 2 }
              };

            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KBJS, storeProcedureName, CommandType.StoredProcedure, parameters);

            //}

            return affectedRows;
        }


        private int DeletePrintSaleOrderCC(UpdateKDSOrderCommand request)
        {
            int affectedRows = 0;

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "ThirdPartyOrderId", request.ThirdPartyOrderId }
              };

            string queryHeader = "Delete from ext.PrintSalesOrderCC where SalesId = '" + request.ThirdPartyOrderId + "'";

            try
            {
                affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KBJS, queryHeader, CommandType.Text, parameters);

                string queryLine = "Delete from ext.PrintSalesOrderTransCC where SalesId = '" + request.ThirdPartyOrderId + "'";


                affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KBJS, queryLine, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {

                throw;
            }


            //}

            return affectedRows;
        }
    }
}
