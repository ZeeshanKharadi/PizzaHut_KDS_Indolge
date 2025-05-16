using Newtonsoft.Json;
using System.Text;

namespace KIOS.Integration.Web.Helper
{
    public static class ApiHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<string> PostAsync(string url, object requestBody, Dictionary<string, string>? headers = null)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = content;

                // Add custom headers if provided
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                HttpResponseMessage response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


        public static async Task<string> DeleteWithBodyAsync(string url, object requestBody)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = content
                };

                Console.WriteLine("Serialized Body:");
                Console.WriteLine(jsonContent);
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static async Task<string> PutAsync(string url, object requestBody)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(url, content);

                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public static async Task<string> DeleteAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync(url);

                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }


    }
}
