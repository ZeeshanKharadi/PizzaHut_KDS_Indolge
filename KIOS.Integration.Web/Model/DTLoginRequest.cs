using Newtonsoft.Json;

namespace KIOS.Integration.Web.Model
{
    public class DTLoginRequest
    {
       
            [JsonProperty("userName")]
            public string UserName { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("userLevel")]
            public int UserLevel { get; set; }

    }

    public class DragonTailCredentials
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string LoginUrl { get; set; }
    }
}
