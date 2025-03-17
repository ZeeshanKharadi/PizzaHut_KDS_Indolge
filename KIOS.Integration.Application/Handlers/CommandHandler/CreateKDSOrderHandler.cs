using KIOS.Integration.Application.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using POS_Integration_CommonCore.Helpers;
using POS_Integration_CommonCore.Response;
using POS_IntegrationCommonDTO.Response;
using POS_IntegrationCommonInfrastructure.Database;
using POS_IntegrationCommonInfrastructure.Model;
using System.Data;

namespace KIOS.Integration.Application.Handlers.CommandHandler
{
    public class CreateKDSOrderHandler : IRequestHandler<CreateKDSOrderCommand, CreateKDSOrderResponse>
    {
        private readonly AppDbContext _appDbContext;
        private string _connectionString_KBJS;
        private string _itemName;

        public CreateKDSOrderHandler(IConfiguration configuration, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _connectionString_KBJS = configuration.GetConnectionString("AppDbConnection");
        }

     
        public async Task<CreateKDSOrderResponse> Handle(CreateKDSOrderCommand request, CancellationToken cancellationToken)
        {

            ResponseModelWithClass<CreateKDSOrderResponse> response = new ResponseModelWithClass<CreateKDSOrderResponse>();
            CreateKDSOrderResponse responseModel = new CreateKDSOrderResponse();
            
            PrintSalesOrderTransCC printSalesOrderTransCC = null;


            try
            {
                foreach (var item in request.CreateKDSLineCommand)
                {
                    string itemName = string.Empty;
                    string dataAreaId = "kbjs";

                    string qry = "select ax.ECORESPRODUCTTRANSLATION.DESCRIPTION from ax.INVENTTABLE " +
                        "Join ax.ECORESPRODUCTTRANSLATION On ax.ECORESPRODUCTTRANSLATION.PRODUCT = ax.INVENTTABLE.PRODUCT Where ITEMID ='" + item.ItemId + "' And DATAAREAID = '" + dataAreaId + "' And LANGUAGEID='en-us'";

                    DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KBJS, qry, CommandType.Text);

                    if (dataSet.Tables != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
                    {
                        DataTable dataTable = dataSet.Tables[0];
                        if (dataTable.Rows.Count > 0)
                        {
                            //Hard Code
                            itemName = dataTable.Rows[0]["Description"].ToString();
                            _itemName = itemName;
                            //itemName = "Xinger";
                        }
                    }

                    printSalesOrderTransCC = new PrintSalesOrderTransCC
                    {
                        SalesId = request.ThirdPartyOrderId,
                        ItemId = item.ItemId,
                        ItemName = _itemName,
                        Qty = item.Qty,
                        Comment =  item.Comment,
                        OrderStatus =  1,
                        iscancelled =  false,
                        CreatedBy = "Indolge_KDS_Order",
                        CreatedOn = DateTime.Now
                    };

                    await _appDbContext.PrintSalesOrderTransCCs.AddAsync(printSalesOrderTransCC);
                    await _appDbContext.SaveChangesAsync();
                }

                InsertPrintSaleOrderCC(request);

                //CreateKDSRequestLogs(request);

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

        private int InsertPrintSaleOrderCC(CreateKDSOrderCommand request)
        {
            int affectedRows = 0;
            string storeProcedureName = "usp_insert_KDSOrder";

            Dictionary<string, object> parameters = new Dictionary<string, object>
              {
                  { "SalesId", request.ThirdPartyOrderId },
                  { "StoreId", request.StoreId },
                  { "iscancelled", false },
                  { "CreatedOn", DateTime.Now },
                  { "CreatedBy", "Indolge_KDS_Order" },
                  { "@OrderStatus", 1 }
              };

            affectedRows = SqlHelper.ExecuteNonQuery(_connectionString_KBJS, storeProcedureName, CommandType.StoredProcedure, parameters);

            //}

            return affectedRows;
        }


        private int DeletePrintSaleOrderCC(CreateKDSOrderCommand request)
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
