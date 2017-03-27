using System;
using System.Net;
using Newtonsoft.Json;

namespace SimpleWebRequestHelper.Entity
{
    public abstract class SimpleWebResponse
    {
        private Enum.ResponseType _reponseType = Enum.ResponseType.HTML;
        public virtual Enum.ResponseType ResponseType
        {
            get
            {
                return _reponseType;
            }
        }

        public virtual string ResponseBase64String { get; set; }

        [JsonIgnore]
        public virtual WebHeaderCollection Headers { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}