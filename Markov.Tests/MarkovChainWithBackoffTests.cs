// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class MarkovChainWithBackoffTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public void Ctor_WithMaximumOrderLessThanOne_ThrowsArgumentOutOfRangeException(int maximumOrder)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MarkovChainWithBackoff<char>(maximumOrder, 10));
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

        [Fact]
        public void Chain_WithRestrictiveMaximumOrder_DoesntAlwaysQuoteCorpus()
        {
            MarkovChainWithBackoff<char> chainWithBackoff = new MarkovChainWithBackoff<char>(5, 2);
            chainWithBackoff.Add("fool");
            Random deterministicRand = new Random(0);
            string resultWithBackoff = string.Join("", chainWithBackoff.Chain(deterministicRand));
            Assert.Equal("fol", resultWithBackoff);
        }
    }
}