using Newtonsoft.Json;

namespace KIOS.Integration.Web.Model
{
    public class DTLoginResponse
    {

        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
