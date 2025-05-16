using KIOS.Integration.Application.Services.Abstraction;
using KIOS.Integration.Web.Helper;
using KIOS.Integration.Web.Model;
using KIOS.Integration.Web.Services;
using KIOS.Integration.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net.Http;
using System.Text;

namespace KIOS.Integration.Web.Services
{
    public class CreateOrderDTService : ICreateOrderDTService
    {

        private string _connectionString_CHZ_MIDDLEWARE;
        private string _connectionString;
        private string _connectionString_KFC;
        //private InlineQueryResponse lastRecordResponse;
        private string _connectionString_RSSU;
        private readonly IConfiguration _configuration;
        private string _terminalId;
        private string _receiptId;
        private string _suspendedId;
        private string _transactionId;
        private string _fbRInvoiceNo;
        private long _channle = 0;
        private long _batchId = 0;
        private decimal? _totalBillAmount;
        private DataTable dataTable;
        private decimal _taxPrice;
        private decimal _orignalprice;
        private string inventLocationId = string.Empty;
        private decimal _grossAmountCustom;
        private string _staffId = string.Empty;
        private bool _isFBRFail = false;
        private string _isTaxImplemented = string.Empty;
        private string _itemName = string.Empty;
        private string _fbrInvoiceNumber = string.Empty;
        private string _srbInvoiceNumber = string.Empty;
        private decimal _taxValue;
        //private readonly ISender _mediator;
        private string _terminalIdOverride;

        public CreateOrderDTService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString_CHZ_MIDDLEWARE = configuration.GetConnectionString("AppDbConnection");
            _connectionString = configuration.GetConnectionString("AppDbConnection");
            _connectionString_KFC = configuration.GetConnectionString("RSSUConnection");
            _connectionString_RSSU = configuration.GetConnectionString("RSSUConnection");
            _terminalId = _configuration.GetSection("Keys:TerminalId").Value;
            _terminalId = _configuration.GetSection("Keys:TerminalId").Value;
            _isTaxImplemented = _configuration.GetSection("Keys:TaxApplied").Value;
            _terminalIdOverride = _configuration.GetSection("Keys:_terminalIdOverride").Value;
        }

        public async Task<CreateOrderResponse> CreateOrder(CreateOrderModel request)
        {
            CreateOrderResponse response = new CreateOrderResponse();
            if (!IsOrderAlreadyExist(request.ThirdPartyOrderId))
            {
                // Insert into DynamicPosOrder Table
                InsertDynamicPOSOrder(request);
                string apiResult = await SendOrderToDragonTailCreateOrderApi(request);
                response = JsonConvert.DeserializeObject<CreateOrderResponse>(apiResult);
                return response;
            }
            else
            {
                response.Message = "Order Already Exist";
            }
            return response;
        }

        public async Task<CreateOrderResponse> UpdateOrder(UpdateOrderModel request, string ThirdPartyOrderId)
        {

            CreateOrderResponse response = new CreateOrderResponse();
            var json = await GetRequestJsonByThirdPartyOrderId(ThirdPartyOrderId);
            string PreviousOrderStatus;
            if (request.OrderStatus == "01") // Cancelled Kds Order
            {
                PreviousOrderStatus = GetOrderStatus(ThirdPartyOrderId);
                if (PreviousOrderStatus == "0")
                {
                    if (!string.IsNullOrEmpty(json))
                    {
                        string apiResult = await SendCancelKDSOrderToExternalApi(ThirdPartyOrderId);
                        response = JsonConvert.DeserializeObject<CreateOrderResponse>(apiResult);
                        UpdateOrderStatus(ThirdPartyOrderId, request.OrderStatus);
                    }
                }
                else
                {
                    response.Message = "Order Can't cancelled";
                }
            }
            else if (request.OrderStatus == "02") // Sale
            {
                PreviousOrderStatus = GetOrderStatus(ThirdPartyOrderId);
                if (PreviousOrderStatus == "0")
                {
                    if (!string.IsNullOrEmpty(json))
                    {
                        var storedOrder = JsonConvert.DeserializeObject<CreateOrderModel>(json);
                        // Assign ExtItemId to ItemId for each sales line
                        storedOrder.SalesLines.ForEach(line => line.ItemId = line.ExtItemId);
                        string apiResult = await SendSaleOrderToExternalApi(storedOrder);
                        response = JsonConvert.DeserializeObject<CreateOrderResponse>(apiResult);
                        UpdateOrderStatus(ThirdPartyOrderId, request.OrderStatus);
                    }
                }
                else
                {
                    response.Message = "Order Already marked as Sale/ Order can't be marked as sale ";
                }
            }
            else if (request.OrderStatus == "03") // Return
            {
                PreviousOrderStatus = GetOrderStatus(ThirdPartyOrderId);
                if (PreviousOrderStatus == "02")
                {
                    if (!string.IsNullOrEmpty(json))
                    {
                        var storedOrder = JsonConvert.DeserializeObject<CreateOrderModel>(json);
                        // Assign ExtItemId to ItemId for each sales line
                        storedOrder.SalesLines.ForEach(line => line.ItemId = line.ExtItemId);
                        // mapping update Order Tender Type
                        storedOrder.TenderTypeId = request.TenderTypeId;
                        string apiResult = await SendReturnOrderToExternalApi(storedOrder);
                        response = JsonConvert.DeserializeObject<CreateOrderResponse>(apiResult);
                        UpdateOrderStatus(ThirdPartyOrderId, request.OrderStatus);
                    }
                }
                else
                {
                    response.Message = "Order can't returned ";
                }
            }
            else
            {
                response = new CreateOrderResponse();
            }
            return response;
        }

        public async Task<string> GetRequestJsonByThirdPartyOrderId(string thirdPartyOrderId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT RequestJson FROM DynamicPosOrders WHERE ThirdPartyOrderId = @ThirdPartyOrderId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ThirdPartyOrderId", thirdPartyOrderId);

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();
                return result?.ToString();
            }
        }
        public void InsertDynamicPOSOrder(CreateOrderModel order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Serialize SalesLines to JSON
                var salesLinesJson = JsonConvert.SerializeObject(order);

                // Prepare the SQL command for stored procedure
                using (var command = new SqlCommand("InsertDynamicPOSOrder", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@StoreId", order.Store);
                    command.Parameters.AddWithValue("@ThirdPartyOrderId", order.ThirdPartyOrderId);
                    command.Parameters.AddWithValue("@TransDate", order.TransDate);
                    command.Parameters.AddWithValue("@OrderSource", order.OrderSource ?? "");
                    command.Parameters.AddWithValue("@OrderStatus", (int)OrderStatus.Created); // Default status as 'Created'
                    command.Parameters.AddWithValue("@RequestJson", salesLinesJson);

                    // Execute the command
                    command.ExecuteNonQuery();
                }
            }
        }

        public enum OrderStatus
        {
            Created = 0,
            Cancelled = 1,
            Sale = 2,
            Return = 3
        }



        public async Task<string> SendOrderToDragonTailCreateOrderApi(CreateOrderModel request)
        {
            var dtModel = MapToDTModel(request);
            DragonTailCredentials DTCredentials =  await GetDragonTailCredentialsAsync(request.Store);
            string token = await GetDragonTailTokenAsync(DTCredentials.LoginUrl, DTCredentials.UserId, DTCredentials.Password);
            var headers = new Dictionary<string, string>
            {
                { "token", token }
            };
            string apiUrl = "http://localhost:1638/api/OrderKDS/CreateKDSOrder";
            return await ApiHelper.PostAsync(apiUrl, dtModel);

        }
        public async Task<string> SendSaleOrderToExternalApi(CreateOrderModel request)
        {
            request.Company = "php";
            request.BusinessDateCustom = request.TransDate;
            string apiUrl = "http://localhost:1638/api/OrderPOS/complete-order";
            return await ApiHelper.PostAsync(apiUrl, request);
        }

        public async Task<string> SendReturnOrderToExternalApi(CreateOrderModel request)
        {
            request.Company = "php";
            request.BusinessDateCustom = request.TransDate;
            string apiUrl = "http://localhost:1638/api/OrderPOS/return-order";
            return await ApiHelper.PostAsync(apiUrl, request);
        }

        public async Task<string> SendCancelKDSOrderToExternalApi(string ThirdPartyOrderId)
        {

            string apiUrl = "http://localhost:1638/api/OrderKDS/cancelKDSOrder/?OrderId=" + ThirdPartyOrderId;
            return await ApiHelper.DeleteAsync(apiUrl);
        }

        public bool IsOrderAlreadyExist(string thirdPartyOrderId)
        {
            string query = "SELECT COUNT(*) FROM dbo.DynamicPOSOrders WHERE thirdPartyOrderId = @thirdPartyOrderId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add(new SqlParameter("@thirdPartyOrderId", SqlDbType.VarChar) { Value = thirdPartyOrderId });

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public string GetOrderStatus(string thirdPartyOrderId)
        {
            string query = "SELECT orderstatus FROM dbo.DynamicPOSOrders WHERE thirdPartyOrderId = @thirdPartyOrderId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add(new SqlParameter("@thirdPartyOrderId", SqlDbType.VarChar) { Value = thirdPartyOrderId });

                connection.Open();
                var result = command.ExecuteScalar();
                return result?.ToString(); // Returns null if no record found
            }
        }
        public void UpdateOrderStatus(string thirdPartyOrderId, string orderStatus)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("UpdateDynamicPOSOrder", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@thirdPartyOrderId", SqlDbType.VarChar) { Value = thirdPartyOrderId });
                command.Parameters.Add(new SqlParameter("@orderStatus", SqlDbType.VarChar) { Value = orderStatus });

                connection.Open();
                command.ExecuteNonQuery(); // Use ExecuteNonQuery for UPDATE
            }
        }
        public async Task<string> GetDragonTailTokenAsync(string loginUrl, string username, string password)
        {
            var loginRequest = new DTLoginRequest
            {
                UserName = username,
                Password = password,
                UserLevel = -1
            };

            string response = await ApiHelper.PostAsync(loginUrl, loginRequest);

            var loginResponse = JsonConvert.DeserializeObject<DTLoginResponse>(response);

            if (loginResponse.Status.ToLower() != "ok")
                throw new Exception("Login failed: " + response);

            return loginResponse.Token;
        }

        public async Task<DragonTailCredentials> GetDragonTailCredentialsAsync(string storeId)
        {
           
            const string query = @"
        SELECT DTUserId, DTPassword, DTloginUrl 
        FROM ax.RETAILSTORETABLE 
        WHERE STORENUMBER = @STORENUMBER";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.Add(new SqlParameter("@STORENUMBER", SqlDbType.VarChar) { Value = storeId });

                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new DragonTailCredentials
                        {
                            UserId = reader["DTUserId"]?.ToString(),
                            Password = reader["DTPassword"]?.ToString(),
                            LoginUrl = reader["DTloginUrl"]?.ToString()
                        };
                    }
                    else
                    {
                        throw new Exception("No credentials found for the given store ID.");
                    }
                }
            }
        }


        public DTCreateOrderModel MapToDTModel(CreateOrderModel request)
        {
            int generatedOrderId = new Random().Next(50000, 99999); // Use a proper ID strategy here

            return new DTCreateOrderModel
            {
                time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                storeNo = int.Parse(request.Store),
                fullLoad = false,
                AltOrderId = request.ThirdPartyOrderId,
                orders = new List<DTOrder>
        {
            new DTOrder
            {
                orderId = generatedOrderId,
                storeNo = int.Parse(request.Store),
                clientId = request.clientId ?? 0,
                lastName = request.lastName ?? "",
                firstName = request.firstName,
                city = request.city,
                street = request.street ?? "",
                addressNo = request.addressNo,
                postCode = request.postCode ?? "",
                secondaryAddress = request.secondaryAddress ?? "",
                lat = request.lat,
                lng = request.lng,
                phone = request.phone ?? "",
                orderTotal = request.NetAmount,
                paymentMethod = request.TenderTypeId ?? "1",
                cash = request.AmountCur,
                orderTime = request.orderTime,
                saleType = request.Type,
                dailyNo = 1, // or generate dynamically
                priority = 1,
                carrierInstructions = request.carrierInstructions ?? "",
                vipId = Guid.NewGuid().ToString()
            }
        },
                orderItems = request.SalesLines.Select(line => new DTOrderItem
                {
                    orderId = generatedOrderId,
                    storeNo = int.Parse(request.Store),
                    kdsList = "Pizza-KDS", // or dynamic
                    CutTableDineIn_KDS = null,
                    CutTableDineIn_Printer = null,
                    position = line.position,
                    itemNo = line.ExtItemId,
                    quantity = line.Qty,
                    description = line.LineComment ?? line.ExtItemId,
                    side = line.side,
                    rightSideIcon = "default.jpg", // set dynamically if needed
                    style = line.side == 0 ? "color: green;" : null
                }).ToList()
            };
        }

    }
}
