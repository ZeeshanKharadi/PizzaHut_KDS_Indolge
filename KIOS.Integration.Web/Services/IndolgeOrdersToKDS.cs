using KIOS.Integration.Web.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net;

namespace KIOS.Integration.Web.Services
{
    public class IndolgeOrdersToKDS
    {
        private readonly IConfiguration Iconfig;
        private string cs;
        private string _connectionString_RSSU;

        public IndolgeOrdersToKDS(IConfiguration iconfig)
        {
            Iconfig = iconfig;
            _connectionString_RSSU = iconfig.GetConnectionString("RSSUConnection");

        }

        public async Task<ResponseModelWithClass> InsertIndolgeOrdersToKDS(OrderHeader _request)
        {
            ResponseModelWithClass res = new ResponseModelWithClass();

            try
            {
                cs = Iconfig.GetConnectionString("CSKDS");

                // check third pary order id exist
                bool checkRowHeader = this.IsCheckThordtPartyOrderId(cs, _request.thirdPartyOrderId);
                if (checkRowHeader == true) // if ThirdParty OrderId Exist == true
                {
                    res.Message = "Third Party OrderId Already Exist";
                    res.MessageType = 400;
                    res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                }
                else // Third Party Order Id Not Found Else Data Inserted
                {

                    int GetInsertLineRow = this.InsertOrderToKDS(cs, _request);

                    if (GetInsertLineRow == 1)
                    {
                        res.Message = "Data Inserted Successfully";
                        res.MessageType = (int)200;
                        res.HttpStatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        res.Message = "Data Not Inserted";
                        res.MessageType = (int)400;
                        res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = "Data Not Inserted";
                res.MessageType = (int)500;
                res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
            }
            return res;
        }

        private bool IsCheckThordtPartyOrderId(string _connectionString, string OrderId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                bool IsCheckOrderId = false;
                string Get_OrderId = "select * from Orders where OrderId = '" + OrderId + "'";
                SqlDataAdapter _sqldataadapter = new SqlDataAdapter(Get_OrderId, con);
                _sqldataadapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                DataTable datatabe = new DataTable();
                _sqldataadapter.Fill(datatabe);
                if (datatabe.Rows.Count > 0)
                {
                    IsCheckOrderId = true;
                }
                else
                {
                    IsCheckOrderId = false;
                }
                return IsCheckOrderId;
            }
        }
        private int InsertOrderToKDS(string _connectionString, OrderHeader orders)
        {
            DateTime CreatedOn = DateTime.Now;
            string OrderStatus = "Preparation";
            string OrderState = "Preparing";
            string OrderStatusId = "1";
            string OrderSource = "KDS Orders";
            


            //string OrderTypeId = "03";
            string OrderType = orders.orderType ?? "DELIVERY";
            string OrderTypeId = GetOrderTypeIdFromOrderType(_connectionString, OrderType);
            int LineNum = 0;
            string transactionId = "0";
            int transactiontype = 2;
            List<Orders> salesLines = orders.salesLines.ToList();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                int affectedRow = 0;
                foreach (var row in salesLines)
                {
                    string stationName = GetStationNameFromItemId(_connectionString_RSSU, row.itemId);
                    string stationId = "";

                    if (stationName == "Pizza")
                    {
                        stationId = "1";
                    }
                    else if (stationName == "Appetizer")
                    {
                        stationId = "2";
                    }
                    LineNum++;
                    string InsertIntoOrders =
                            @"Insert Into Orders (OrderId,
                                OrderNo,
                                CreatedOn,
                                ItemId,
                                ItemName,
                                Quantity,
                                StoreID,
                                OrderStatus,
                                OrderStatusId,
                                OrderState,
                                PosId,
                                OrderTypeId,
                                OrderType,
                                LineNum,
                                OrderSource,
                                TransactionID,
                                ItemCategory,
                                TransactionType,
                                StationId,
                                StationName,
                                Size)
                                VALUES
                                (@OrderId,
                                @OrderNo,
                                @CreatedOn,
                                @ItemId,
                                @ItemName,
                                @Qty,
                                @StoreID,
                                @OrderStatus,
                                @OrderStatusId,
                                @OrderState,
                                @PosId,
                                @OrderTypeId,
                                @OrderType,
                                @LineNum,
                                @OrderSource,
                                @TransactionId,
                                @ItemCategory,
                                @TransactionType,
                                @StationId,
                                @StationName,
                                @Size)";
                    SqlCommand InsertIntoLineCmd = new SqlCommand(InsertIntoOrders, con);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderId", orders.thirdPartyOrderId);
                    InsertIntoLineCmd.Parameters.AddWithValue("OrderNo", orders.thirdPartyOrderId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@ItemId", row.itemId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@ItemName", row.itemName);
                    InsertIntoLineCmd.Parameters.AddWithValue("@CreatedOn", CreatedOn);
                    InsertIntoLineCmd.Parameters.AddWithValue("@StoreId", orders.storeId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderStatus", OrderStatus);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderStatusId", OrderStatusId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@Qty", row.quantity);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderState", OrderState);
                    InsertIntoLineCmd.Parameters.AddWithValue("@PosId", row.posId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderTypeId", OrderTypeId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderType", OrderType);
                    InsertIntoLineCmd.Parameters.AddWithValue("@LineNum", LineNum);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderSource", OrderSource);
                    InsertIntoLineCmd.Parameters.AddWithValue("@TransactionId", transactionId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@ItemCategory", "");
                    InsertIntoLineCmd.Parameters.AddWithValue("@TransactionType", transactiontype);
                    InsertIntoLineCmd.Parameters.AddWithValue("@StationId", row.stationId?? "1");
                    //InsertIntoLineCmd.Parameters.AddWithValue("@StationId", row.stationId?? stationId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@StationName", row.stationName?? "Pizza");
                    //InsertIntoLineCmd.Parameters.AddWithValue("@StationName", row.stationName?? stationName);
                    InsertIntoLineCmd.Parameters.AddWithValue("@Size", row.size?? "");

                    con.Open();
                    affectedRow = InsertIntoLineCmd.ExecuteNonQuery();
                    con.Close();

                    DeductQtyFromBoh(row.itemId, row.quantity, _connectionString);
                }
                return affectedRow;
            }
        }

        public async Task<ResponseModelWithClass> UpdateIndolgeOrdersToKDS(string OrderId, List<Orders> Orders)
        {
            ResponseModelWithClass res = new ResponseModelWithClass();

            try
            {
                cs = Iconfig.GetConnectionString("CSKDS");
                bool IsOrderExist = this.CheckOrderIdIfExist(cs, OrderId);
                if (IsOrderExist == true)
                {
                    // check third pary order id exist
                    string OrderStatusId = this.CheckOrderStatus(cs, OrderId);

                    if (OrderStatusId == "1")  // Third Party Order Id Not Found Else Data Inserted
                    {
                        // delete Order
                        this.DeleteFromKDS(cs, OrderId);
                        int GetInsertLineRow = this.InsertUpdatedOrder(cs, Orders, OrderId);

                        if (GetInsertLineRow == 1)
                        {
                            res.Message = "Data Inserted Successfully";
                            res.MessageType = (int)200;
                            res.HttpStatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            res.Message = "Data Not Inserted";
                            res.MessageType = (int)400;
                            res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                        }
                    }
                    else
                    {
                        res.Message = "Order Status Not Equal To MOH";
                        res.MessageType = 400;
                        res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    res.Message = "Order Id Not Exist : " + OrderId;
                    res.MessageType = 400;
                    res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                res.Message = "Data Not Inserted";
                res.MessageType = (int)500;
                res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
            }
            return res;
        }

        private string CheckOrderStatus(string _connectionString, string OrderId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string OrderStatusId = "";
                DataRow dr;
                string Get_OrderId = "select OrderStatusId from Orders where OrderId = '" + OrderId + "'";
                SqlDataAdapter _sqldataadapter = new SqlDataAdapter(Get_OrderId, con);
                _sqldataadapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                DataTable datatabe = new DataTable();
                _sqldataadapter.Fill(datatabe);
                if (datatabe.Rows.Count > 0)
                {
                    dr = datatabe.Rows[0];
                    OrderStatusId = dr["OrderStatusId"].ToString();
                }
                return OrderStatusId;
            }
        }
        private int DeleteFromKDS(string _connectionString, string OrderId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string OrderStatusId = "";
                string Get_OrderId = "delete from Orders where OrderId = '" + OrderId + "'";
                SqlCommand sqlcmd = new SqlCommand(Get_OrderId, con);
                con.Open();
                int DelRow = sqlcmd.ExecuteNonQuery();
                con.Close();
                if (DelRow > 0)
                {
                    return DelRow;
                }
                else
                {
                    return DelRow;
                }

            }
        }
        private int InsertUpdatedOrder(string _connectionString, List<Orders> orders, string OrderId)
        {
            DateTime CreatedOn = DateTime.Now;
            string OrderStatus = "Preparation";
            string OrderState = "Preparing";
            string OrderStatusId = "1";
            string OrderSource = "KDS Orders";
            //string OrderTypeId = "03";
            //string OrderType = orders.orderType ?? "DELIVERY";
            string OrderType = GetOrderTypeFromOrderId(_connectionString, OrderId);
            string OrderTypeId = GetOrderTypeIdFromOrderType(_connectionString, OrderType);
            int LineNum = 0;
            string transactionId = "0";
            int transactiontype = 2;
            List<Orders> salesLines = orders.ToList();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                int affectedRow = 0;
                foreach (var row in salesLines)
                {
                    LineNum++;
                    string InsertIntoOrders =
                            @"Insert Into Orders (OrderId,
                                OrderNo,
                                CreatedOn,
                                ItemId,
                                ItemName,
                                Quantity,
                                OrderStatus,
                                OrderStatusId,
                                OrderState,
                                PosId,
                                OrderTypeId,
                                OrderType,
                                LineNum,
                                OrderSource,
                                TransactionID,
                                ItemCategory,
                                TransactionType,
                                StationId,
                                StationName,
                                Size)
                                VALUES
                                (@OrderId,
                                @OrderNo,
                                @CreatedOn,
                                @ItemId,
                                @ItemName,
                                @Qty,
                                @OrderStatus,
                                @OrderStatusId,
                                @OrderState,
                                @PosId,
                                @OrderTypeId,
                                @OrderType,
                                @LineNum,
                                @OrderSource,
                                @TransactionId,
                                @ItemCategory,
                                @TransactionType
                                @StationId,
                                @StationName,
                                @Size)";
                    SqlCommand InsertIntoLineCmd = new SqlCommand(InsertIntoOrders, con);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderId", OrderId);
                    InsertIntoLineCmd.Parameters.AddWithValue("OrderNo", OrderId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@ItemId", row.itemId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@ItemName", row.itemName);
                    InsertIntoLineCmd.Parameters.AddWithValue("@CreatedOn", CreatedOn);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderStatus", OrderStatus);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderStatusId", OrderStatusId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@Qty", row.quantity);

                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderState", OrderState);
                    InsertIntoLineCmd.Parameters.AddWithValue("@PosId", row.posId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderTypeId", OrderTypeId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderType", OrderType);
                    InsertIntoLineCmd.Parameters.AddWithValue("@LineNum", LineNum);
                    InsertIntoLineCmd.Parameters.AddWithValue("@OrderSource", OrderSource);
                    InsertIntoLineCmd.Parameters.AddWithValue("@TransactionId", transactionId);
                    InsertIntoLineCmd.Parameters.AddWithValue("@ItemCategory", "");
                    InsertIntoLineCmd.Parameters.AddWithValue("@TransactionType", transactiontype); 
                    InsertIntoLineCmd.Parameters.AddWithValue("@StationId", row.stationId ?? "1");
                    InsertIntoLineCmd.Parameters.AddWithValue("@StationName", row.stationName ?? "");
                    InsertIntoLineCmd.Parameters.AddWithValue("@Size", row.stationName ?? "");

                    con.Open();

                    affectedRow = InsertIntoLineCmd.ExecuteNonQuery();

                    con.Close();
                }
                return affectedRow;
            }
        }
        private bool CheckOrderIdIfExist(string _connectionString, string OrderId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                bool IsCheckOrderId = false;
                string Get_OrderId = "select * from Orders where OrderId = '" + OrderId + "'";
                SqlDataAdapter _sqldataadapter = new SqlDataAdapter(Get_OrderId, con);
                _sqldataadapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                DataTable datatabe = new DataTable();
                _sqldataadapter.Fill(datatabe);
                if (datatabe.Rows.Count > 0)
                {
                    IsCheckOrderId = true;
                }
                else
                {
                    IsCheckOrderId = false;
                }
                return IsCheckOrderId;
            }
        }

        public async Task<ResponseModelWithClass> DeleteOrderFromKDS(string cs, string orderid)
        {
            ResponseModelWithClass res = new ResponseModelWithClass();

            try
            {
                bool isOrderIdYes = IsCheckThordtPartyOrderId(cs, orderid);
                if (isOrderIdYes == true)
                {
                    string OrderStatusId = CheckOrderStatus(cs, orderid);
                    if (OrderStatusId == "1")
                    {
                        int DeleteRow = DeleteFromKDS(cs, orderid);
                        if (DeleteRow > 0)
                        {
                            res.Message = "Data Deleted Successfully";
                            res.MessageType = (int)200;
                            res.HttpStatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            res.Message = "Order Id Not Exist : " + orderid;
                            res.MessageType = (int)400;
                            res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                        }
                    }
                    else
                    {
                        res.Message = "Order Status Not Equal To MOH";
                        res.MessageType = 400;
                        res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    res.Message = "Order Id Not Exist : " + orderid;
                    res.MessageType = 400;
                    res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            catch (Exception)
            {
                res.Message = "Data Not Deleted";
                res.MessageType = 400;
                res.HttpStatusCode = (int)HttpStatusCode.BadRequest;
            }
            return res;
        }

        public void DeductQtyFromBoh(string ItemId, decimal Qty, string cs)
        {
            decimal Onhand = 0;
            decimal RemainingQty = 0;
            try
            {
                SqlConnection con = new SqlConnection(cs);
                string query = "select * from BOM where ItemId = '" + ItemId + "'";
                SqlDataAdapter _sda = new SqlDataAdapter(query, con);
                DataTable _dt = new DataTable();
                _sda.Fill(_dt);
                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow rows in _dt.Rows)
                    {
                        string GetOnhandFromItem = @"select * from item where ItemId = 
                                                    '" + rows["FryingItem"].ToString() + "' and IsFried = 1";
                        SqlDataAdapter OnHandItemAdapt = new SqlDataAdapter(GetOnhandFromItem, con);

                        DataTable _onhanddt = new DataTable();
                        OnHandItemAdapt.Fill(_onhanddt);
                        if (_onhanddt.Rows.Count > 0)
                        {
                            DataRow GetOnhandRow = _onhanddt.Rows[0];
                            Onhand = Convert.ToDecimal(GetOnhandRow["OnHandQuantity"].ToString());
                            RemainingQty = (Onhand - Qty);

                            string UpdateItemOnhand = "update item set OnHandQuantity = '" + RemainingQty +
                                                       "' where ItemId='" + rows["FryingItem"].ToString() + "'";
                            SqlCommand UpdCon = new SqlCommand(UpdateItemOnhand, con);

                            con.Open();
                            UpdCon.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string GetOrderTypeIdFromOrderType(string _connectionString, string OrderType)
        {
            // repetitive data :/
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string OrderTypeId = "";
                DataRow dr;
                string Get_OrderId = "select OrderTypeId from OrderTypes where Ordertype = '" + OrderType + "'";
                SqlDataAdapter _sqldataadapter = new SqlDataAdapter(Get_OrderId, con);
                _sqldataadapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                DataTable datatabe = new DataTable();
                _sqldataadapter.Fill(datatabe);
                if (datatabe.Rows.Count > 0)
                {
                    dr = datatabe.Rows[0];
                    OrderTypeId = dr["OrderTypeId"].ToString();
                }
                return OrderTypeId;
            }
        }
        private string GetOrderTypeFromOrderId(string _connectionString, string OrderId)
        {
            // repetitive data :/
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string OrderType = "";
                DataRow dr;
                string Get_OrderType = "select top 1 OrderType from Orders where Orderid = '" + OrderId + "'";
                SqlDataAdapter _sqldataadapter = new SqlDataAdapter(Get_OrderType, con);
                _sqldataadapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                DataTable datatabe = new DataTable();
                _sqldataadapter.Fill(datatabe);
                if (datatabe.Rows.Count > 0)
                {
                    dr = datatabe.Rows[0];
                    OrderType = dr["OrderType"].ToString();
                }
                return OrderType;
            }
        }
        private string GetStationNameFromItemId(string _connectionString, string itemId)
        {
            // repetitive data :/
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string StationName = "";
                DataRow dr;
                string Get_OrderType = "SELECT ITEMID,CASE WHEN PizzaStation = 1 THEN 'Pizza' WHEN PastaStation = 1 THEN 'Pasta Station' ELSE 'Unknown' END AS AssignedStation FROM ax.retailinventtable WHERE ITEMID = '" + itemId + "'";
                SqlDataAdapter _sqldataadapter = new SqlDataAdapter(Get_OrderType, con);
                _sqldataadapter.SelectCommand.CommandType = System.Data.CommandType.Text;
                DataTable datatabe = new DataTable();
                _sqldataadapter.Fill(datatabe);
                if (datatabe.Rows.Count > 0)
                {
                    dr = datatabe.Rows[0];
                    StationName = dr["AssignedStation"].ToString();
                }
                return StationName;
            }
        }



    }
}
