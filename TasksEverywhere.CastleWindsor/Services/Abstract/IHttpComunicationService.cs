using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.CastleWindsor.Service.Abstract
{
    public interface IHttpComunicationService
    {
        Task<HttpResponseMessage> GetAsync(string url, string username, string password, bool isSecureConnection);
        Task<HttpResponseMessage> PostAsync(string url, string username, string password, bool isSecureConnection, object content);
        T Get<T>(string url, string username, string password, bool isSecureConnection) where T : new();
        T Post<T>(string url, string username, string password, bool isSecureConnection, object content) where T : new();
    }
}
