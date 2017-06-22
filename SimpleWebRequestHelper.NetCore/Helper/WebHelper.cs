using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWebRequestHelper.Helper
{
    internal class WebHelper
    {
        #region private methods    

        private static HttpWebRequest CreateRequest<T>(Entity.SimpleWebRequest<T> _request, ref CookieContainer cookie)
            where T : Entity.SimpleWebResponse
        {
            var api = _request.ApiUrl;
            if (_request.HttpMethod == Enum.HttpMethods.GET && !string.IsNullOrWhiteSpace(_request.QueryString))
            {
                var uri = new Uri(api);
                if (!string.IsNullOrWhiteSpace(uri.Query))
                    api = api + ("&" + _request.QueryString);
                else
                    api = api + "?" + _request.QueryString;
            }

            var request = WebRequest.Create(api) as HttpWebRequest;
            if (_request.Headers != null)
            {
                foreach (var k in _request.Headers)
                    request.Headers[k.Key] = k.Value;
            }

            if (!string.IsNullOrWhiteSpace(_request.Accept))
                request.Accept = _request.Accept;
            if (!string.IsNullOrWhiteSpace(_request.UserAgent))
                request.Headers["User-Agent"] = _request.UserAgent;
            request.CookieContainer = cookie;

            if (_request.HttpMethod == Enum.HttpMethods.GET)
                request.Method = "GET";
            else if (_request.HttpMethod == Enum.HttpMethods.POST)
                request.Method = "POST";
            else if (_request.HttpMethod == Enum.HttpMethods.PUT)
                request.Method = "PUT";
            else if (_request.HttpMethod == Enum.HttpMethods.DELETE)
                request.Method = "DELETE";

            if (!string.IsNullOrWhiteSpace(_request.ContentTypeHeader))
                request.ContentType = _request.ContentTypeHeader;

            return request;
        }

        private static void WriteRequestParas(HttpWebRequest request, string query)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(query);
            using (var requestStream = request.GetRequestStreamAsync().Result)
            {
                requestStream.Write(buffer, 0, buffer.Length);
            }
        }

        private static void WriteRequestParas(HttpWebRequest request, byte[] query)
        {
            using (var requestStream = request.GetRequestStreamAsync().Result)
            {
                requestStream.Write(query, 0, query.Length);
            }
        }

        private static byte[] ReadStreamBytes(Stream sourceStream)
        {
            var bytes = new List<byte>();
            int bufferSize = 1024;
            using (BinaryReader br = new BinaryReader(sourceStream, Encoding.UTF8))
            {
                while (true)
                {
                    byte[] buffer = br.ReadBytes(bufferSize);
                    bytes.AddRange(buffer);
                    if (buffer.Length != bufferSize)
                        break;
                }
            }
            return bytes.ToArray();
        }

        #endregion

        #region sync

        internal static HttpResponse GetResponse<T>(Entity.SimpleWebRequest<T> _request, CookieContainer cookie)
            where T : Entity.SimpleWebResponse
        {
            HttpResponse httpResponse = null;
            HttpWebResponse response = null;

            var request = CreateRequest(_request, ref cookie);
            try
            {
                if (_request.HttpMethod == Enum.HttpMethods.POST)
                {
                    if (_request.QueryBytes == null || _request.QueryBytes.Length == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(_request.QueryString))
                            WriteRequestParas(request, _request.QueryString);
                    }
                    else
                    {
                        WriteRequestParas(request, _request.QueryBytes);
                    }
                }

                response = request.GetResponseAsync().Result as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            httpResponse = new HttpResponse(response);
            var encoding = response.Headers["Content-Encoding"];
            using (Stream s = response.GetResponseStream())
            {
                if (!string.IsNullOrEmpty(encoding))
                {
                    if (encoding.ToLower().Contains("gzip"))
                    {
                        GZipStream st = new GZipStream(s, CompressionMode.Decompress);
                        httpResponse.ResponseBytes = ReadStreamBytes(st);
                    }
                    else if (encoding.ToLower().Contains("deflate"))
                    {
                        DeflateStream st = new DeflateStream(s, CompressionMode.Decompress);
                        httpResponse.ResponseBytes = ReadStreamBytes(st);
                    }
                    else
                    {
                        httpResponse.ResponseBytes = ReadStreamBytes(s);
                    }
                }
                else
                {
                    httpResponse.ResponseBytes = ReadStreamBytes(s);
                }
            }

            response.Dispose();

            return httpResponse;
        }

        internal static HttpResponse DownloadFile<T>(Entity.SimpleWebRequest<T> _request, CookieContainer cookie, string fileSaveFullPath)
            where T : Entity.SimpleWebResponse
        {
            HttpResponse httpResponse = null;
            HttpWebResponse response = null;

            var request = CreateRequest(_request, ref cookie);

            try
            {
                if (_request.HttpMethod == Enum.HttpMethods.POST)
                {
                    if (_request.QueryBytes == null || _request.QueryBytes.Length == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(_request.QueryString))
                            WriteRequestParas(request, _request.QueryString);
                    }
                    else
                    {
                        WriteRequestParas(request, _request.QueryBytes);
                    }
                }

                response = request.GetResponseAsync().Result as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            httpResponse = new HttpResponse(response);

            // Open the report file.
            FileInfo zipFileInfo = new FileInfo(fileSaveFullPath);
            if (!zipFileInfo.Directory.Exists)
            {
                zipFileInfo.Directory.Create();
            }

            using (Stream httpStream = response.GetResponseStream())
            {
                using (FileStream fileStream = new FileStream(zipFileInfo.FullName, FileMode.Create))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(httpStream))
                        {
                            // Read the report and save it to the file.
                            int bufferSize = 10240;
                            long writtenBytes = 0L;
                            while (true)
                            {
                                // Read report data from API.
                                byte[] buffer = binaryReader.ReadBytes(bufferSize);

                                // Write report data to file.
                                binaryWriter.Write(buffer);

                                writtenBytes += buffer.Length;

                                // If the end of the report is reached, break out of the 
                                // loop.
                                if (buffer.Length != bufferSize)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            response.Dispose();

            httpResponse.ResponseBytes = UTF8Encoding.UTF8.GetBytes(zipFileInfo.FullName);
            return httpResponse;
        }

        #endregion

        #region async

        internal static async Task<HttpResponse> GetResponseAsync<T>(Entity.SimpleWebRequest<T> _request, CookieContainer cookie)
            where T : Entity.SimpleWebResponse
        {
            HttpResponse httpResponse = null;
            HttpWebResponse response = null;

            var request = CreateRequest(_request, ref cookie);
            try
            {
                if (_request.HttpMethod == Enum.HttpMethods.POST)
                {
                    if (_request.QueryBytes == null || _request.QueryBytes.Length == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(_request.QueryString))
                            WriteRequestParas(request, _request.QueryString);
                    }
                    else
                    {
                        WriteRequestParas(request, _request.QueryBytes);
                    }
                }

                response = await request.GetResponseAsync() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            httpResponse = new HttpResponse(response);
            var encoding = response.Headers["Content-Encoding"];
            using (Stream s = response.GetResponseStream())
            {
                if (!string.IsNullOrEmpty(encoding))
                {
                    if (encoding.ToLower().Contains("gzip"))
                    {
                        GZipStream st = new GZipStream(s, CompressionMode.Decompress);
                        httpResponse.ResponseBytes = ReadStreamBytes(st);
                    }
                    else if (encoding.ToLower().Contains("deflate"))
                    {
                        DeflateStream st = new DeflateStream(s, CompressionMode.Decompress);
                        httpResponse.ResponseBytes = ReadStreamBytes(st);
                    }
                }
                else
                {
                    httpResponse.ResponseBytes = ReadStreamBytes(s);
                }
            }

            response.Dispose();

            return httpResponse;
        }

        internal static async Task<HttpResponse> DownloadFileAsync<T>(Entity.SimpleWebRequest<T> _request, CookieContainer cookie, string fileSaveFullPath)
            where T : Entity.SimpleWebResponse
        {
            HttpResponse httpResponse = null;
            HttpWebResponse response = null;

            var request = CreateRequest(_request, ref cookie);

            try
            {
                if (_request.HttpMethod == Enum.HttpMethods.POST)
                {
                    if (_request.QueryBytes == null || _request.QueryBytes.Length == 0)
                    {
                        if (!string.IsNullOrWhiteSpace(_request.QueryString))
                            WriteRequestParas(request, _request.QueryString);
                    }
                    else
                    {
                        WriteRequestParas(request, _request.QueryBytes);
                    }
                }
                response = await request.GetResponseAsync() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            httpResponse = new HttpResponse(response);

            // Open the report file.
            FileInfo zipFileInfo = new FileInfo(fileSaveFullPath);
            if (!zipFileInfo.Directory.Exists)
            {
                zipFileInfo.Directory.Create();
            }

            using (Stream httpStream = response.GetResponseStream())
            {
                using (FileStream fileStream = new FileStream(zipFileInfo.FullName, FileMode.Create))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(httpStream))
                        {
                            // Read the report and save it to the file.
                            int bufferSize = 10240;
                            long writtenBytes = 0L;
                            while (true)
                            {
                                // Read report data from API.
                                byte[] buffer = binaryReader.ReadBytes(bufferSize);

                                // Write report data to file.
                                binaryWriter.Write(buffer);

                                writtenBytes += buffer.Length;

                                // If the end of the report is reached, break out of the 
                                // loop.
                                if (buffer.Length != bufferSize)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            response.Dispose();

            httpResponse.ResponseBytes = UTF8Encoding.UTF8.GetBytes(zipFileInfo.FullName);
            return httpResponse;
        }
        #endregion
    }

    internal class HttpResponse
    {
        public string ResposeString
        {
            get
            {
                if (this.ResponseBytes == null || this.ResponseBytes.Length == 0)
                    return null;
                return Encoding.UTF8.GetString(this.ResponseBytes);
            }
        }
        public byte[] ResponseBytes { get; set; }

        public WebHeaderCollection Headers { get; private set; }
        public CookieCollection Cookies { get; private set; }
        public long ContentLength { get; private set; }
        public string ContentType { get; private set; }
        public string Method { get; private set; }
        public Uri ResponseUri { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        public bool SupportsHeaders { get; private set; }

        public HttpResponse(HttpWebResponse response)
        {
            this.Headers = response.Headers;
            this.Cookies = response.Cookies;
            this.ContentLength = response.ContentLength;
            this.ContentType = response.ContentType;
            this.Method = response.Method;
            this.ResponseUri = response.ResponseUri;
            this.StatusCode = response.StatusCode;
            this.StatusDescription = response.StatusDescription;
            this.SupportsHeaders = response.SupportsHeaders;
        }
    }
}