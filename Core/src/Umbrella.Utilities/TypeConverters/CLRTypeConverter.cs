using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Umbrella.Utilities.TypeConverters
{
    //TODO: What the hell was this used for? It looks like it's used to convert any .NET object instance to it's .NET type name.
    //Maybe just document this a bit better and keep it in.
    public class CLRTypeConverter : TypeConverter
    {
        #region Public Overridden Methods
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                Type type = Type.GetType((string)value);
                return type;
            }
            return base.ConvertFrom(context, culture, value);
        }

        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((Type)value).FullName;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
}