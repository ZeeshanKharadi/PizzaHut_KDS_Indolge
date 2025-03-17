using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Queries;
using KIOS.Integration.Application.Services.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using POS_Integration_CommonCore.Enums;
using POS_Integration_CommonCore.Response;

namespace KIOS.Integration.Application.Services
{

	public class InsertIntoFoods : ICreateOrders
	{
		
		private readonly IConfiguration iconfig;
		public  string FoodDBCS;
		public InsertIntoFoods(IConfiguration _iconfig)
		{
			iconfig = _iconfig;
		}


		public async Task<ResponseModelWithClass<CreateSalesOrderResponse>> InsertFoods(FoodDetails request)
		{
			FoodDBCS = iconfig.GetConnectionString("FoodDB");
			ResponseModelWithClass<CreateSalesOrderResponse> response = new ResponseModelWithClass<CreateSalesOrderResponse>(); //await this._ifooddetails.CreateFoodDetails(request);
			try
			{
				using (SqlConnection con = new SqlConnection(FoodDBCS))
				{
					// check itemid already exist
					string InsertIntoFoodDetails = @"insert into fooddetails(itemid,itemname,itemprice,itemqty,TotalAmount,TransDate,TransTime,OrderStatus)
                                                 VALUES(@itemid,@itemname,@itemprice,@itemqty,@totalamount,@transdate,@transtime,@orderstatus)";

					using (SqlCommand cmd = new SqlCommand(InsertIntoFoodDetails, con))
					{
						con.Open();

						cmd.Parameters.AddWithValue("@itemid", request.itemid);
						cmd.Parameters.AddWithValue("@itemname", request.itemname);
						cmd.Parameters.AddWithValue("@itemprice", request.itemprice);
						cmd.Parameters.AddWithValue("@itemqty", request.itemqty);
						cmd.Parameters.AddWithValue("@totalamount", request.TotalAmount);
						cmd.Parameters.AddWithValue("@TransDate", request.TransDate);
						cmd.Parameters.AddWithValue("@transtime", request.TransTime);
						cmd.Parameters.AddWithValue("@orderstatus", request.OrderStatus);
						int i = cmd.ExecuteNonQuery();
						con.Close();
						if (i > 0)
						{
							response.HttpStatusCode = (int)HttpStatusCode.OK;
							response.MessageType = (int)MessageType.Success;
							response.Message = "Data Inserted Successfully";
						}
						else
						{
							response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
							response.MessageType = (int)MessageType.Error;
							response.Message = "No Data Inserted";
						}
					}
				}
			}
			catch (Exception ex)
			{
				response.HttpStatusCode = (int)HttpStatusCode.BadRequest;
				response.MessageType = (int)MessageType.Error;
				response.Message = "No Data Inserted";
			}
			return response;

		}
	}
}
