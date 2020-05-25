// Copyright © John Gietzen and Contributors. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class MarkovChainWithBackoffTests
    {
        [Fact]
        public void Chain_WithRestrictiveMaximumOrder_DoesntAlwaysQuoteCorpus()
        {
            var chainWithBackoff = new MarkovChainWithBackoff<char>(5, 2);
            chainWithBackoff.Add("fool");
            var deterministicRand = new Random(0);
            var resultWithBackoff = string.Join("", chainWithBackoff.Chain(deterministicRand));
            Assert.Equal("fol", resultWithBackoff);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public void Ctor_WithDesiredNumNextStatesLessThanZero_ThrowsArgumentOutOfRangeException(int desiredNumNextStates)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MarkovChainWithBackoff<char>(10, desiredNumNextStates));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public void Ctor_WithMaximumOrderLessThanOne_ThrowsArgumentOutOfRangeException(int maximumOrder)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MarkovChainWithBackoff<char>(maximumOrder, 10));
        }
    }
}
