using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Umbrella.DataAccess.EntityFrameworkCore.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="PropertyEntry{TEntity, TProperty}" /> class.
	/// </summary>
	public static class PropertyEntryExtensions
	{
		/// <summary>
		/// Checks for changes to the value of a property that is being tracked by a <see cref="DbContext"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="property">The property to check for changes.</param>
		/// <returns><see langword="true"/> if the value of the <paramref name="property"/> has changed; otherwise <see langword="false"/></returns>
		public static bool HasChanged<TEntity, TProperty>(this PropertyEntry<TEntity, TProperty> property)
			where TEntity : class
		{
			TProperty currentValue = property.CurrentValue;
			TProperty originalValue = property.OriginalValue;

			if (currentValue is null && originalValue is null)
				return false;

			if (currentValue is null && originalValue != null)
				return true;

			if (currentValue != null && originalValue is null)
				return true;

			return currentValue!.Equals(originalValue) is false;
		}
	}
}