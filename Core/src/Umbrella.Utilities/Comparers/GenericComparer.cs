using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Comparers
{
	public class GenericComparer<TObject> : GenericComparer<TObject, TObject>
	{
		public GenericComparer(
			Func<TObject, TObject> propertySelector,
			Func<TObject, TObject, int> customComparer = null)
			: base(propertySelector, customComparer)
		{
		}
	}

	public class GenericComparer<TObject , TProperty> : Comparer<TObject>
	{
		private readonly Func<TObject, TProperty> m_PropertySelector;
		private readonly Func<TObject, TObject, int> m_CustomComparer;

		/// <summary>
		/// Create a new instance of the comparer using the specified delegate to select the value used to check
		/// for object instance equality. If a custom comparer is specified then that will be used instead with the property
		/// selector only being used to determine the HashCode for an object instance.
		/// <para>
		/// If a custom comparer is not specified then equality will be determined by trying to get values from each object using the <paramref name="propertySelector"/> and comparing those.
		/// </para>
		/// </summary>
		/// <param name="propertySelector">The property selector delegate.</param>
		/// <param name="customComparer">An optional delegate for custom comparison instead of using the <paramref name="propertySelector"/> if possible.</param>
		public GenericComparer(Func<TObject, TProperty> propertySelector, Func<TObject, TObject, int> customComparer = null)
		{
			Guard.ArgumentNotNull(propertySelector, nameof(propertySelector));

			m_PropertySelector = propertySelector;
			m_CustomComparer = customComparer;
		}

		public override int Compare(TObject x, TObject y)
		{
			// Check the objects for null combinations first.
			if (x == null && y == null)
				return 1;

			if (x == null)
				return 0;

			if (y == null)
				return 0;

			if (m_CustomComparer != null)
				return m_CustomComparer(x, y);

			TProperty xProperty = m_PropertySelector(x);
			TProperty yProperty = m_PropertySelector(y);

			if (xProperty == null && yProperty == null)
				return 1;

			if (xProperty == null)
				return 0;

			if (yProperty == null)
				return 0;

			// Check if we can compare the 2 values using the IComparable interface.
			if (xProperty is IComparable xPropertyComparable && yProperty is IComparable yPropertyComparable)
				return xPropertyComparable.CompareTo(yPropertyComparable);

			throw new UmbrellaException("It has not been possible to compare the two instances of type: " + typeof(TObject).FullName);
		}
	}
}