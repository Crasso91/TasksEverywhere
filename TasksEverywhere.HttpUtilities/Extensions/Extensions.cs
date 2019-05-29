using TasksEverywhere.HttpUtilities.HttpModels.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TasksEverywhere.Extensions.Extensions;

namespace TasksEverywhere.HttpUtilities.Extensions
{
    public static class Extensions
    {
        public static string Username(this HttpRequestMessage _req)
        {
            // Gets header parameters  
            string authenticationString = _req.Headers.Authorization.Parameter;
            string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));

            // Gets username and password  
            return originalString.Split(':')[0];
        }
        public static string Password(this HttpRequestMessage _req)
        {
            // Gets header parameters  
            string authenticationString = _req.Headers.Authorization.Parameter;
            string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));

            // Gets username and password  
            return originalString.Split(':')[1];
        }

        public static string AppID(this HttpRequestMessage _req)
        {
            var request = _req.Content.ReadAsStringAsync().Result.Deserialize<ApiHttpRequest<dynamic>>();
            return request.AppID;
        }
        public static string AppToken(this HttpRequestMessage _req)
        {
            var request = _req.Content.ReadAsStringAsync().Result.Deserialize<ApiHttpRequest<dynamic>>();
            return request.AppToken;
        }
    }
}
