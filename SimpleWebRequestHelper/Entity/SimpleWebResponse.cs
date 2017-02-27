using System;

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
    }
}