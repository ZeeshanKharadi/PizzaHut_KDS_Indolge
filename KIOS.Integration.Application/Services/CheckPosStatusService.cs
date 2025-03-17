using KIOS.Integration.Application.Services.Abstraction;
using MediatR;
using Microsoft.Extensions.Configuration;
using POS_Integration_CommonCore.Enums;
using POS_Integration_CommonCore.Helpers;
using POS_Integration_CommonCore.Response;
using POS_IntegrationCommonDTO.Response;
using System.Data;
using System.Net;

namespace KIOS.Integration.Application.Services
{
    public class CheckPosStatusService : ICheckPosStatusService
    {
        private string _connectionString_KFC;
        private readonly IConfiguration _configuration;
        private string _terminalId;


        public CheckPosStatusService(IConfiguration configuration, ISender mediator)
        {
            _configuration = configuration;
            _connectionString_KFC = configuration.GetConnectionString("RSSUConnection");
            _terminalId = _configuration.GetSection("Keys:TerminalId").Value;
        }

        public async Task<ResponseModelWithClass<CheckPOSStatusReposne>> CheckPosStatusAsync(string storeId)
        {
            ResponseModelWithClass<CheckPOSStatusReposne> response = new ResponseModelWithClass<CheckPOSStatusReposne>();
            CheckPOSStatusReposne responseModel = new CheckPOSStatusReposne();

            string posStatus = string.Empty;
            string shiftId = string.Empty;
            response.HttpStatusCode = (int)HttpStatusCode.Accepted;
            response.MessageType = (int)MessageType.Info;

            if (storeId != null || storeId != "" || storeId != string.Empty || storeId != "string" )
            {
                string query = "select Top 1 SHIFTID from crt.RETAILSHIFTSTAGINGVIEW Where CURRENTTERMINALID ='" + _terminalId + "' AND STOREID = '" + storeId + "'  AND STATUS = 1";

                DataSet dataSet = SqlHelper.ExecuteDataSet(_connectionString_KFC, query, CommandType.Text);

                try
                {
                    if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
                    {
                        DataTable dataTable = dataSet.Tables[0];

                        if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
                        {

                            shiftId = dataTable.Rows[0]["SHIFTID"].ToString();
                        }

                        if (shiftId != null && shiftId != "" && shiftId != string.Empty)
                        {
                            posStatus = "TerminalID: " + _terminalId + " is on";

                            responseModel.IsPOSOn = true;
                            responseModel.Message = posStatus;
                            response.Result = responseModel;
                            response.Message = "Success";
                            response.HttpStatusCode = (int)HttpStatusCode.OK;
                            response.MessageType = (int)MessageType.Success;
                            return response;
                        }
                        else
                        {
                            posStatus = "TerminalID: " + _terminalId + " not found!";
                            responseModel.IsPOSOn = false;
                            responseModel.Message = posStatus;
                            response.Result = null;
                            response.Message = "ServiceUnavailable (Failed)";
                            response.HttpStatusCode = (int)HttpStatusCode.ServiceUnavailable;
                            response.MessageType = (int)MessageType.Info;
                            return response;
                        }
                    }
                }
                catch (Exception ex)
                {
                    posStatus = "TerminalID: " + _terminalId + " not found!";
                    responseModel.IsPOSOn = false;
                    responseModel.Message = null;
                    response.Result = null;
                    response.Message = ex.Message;
                    response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                    response.MessageType = (int)MessageType.Error;
                    return response;
                    throw;
                }
                
            }
            
            return response;
        }
    }
}
