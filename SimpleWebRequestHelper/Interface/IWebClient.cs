using System;
using System.Net;
using System.Threading.Tasks;

namespace SimpleWebRequestHelper
{
    public interface IWebClient : IDisposable
    {
        CookieContainer Cookie { get; }

        string Host { get; }

        T Execute<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse;

        string ExecuteAsString<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse;

        Task<T> ExecuteAsync<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse;

        Task<string> ExecuteAsStringAsync<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse;

        void SetCookie(CookieContainer cookie);

        void SetDomain(string domain);

        string GetCookieValue(Uri uri, string cookieName);
    }
}