using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SimpleWebRequestHelper.Helper;

namespace SimpleWebRequestHelper
{
    /// <summary>
    /// Provide the base implementation of IWebClient, you could do the all web request using the instance of this client
    /// </summary>
    public class DefaultWebClient : IWebClient
    {
        private CookieContainer _cookie = new CookieContainer();
        private string _host = "wx2.qq.com";

        public void Dispose()
        {
            _cookie = null;
        }

        /// <summary>
        /// Get cookie
        /// </summary>
        public CookieContainer Cookie
        {
            get
            {
                return _cookie;
            }
        }

        /// <summary>
        /// Set cookie
        /// </summary>
        /// <param name="cookie"></param>
        public virtual void SetCookie(CookieContainer cookie)
        {
            if (cookie == null)
                this._cookie = new CookieContainer();
            else
                this._cookie = cookie;
        }

        public string Host
        {
            get
            {
                return _host;
            }
        }

        public virtual void SetDomain(string domain)
        {
            this._host = domain;
        }

        public DefaultWebClient() : 
            base()
        {

        }

        public DefaultWebClient(string domain) : 
            this()
        {
            this._host = domain;
        }

        private T Parse<T>(Entity.SimpleWebRequest<T> request, byte[] bytes)
            where T : Entity.SimpleWebResponse
        {
            var responseString = Encoding.UTF8.GetString(bytes);

            if (string.IsNullOrWhiteSpace(responseString))
                return default(T);

            var t = System.Activator.CreateInstance<T>();
            t.ResponseBase64String = Convert.ToBase64String(bytes);

            if (t.ResponseType == Enum.ResponseType.Stream)
            {
                return t;
            }
            else if (t.ResponseType == Enum.ResponseType.JSON)
            {
                var _t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString);
                _t.ResponseBase64String = t.ResponseBase64String;
                return _t;
            }
            else if (t.ResponseType == Enum.ResponseType.XML)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    ms.Write(bytes, 0, bytes.Length);
                    ms.Position = 0;
                    var serializor = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    var obj = serializor.Deserialize(ms) as T;
                    if (obj != null)
                    {
                        obj.ResponseBase64String = Convert.ToBase64String(bytes);
                    }
                    return obj;
                }
            }
            else if (t.ResponseType == Enum.ResponseType.HTML || t.ResponseType == Enum.ResponseType.JavaScript)
            {
                var ps = t.GetType().GetProperties();
                foreach (var p in ps)
                {
                    var at = p.GetCustomAttribute(typeof(Common.RegexAttribute));
                    if (at != null)
                    {
                        var reg = at as Common.RegexAttribute;
                        var regex = new Regex(reg.RegexPattern, reg.RegexOptions);
                        var match = regex.Match(responseString);
                        if (match != null && p.CanWrite)
                        {
                            var propType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                            var _value = match.Groups[reg.Key].Value;

                            if (reg.NeedUrlDecode)
                                _value = _value.UrlDecode();
                            if (reg.NeedHtmlDecode)
                                _value = _value.HtmlDecode();

                            if (propType != typeof(System.String))
                                p.SetValue(t, Convert.ChangeType(_value, propType));
                            else
                                p.SetValue(t, _value);
                        }
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// Submit the specified request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual T Execute<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse
        {
            SetHost(request);

            var bytes = Helper.WebHelper.GetRequestBytes(request, ref _cookie);
            return this.Parse(request, bytes);
        }

        public virtual string ExecuteAsString<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse
        {
            SetHost(request);

            return Helper.WebHelper.GetRequestString(request, ref _cookie);
        }

        public virtual async Task<string> ExecuteAsStringAsync<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse
        {
            SetHost(request);

            return await Helper.WebHelper.GetRequestStringAsync(request, _cookie);
        }

        /// <summary>
        /// Async submit the specified request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual async Task<T> ExecuteAsync<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse
        {
            SetHost(request);

            var bytes = await Helper.WebHelper.GetRequestBytesAsync(request, _cookie);
            return this.Parse(request, bytes);
        }

        /// <summary>
        /// Get cookie value from cookie
        /// </summary>
        /// <returns></returns>
        public virtual string GetCookieValue(Uri uri, string cookieName)
        {
            CookieCollection c = _cookie.GetCookies(uri);
            if (c != null)
            {
                for (int i = 0; i < c.Count; i++)
                {
                    if (c[i].Name == cookieName)
                        return c[i].Value;
                }
            }

            return "";
        }

        /// <summary>
        /// Auto set the api host 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        protected virtual void SetHost<T>(Entity.SimpleWebRequest<T> request)
            where T : Entity.SimpleWebResponse
        {
            var ps = request.GetType().GetProperties();
            foreach (var p in ps)
            {
                if (p.Name == "Host" && p.CanWrite && p.PropertyType == typeof(System.String))
                {
                    p.SetValue(request, _host);
                    break;
                }
            }
        }
    }
}