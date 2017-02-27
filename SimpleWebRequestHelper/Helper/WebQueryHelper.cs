using System;

namespace SimpleWebRequestHelper.Helper
{
    public static class WebQueryHelper
    {
        public static string UrlEncode(this string para)
        {
            if (string.IsNullOrEmpty(para))
                return "";
            return System.Web.HttpUtility.UrlEncode(para);
        }

        public static string UrlDecode(this string encodedUrl)
        {
            if (string.IsNullOrEmpty(encodedUrl))
                return "";
            return System.Web.HttpUtility.UrlDecode(encodedUrl);
        }

        public static string HtmlEncode(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return "";
            return System.Web.HttpUtility.HtmlEncode(html);
        }

        public static string HtmlDecode(this string encodedHtml)
        {
            if (string.IsNullOrEmpty(encodedHtml))
                return "";
            return System.Web.HttpUtility.HtmlDecode(encodedHtml);
        }
    }
}