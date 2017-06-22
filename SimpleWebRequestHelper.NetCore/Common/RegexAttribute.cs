using System;
using System.Text.RegularExpressions;

namespace SimpleWebRequestHelper.Common
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class RegexAttribute : Attribute
    {
        public RegexAttribute(
            string key,
            string pattern,
            RegexOptions options = RegexOptions.IgnoreCase,
            bool needUrlDecode = false,
            bool needHtmlDecode = false)
        {
            this.Key = key;
            this.RegexPattern = pattern;
            this.RegexOptions = options;
            this.NeedUrlDecode = needUrlDecode;
            this.NeedHtmlDecode = needHtmlDecode;
        }

        public RegexAttribute()
        {
            this.RegexOptions = RegexOptions.IgnoreCase;
            this.NeedUrlDecode = false;
            this.NeedHtmlDecode = false;
        }

        public string Key { get; set; }
        public string RegexPattern { get; set; }
        public RegexOptions RegexOptions { get; set; }
        public bool NeedUrlDecode { get; set; }
        public bool NeedHtmlDecode { get; set; }
    }
}