// Copyright © John Gietzen and Contributors. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Xunit;

    public class MarkovChainWithBackoffTests
    {
        public static readonly string EmptySample = @"{}";

        public static object[][] GetSerializationSamples()
        {
            return new[]
          {
            new object[] { "fool", 1, @"{'':{'f':1},'f':{'o':1},'o':{'o':1,'l':1},'l':{'':1}}" },
            new object[] { "fool", 2, @"{'fo':{'o':1,'l':1},'oo':{'o':1,'l':1},'ol':{'':1},'':{'f':1},'f':{'o':1},'o':{'o':1,'l':1},'l':{'':1}}" },
            new object[] { "food", 1, @"{'':{'f':1},'f':{'o':1},'o':{'o':1,'d':1},'d':{'':1}}" },
            new object[] { "food", 2, @"{'fo':{'o':1,'d':1},'oo':{'o':1,'d':1},'od':{'':1},'':{'f':1},'f':{'o':1},'o':{'o':1,'d':1},'d':{'':1}}" },
            new object[] { "loose", 1, @"{'':{'l':1},'l':{'o':1},'o':{'o':1,'s':1},'s':{'e':1},'e':{'':1}}" },
            new object[] { "loose", 2, @"{'lo':{'o':1,'s':1},'oo':{'o':1,'s':1},'os':{'e':1},'se':{'':1},'':{'l':1},'l':{'o':1},'o':{'o':1,'s':1},'s':{'e':1},'e':{'':1}}" },
        };
        }

        public static IEnumerable<object[]> GetSampleDataForOppositeWeights()
        {
            foreach (var word in new string[] { "fool", "food", "loose" })
            {
                foreach (var maximumOrder in new int[] { 1, 2, 3, 10 })
                {
                    yield return new object[] { word, maximumOrder };
                }
            }
        }

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

        [Fact]
        public void GetNextStates_WithoutRestrictiveDesiredNextStatesValue_DoesntBackOff()
        {
            var chainWithBackoff = new MarkovChainWithBackoff<char>(5, 1);
            chainWithBackoff.Add("fool");

            var nextStates = chainWithBackoff.GetNextStates("foo");

            Assert.Equal(nextStates, new Dictionary<char, int> { { 'l', 1 } });
        }

        [Fact]
        public void GetNextStates_WithRestrictiveDesiredNextStatesValue_DoesBackOff()
        {
            var chainWithBackoff = new MarkovChainWithBackoff<char>(5, 2);
            chainWithBackoff.Add("fool");

            var nextStates = chainWithBackoff.GetNextStates("foo");

            Assert.Equal(nextStates, new Dictionary<char, int> { { 'o', 1 }, { 'l', 1 } });
        }

        [Fact]
        public void GetNextStates_WithOverRestrictiveDesiredNextStatesValue_PicksBestPossibleOrder()
        {
            var chainWithBackoff = new MarkovChainWithBackoff<char>(5, 3);
            chainWithBackoff.Add("fool");

            var nextStates = chainWithBackoff.GetNextStates("foo");

            Assert.Equal(nextStates, new Dictionary<char, int> { { 'o', 1 }, { 'l', 1 } });
        }

        [Theory]
        [MemberData(nameof(GetSerializationSamples))]
        public void Add_WhenEmpty_AddsTheValuesToTheState(string sample, int maximumOrder, string serialized)
        {
            var chain = new MarkovChainWithBackoff<char>(maximumOrder, 2);

            chain.Add(sample);

            var result = Serialize(chain);
            Assert.Equal(serialized, result);
        }

        [Theory]
        [MemberData(nameof(GetSampleDataForOppositeWeights))]
        public void Add_WithOppositeWeight_ResetsInternalsToInitialState(string word, int maximumOrder)
        {
            var chain = new MarkovChainWithBackoff<char>(maximumOrder, 2);
            chain.Add(word, 1);

            chain.Add(word, -1);

            var result = Serialize(chain);
            Assert.Equal(EmptySample, result);
        }

        private static string Serialize(MarkovChain<char> chain)
        {
            var states = chain.GetStates().ToDictionary(
                s => string.Concat(s),
                s =>
                {
                    var next = chain.GetNextStates(s) ?? Enumerable.Empty<KeyValuePair<char, int>>();
                    var result = next.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);

                    var terminal = chain.GetTerminalWeight(s);
                    if (terminal > 0)
                    {
                        result[string.Empty] = terminal;
                    }

                    return result;
                });

            return JsonConvert.SerializeObject(states).Replace('\"', '\'');
        }
    }
}
