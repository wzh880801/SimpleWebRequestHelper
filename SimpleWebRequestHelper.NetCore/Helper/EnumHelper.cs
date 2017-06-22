using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleWebRequestHelper.Helper
{
    public static class EnumHelper
    {
        public static Dictionary<int, string> ToDictionary(this Type _enumType)
        {
            var _typeInfo = _enumType.GetTypeInfo();
            if (!_typeInfo.IsEnum)
                throw new InvalidCastException("Only support enum type!");
            var dic = new Dictionary<int, string>();
            var ps = _enumType.GetFields();
            foreach (var p in ps)
            {
                if (p.FieldType != _enumType)
                    continue;

                var at = p.GetCustomAttribute(typeof(EnumDescriptionAttribute));
                if (at != null)
                {
                    var a = at as EnumDescriptionAttribute;
                    dic.Add(Convert.ToInt32(p.GetValue(_enumType)), a.Text);
                }
                else
                {
                    dic.Add(Convert.ToInt32(p.GetValue(_enumType)), p.Name);
                }
            }

            return dic;
        }
    }
}