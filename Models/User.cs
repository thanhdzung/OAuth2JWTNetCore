using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth2NetCore.Models
{
    public class User
    {
        [JsonProperty("sub")]
        public string UserId { get; set; }
        [JsonProperty("name")]
        public String Name { get; set; }
        [JsonProperty("preferred_username")]
        public string UserName { get; set; }
        [JsonProperty("given_name")]
        public string GivenName { get; set; }
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("email_verified")]
        public bool EmailVerify { get; set; }
    }
}
