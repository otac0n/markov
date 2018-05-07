// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class ChainStateTests
    {
        public static string[] Samples => new[]
        {
            "a",
            "ab",
            "abc",
            "aaa",
        };

        public static object[][] ValuePairs =>
            (from v1 in Samples
             from v2 in Samples
             where v1 != v2
             select new[] { v1, v2 }).ToArray();

        public static object[][] Values =>
            (from v in Samples
             select new[] { v }).ToArray();

        [Fact]
        public void Ctor_WithNullArray_ThrowsArgumentNullException()
        {
            char[] value = null;

            Assert.Throws<ArgumentNullException>(
                () => new ChainState<char>(value));
        }

        [Fact]
        public void Ctor_WithNullEnumerable_ThrowsArgumentNullException()
        {
            IEnumerable<char> value = null;

            Assert.Throws<ArgumentNullException>(
                () => new ChainState<char>(value));
        }

        [Theory]
        [MemberData(nameof(ValuePairs))]
        public void Equals_WithDifferentValues_ReturnsFalse(string value1, string value2)
        {
            var a = new ChainState<char>(value1.ToCharArray());
            var b = new ChainState<char>(value2.ToCharArray());

            var result = a.Equals(b);

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Equals_WithNullReference_ReturnsFalse(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = default(ChainState<char>);

            var result = a.Equals(b);

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Equals_WithTheSameReference_ReturnsTrue(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = a;

            var result = a.Equals(b);

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Equals_WithTheSameValue_ReturnsTrue(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = new ChainState<char>(value.ToCharArray());

            var result = a.Equals(b);

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(ValuePairs))]
        public void OpEquality_WithDifferentValues_ReturnsFalse(string value1, string value2)
        {
            var a = new ChainState<char>(value1.ToCharArray());
            var b = new ChainState<char>(value2.ToCharArray());

            var result = a == b;

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void OpEquality_WithNullReference_ReturnsFalse(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = default(ChainState<char>);

            var result = a == b;

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void OpEquality_WithTheSameReference_ReturnsTrue(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = a;

            var result = a == b;

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void OpEquality_WithTheSameValue_ReturnsTrue(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = new ChainState<char>(value.ToCharArray());

            var result = a == b;

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(ValuePairs))]
        public void OpInequality_WithDifferentValues_ReturnsTrue(string value1, string value2)
        {
            var a = new ChainState<char>(value1.ToCharArray());
            var b = new ChainState<char>(value2.ToCharArray());

            var result = a != b;

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void OpInequality_WithNullReference_ReturnsTrue(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = default(ChainState<char>);

            var result = a != b;

            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void OpInequality_WithTheSameReference_ReturnsFalse(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = a;

            var result = a != b;

            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void OpInequality_WithTheSameValue_ReturnsFalse(string value)
        {
            var a = new ChainState<char>(value.ToCharArray());
            var b = new ChainState<char>(value.ToCharArray());

            var result = a != b;

            Assert.False(result);
        }
    }
}
