using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Gather.Core.Infrastructure;

namespace Gather.Core
{
    public static class Extensions
    {

        public static string EncodeEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
                return email;

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            return webHelper.HtmlFullEncode(email);
        }

        public static string EncodeEmails(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            return Regex.Replace(text, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", match => webHelper.HtmlFullEncode(match.ToString()));
        }

        public static string GetDescription(this object enumerationValue)
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");

            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);

                if (attrs.Length > 0)
                    return ((DescriptionAttribute) attrs[0]).Description;
            }

            return enumerationValue.ToString();
        }

        public static string StripHtml(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return Regex.Replace(text, @"<[^>]*>", string.Empty);
        }

    }
}