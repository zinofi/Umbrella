using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.TypeConverters.Abstractions
{
	public interface IGenericTypeConverter
	{
		T Convert<T>(string value, Func<T> fallbackCreator, Func<string, T> customValueConverter = null);
		T Convert<T>(string value, T fallback = default, Func<string, T> customValueConverter = null);
		T ConvertToEnum<T>(string value, T fallback = default) where T : struct, Enum;
	}
}