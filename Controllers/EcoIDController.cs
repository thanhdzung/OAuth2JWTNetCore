using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OAuth2NetCore.Models;

namespace OAuth2NetCore.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class EcoIDController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public EcoIDController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("[action]")]
        [Route("api/me")]
        [Authorize]
        public async Task<User> WeatherForecasts()
        {
            String token = await HttpContext.GetTokenAsync("access_token");
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://ecoid.comartek.com/auth/realms/ecoid/protocol/openid-connect/userinfo");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            User user = new User();
            if (response.IsSuccessStatusCode)
            {
                user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            }

            return user;
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        [Route("api/register")]
        public async Task<Object> Register([FromBody] Register register)
        {
            String token = await HttpContext.GetTokenAsync("access_token");
            var dict = new Dictionary<string, string>();
            dict.Add("client_id", "web_app");
            dict.Add("client_secret", "80ce504b-bfad-411d-ab5c-a472d2c85975");
            dict.Add("grant_type", "client_credentials");
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://ecoid.comartek.com/auth/realms/ecoid/protocol/openid-connect/token")
            {
                Content = new FormUrlEncodedContent(dict)
            };
            //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            Token tokenVM = new Token();
            if (response.IsSuccessStatusCode)
            {
                tokenVM = JsonConvert.DeserializeObject<Token>(await response.Content.ReadAsStringAsync());
                token = tokenVM.AccessToken;
                StringBuilder builder = new StringBuilder();
                builder.Append("{");
                builder.Append("\"username\": \"" + register.Username + "\",");
                builder.Append("\"enabled\": true,");
                builder.Append("\"emailVerified\": false,");
                builder.Append("\"firstName\": \"" + register.FirstName + "\",");
                builder.Append("\"lastName\": \"" + register.LastName + "\",");
                builder.Append("\"attributes\": {");
                builder.Append("\"phone\": \"" + register.Phone + "\"");
                builder.Append("},");
                builder.Append("\"credentials\": [");
                builder.Append("{");
                builder.Append("\"type\": \"password\",");
                builder.Append("\"value\": \"" + register.Password + "\",");
                builder.Append("\"temporary\": false");
                builder.Append("}");
                builder.Append("]");
                builder.Append("}");
                var buffer = Encoding.UTF8.GetBytes(builder.ToString());
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var request2 = new HttpRequestMessage(HttpMethod.Post,
                "https://ecoid.comartek.com/auth/admin/realms/ecoid/users")
                {
                    Content = byteContent
                };
                request2.Headers.Add("Authorization", $"Bearer {token}");
                var response2 = await client.SendAsync(request2);
                if (response2.IsSuccessStatusCode)
                {
                    return StatusCode(201);
                }
                else
                {
                    this.HttpContext.Response.StatusCode = 500;
                    return response2;
                }
            }
            else
            {
                this.HttpContext.Response.StatusCode = 500;
                return response;
            }
        }
    }
}