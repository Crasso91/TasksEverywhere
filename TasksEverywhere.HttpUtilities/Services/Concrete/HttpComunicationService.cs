using TasksEverywhere.Extensions;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.CastleWindsor.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace TasksEverywhere.HttpUtilities.Service.Concrete
{
    public class HttpComunicationService : IHttpComunicationService
    {

        public Task<HttpResponseMessage> GetAsync(string url, string username, string password, bool isSecureConnection)
        {
            /*LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Url = " + url.ToString() + ", usnerma = " + username.ToString() + ", password = " + password.ToString() + ", isSecure = " + isSecureConnection.ToString());*/
            Task<HttpResponseMessage> _result = null;
            if (isSecureConnection)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            /*
                        var handler = new HttpClientHandler
                        {
                            Credentials = new NetworkCredential(username, password),
                            UseDefaultCredentials = true,
                        };
                        */
            var credentials = string.Format("{0}:{1}", username, password);
            var credentialsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(120);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentialsBase64);
                _result = httpClient.GetAsync(url);
                _result.Wait();
            }
            return _result;
        }

        public Task<HttpResponseMessage> PostAsync(string url, string username, string password, bool isSecureConnection, object content)
        {
            /*LogsAppendersManager.Instance.Debug(this.GetType(), MethodBase.GetCurrentMethod(), "Url = " + url.ToString() + ", username = " + username.ToString() + ", password = " + password.ToString() + ", isSecure = " + isSecureConnection.ToString());*/
            Task<HttpResponseMessage> _result = null;
            if (isSecureConnection)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var body = new StringContent(content.Stringify(), Encoding.UTF8, "application/json");
/*
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password),
                UseDefaultCredentials = true,
            };
            */
            var credentials = string.Format("{0}:{1}", username, password);
            var credentialsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(120);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentialsBase64);
                _result = httpClient.PostAsync(url, body);
                _result.Wait();
            }
            return _result;
        }

        public T Get<T>(string url, string username, string password, bool isSecureConnection) where T : new()
        {
            T _result = new T();
            var response = GetAsync(url, username, password, isSecureConnection).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            return content.Deserialize<T>();
        }

        public T Post<T>(string url, string username, string password, bool isSecureConnection, object content) where T : new()
        {
            T _result = new T();
            var response = PostAsync(url, username, password, isSecureConnection, content).Result;
            var respContent = response.Content.ReadAsStringAsync().Result;
            return respContent.Deserialize<T>();
        }
    }
}
