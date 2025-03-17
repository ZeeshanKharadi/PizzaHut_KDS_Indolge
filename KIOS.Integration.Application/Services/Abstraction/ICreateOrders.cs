using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KIOS.Integration.Application.Commands;
using KIOS.Integration.Application.Queries;
using POS_Integration_CommonCore.Response;

namespace KIOS.Integration.Application.Services.Abstraction
{
	public interface ICreateOrders
	{
		Task<ResponseModelWithClass<CreateSalesOrderResponse>> InsertFoods(FoodDetails request);
	}
}
