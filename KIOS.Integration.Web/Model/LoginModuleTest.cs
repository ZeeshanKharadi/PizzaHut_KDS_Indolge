using Azure.Core;
using Azure;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace KIOS.Integration.Web.Model
{
    public class LoginModuleTest
    {

            public async Task DTIntegration(SalesTransaction salesTransaction, Request request)
            {
                string username = string.Empty;
                string password = string.Empty;
                string Loginurl = string.Empty;
                string Token = string.Empty;
                LoginDT LoginDT = new LoginDT();

                username = await this.GetUsername(salesTransaction.StoreId, request.RequestContext).ConfigureAwait(false);
                password = await this.GetPassword(salesTransaction.StoreId, request.RequestContext).ConfigureAwait(false);
                Loginurl = await this.GetLoginUrl(salesTransaction.StoreId, request.RequestContext).ConfigureAwait(false);

                if (username != "" && password != "" && Loginurl != "")
                {

                    LoginDT.UserName = username;
                    LoginDT.PassWord = password;


                    Token = await this.LoginDTAsync(username, password, Loginurl, LoginDT).ConfigureAwait(false);
                }
                else
                {
                    throw new CommerceException("Microsoft_Dynamics_Commerce_30104", "UserName/Password Not Found For DT API")
                    {

                        LocalizedMessage = "UserName/Password Not Found For DT API"

                    };
                }


            }

            public async Task<string> LoginDTAsync(string username, string password, string url, LoginDT ld)
            {
                string token = string.Empty;
                string Responsejson = string.Empty;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                var content = new StringContent(JsonConvert.SerializeObject(ld), Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                Responsejson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);


                if (JObject.Parse(Responsejson).GetValue("status").ToString() == "ok")
                {
                    token = JObject.Parse(Responsejson).GetValue("token").ToString();
                }
                else
                {
                    throw new CommerceException("Microsoft_Dynamics_Commerce_30104", "Dragon Trail Login Failed")
                    {

                        LocalizedMessage = "Dragon Trail Login Failed"

                    };

                }

                return token;
            }

            //public async Task<string> SalesDTAsync(string Token)
            //{
            //}


            public async Task<string> GetUsername(string storeid, RequestContext context)
            {
                string userid = string.Empty;
                try
                {
                    string qry = "select DTUserId,DTPassword from ax.RETAILSTORETABLE where STORENUMBER  = @STORENUMBER";
                    //"SELECT NAME FROM Ax.RetailStoreTenderTypeTable where NAME = @ERROR"; ;
                    ParameterSet parameters = new ParameterSet();
                    parameters.Add("@STORENUMBER", storeid);

                    DataTable dataTable = new DataTable();
                    DataSet dataSet = await new DatabaseContext(context).ExecuteQueryDataSetAsync(qry, parameters).ConfigureAwait(false);
                    if (dataSet != null)
                        dataTable = dataSet.Tables[0];

                    if (dataTable.Rows.Count > 0)
                        userid = (string)dataTable.Rows[0]["DTUserId"];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return userid;
            }
            public async Task<string> GetPassword(string storeid, RequestContext context)
            {
                string password = string.Empty;
                try
                {
                    string qry = "select DTPassword from ax.RETAILSTORETABLE where STORENUMBER  = @STORENUMBER";
                    //"SELECT NAME FROM Ax.RetailStoreTenderTypeTable where NAME = @ERROR"; ;
                    ParameterSet parameters = new ParameterSet();
                    parameters.Add("@STORENUMBER", storeid);

                    DataTable dataTable = new DataTable();
                    DataSet dataSet = await new DatabaseContext(context).ExecuteQueryDataSetAsync(qry, parameters).ConfigureAwait(false);
                    if (dataSet != null)
                        dataTable = dataSet.Tables[0];

                    if (dataTable.Rows.Count > 0)
                        password = (string)dataTable.Rows[0]["DTPassword"];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return password;
            }
            public async Task<string> GetLoginUrl(string storeid, RequestContext context)
            {
                string password = string.Empty;
                try
                {
                    string qry = "select DTloginUrl from ax.RETAILSTORETABLE where STORENUMBER  = @STORENUMBER";
                    //"SELECT NAME FROM Ax.RetailStoreTenderTypeTable where NAME = @ERROR"; ;
                    ParameterSet parameters = new ParameterSet();
                    parameters.Add("@STORENUMBER", storeid);

                    DataTable dataTable = new DataTable();
                    DataSet dataSet = await new DatabaseContext(context).ExecuteQueryDataSetAsync(qry, parameters).ConfigureAwait(false);
                    if (dataSet != null)
                        dataTable = dataSet.Tables[0];

                    if (dataTable.Rows.Count > 0)
                        password = (string)dataTable.Rows[0]["DTPassword"];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return password;
            }
    }
}
