using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebRequestHelper
{
    public interface IWebClient : IDisposable
    {
        CookieContainer Cookie { get; }

        string Host { get; }

        Encoding ParseEncoding { get; set; }

        T Execute<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse;

        Task<T> ExecuteAsync<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse;

        T ExecuteDownload<T>(Entity.SimpleWebRequest<T> request, string fileSaveFullPath)
            where T : Entity.SimpleWebResponse;

        Task<T> ExecuteDownloadAsync<T>(Entity.SimpleWebRequest<T> request, string fileSaveFullPath)
            where T : Entity.SimpleWebResponse;

        void SetCookie(CookieContainer cookie);

        void SetDomain(string domain);

        string GetCookieValue(Uri uri, string cookieName);
    }
}