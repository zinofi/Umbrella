using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Comparers
{
    public class GenericEqualityComparer<TObject, TProperty> : EqualityComparer<TObject>
    {
        private readonly Func<TObject, TProperty> _PropertySelector;

        public GenericEqualityComparer(Func<TObject, TProperty> propertySelector)
            => _PropertySelector = propertySelector;

        public override bool Equals(TObject x, TObject y)
            => GetHashCode(x) == GetHashCode(y);

        public override int GetHashCode(TObject obj)
            => _PropertySelector(obj).GetHashCode();
    }
}