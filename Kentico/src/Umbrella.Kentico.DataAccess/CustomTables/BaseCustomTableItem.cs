using System.Runtime.CompilerServices;
using CMS.CustomTables;
using CMS.Helpers;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Kentico.DataAccess.CustomTables
{
	public abstract class BaseCustomTableItem : CustomTableItem
	{
		public BaseCustomTableItem(string tableName)
			: base(tableName)
		{
		}

		protected T GetPropertyValue<T>(T defaultValue = default, [CallerMemberName] string propertyName = "")
		{
			if (typeof(T).IsEnum)
			{
				int enumValue = GetIntegerValue(propertyName, -1);

				// For new items, the item will initially be stored on the object as an Enum, not an integer
				// so we need to read it as an enum. After it has been saved, it will be an integer and will
				// need to be converted.
				if (enumValue == -1 && ItemID == 0)
					return ValidationHelper.GetValue(GetValue(propertyName, defaultValue), defaultValue);

				return ValidationHelper.GetValue(enumValue.ToEnum(defaultValue), defaultValue);
			}

			return ValidationHelper.GetValue(GetValue(propertyName, defaultValue), defaultValue);
		}

		protected void SetPropertyValue<T>(T value, [CallerMemberName] string propertyName = "")
			=> SetValue(propertyName, value);
	}
}