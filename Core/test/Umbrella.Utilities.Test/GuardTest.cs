using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Umbrella.Utilities.Test
{
    public class GuardTest
    {
        public class A
        {
        }

        public class B : A
        {
        }

        [Fact]
        public void ArgumentNotNull()
            => Assert.Throws<ArgumentNullException>(() => Guard.ArgumentNotNull(null, "test"));

        [Fact]
        public void ArgumentNotNullOrEmpty_IList_T()
            => Assert.Throws<ArgumentNullException>(() =>
            {
                IList<string> testArgument = null;

                Guard.ArgumentNotNullOrEmpty(testArgument, nameof(testArgument));
            });

        [Fact]
        public void ArgumentNotNullOrEmpty_ICollection_T()
            => Assert.Throws<ArgumentNullException>(() =>
            {
                ICollection<string> testArgument = null;

                Guard.ArgumentNotNullOrEmpty(testArgument, nameof(testArgument));
            });

        [Fact]
        public void ArgumentNotNullOrEmpty_IEnumerable_T()
            => Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<string> testArgument = null;

                Guard.ArgumentNotNullOrEmpty(testArgument, nameof(testArgument));
            });

        [Fact]
        public void ArgumentNotNullOrWhiteSpace_Null()
            => Assert.Throws<ArgumentNullException>(() => Guard.ArgumentNotNullOrWhiteSpace(null, "test"));

        [Fact]
        public void ArgumentNotNullOrWhiteSpace_Empty()
            => Assert.Throws<ArgumentException>(() => Guard.ArgumentNotNullOrWhiteSpace("", "test"));

        [Fact]
        public void ArgumentNotNullOrWhiteSpace_WhiteSpace()
            => Assert.Throws<ArgumentException>(() => Guard.ArgumentNotNullOrWhiteSpace("    ", "test"));

        [Fact]
        public void ArgumentOfType_Null()
            => Assert.Throws<ArgumentNullException>(() => Guard.ArgumentOfType<A>(null, "test"));

        [Fact]
        public void ArgumentOfType_A_Valid()
            => Guard.ArgumentOfType<A>(new A(), "test");

        [Fact]
        public void ArgumentOfType_A_DerivesFrom_A_Valid()
            => Guard.ArgumentOfType<A>(new B(), "test");

        [Fact]
        public void ArgumentOfType_B_Invalid()
            => Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ArgumentOfType<B>(new A(), "test"));

        [Fact]
        public void ArgumentOfTypeExact_Null()
            => Assert.Throws<ArgumentNullException>(() => Guard.ArgumentOfTypeExact<A>(null, "test"));

        [Fact]
        public void ArgumentOfTypeExact_Passes()
            => Guard.ArgumentOfTypeExact<A>(new A(), "test");

        [Fact]
        public void ArgumentOfTypeExact_Fails_A_To_B()
            => Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ArgumentOfTypeExact<A>(new B(), "test"));

        [Fact]
        public void ArgumentOfTypeExact_Fails_B_To_A()
            => Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ArgumentOfTypeExact<B>(new A(), "test"));
    }
}