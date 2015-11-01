using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Comparers
{
    public class GenericEqualityComparer<T> : EqualityComparer<T>
    {
        private readonly Func<T, object> m_PropertySelector;

        public GenericEqualityComparer(Func<T, object> propertySelector)
        {
            m_PropertySelector = propertySelector;
        }

        public override bool Equals(T x, T y)
        {
            return GetHashCode(x) == GetHashCode(y);
        }

        public override int GetHashCode(T obj)
        {
            return m_PropertySelector(obj).GetHashCode();
        }
    }
}
