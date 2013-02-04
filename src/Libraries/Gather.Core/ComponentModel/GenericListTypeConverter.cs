using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Gather.Core.ComponentModel
{
    public class GenericListTypeConverter<T> : TypeConverter
    {

        protected readonly TypeConverter TypeConverter;

        public GenericListTypeConverter()
        {
            TypeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (TypeConverter == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);
        }

        protected virtual string[] GetStringArray(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                string[] result = input.Split(',');
                Array.ForEach(result, s => s.Trim());
                return result;
            }
            return new string[0];
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                string[] items = GetStringArray(sourceType.ToString());
                return (items.Any());
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var input = value as string;
            if (input != null)
            {
                string[] items = GetStringArray(input);
                var result = new List<T>();
                Array.ForEach(items, s =>
                {
                    object item = TypeConverter.ConvertFromInvariantString(s);
                    if (item != null)
                    {
                        result.Add((T)item);
                    }
                });

                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string result = string.Empty;
                if (value != null)
                    result = ((IList<T>) value).Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)).Aggregate((current, x) => current + "," + x).TrimEnd(',');
                return result;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

    }
}