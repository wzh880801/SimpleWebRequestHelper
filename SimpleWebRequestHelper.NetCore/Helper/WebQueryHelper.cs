using System;

namespace SimpleWebRequestHelper.Helper
{
    public static class WebQueryHelper
    {
        public static string UrlEncode(this string para)
        {
            if (string.IsNullOrEmpty(para))
                return "";
            return System.Net.WebUtility.UrlEncode(para);
        }

        public static string UrlDecode(this string encodedUrl)
        {
            if (string.IsNullOrEmpty(encodedUrl))
                return "";
            return System.Net.WebUtility.UrlDecode(encodedUrl);
        }

        public static string HtmlEncode(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";
            return System.Net.WebUtility.HtmlEncode(html);
        }

        public static string HtmlDecode(this string encodedHtml)
        {
            if (string.IsNullOrEmpty(encodedHtml))
                return "";
            return System.Net.WebUtility.HtmlDecode(encodedHtml);
        }
    }
}