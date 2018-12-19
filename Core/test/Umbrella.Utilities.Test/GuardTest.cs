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
        public void ArgumentNotNull_Null()
            => Assert.Throws<ArgumentNullException>(() => Guard.ArgumentNotNull((object)null, "test"));

        [Fact]
        public void ArgumentNotNull_Valid()
            => Guard.ArgumentNotNull(new object(), "test");

        [Fact]
        public void ArgumentNotNullOrEmpty_IEnumerable_T_Null()
            => Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<string> testArgument = null;

                Guard.ArgumentNotNullOrEmpty(testArgument, nameof(testArgument));
            });

        [Fact]
        public void ArgumentNotNullOrEmpty_IEnumerable_T_Empty()
            => Assert.Throws<ArgumentException>(() =>
            {
                IEnumerable<string> testArgument = new List<string>();

                Guard.ArgumentNotNullOrEmpty(testArgument, nameof(testArgument));
            });

        [Fact]
        public void ArgumentNotNullOrEmpty_IEnumerable_T_Valid()
            => Guard.ArgumentNotNullOrEmpty(new[] { "test" }, "test");

        [Fact]
        public void ArgumentNotNullOrEmpty_Null()
            => Assert.Throws<ArgumentNullException>(() => Guard.ArgumentNotNullOrEmpty(null, "test"));

        [Fact]
        public void ArgumentNotNullOrEmpty_Empty()
            => Assert.Throws<ArgumentException>(() => Guard.ArgumentNotNullOrEmpty("", "test"));

        [Fact]
        public void ArgumentNotNullOrEmpty_Valid()
            => Guard.ArgumentNotNullOrEmpty("test", "test");

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
        public void ArgumentNotNullOrWhiteSpace_Valid()
            => Guard.ArgumentNotNullOrWhiteSpace("test", "test");

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
        public void ArgumentOfTypeExact_Valid()
            => Guard.ArgumentOfTypeExact<A>(new A(), "test");

        [Fact]
        public void ArgumentOfTypeExact_Fails_A_To_B()
            => Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ArgumentOfTypeExact<A>(new B(), "test"));

        [Fact]
        public void ArgumentOfTypeExact_Fails_B_To_A()
            => Assert.Throws<ArgumentOutOfRangeException>(() => Guard.ArgumentOfTypeExact<B>(new A(), "test"));
    }
}