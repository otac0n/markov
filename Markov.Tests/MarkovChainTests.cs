// Copyright © John Gietzen and Contributors. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Xunit;

    public class MarkovChainTests
    {
        public static readonly string EmptySample = @"{}";

        public static object[][] Samples => new[]
        {
            new[] { "fool", @"{'':{'f':1},'f':{'o':1},'o':{'o':1,'l':1},'l':{'':1}}" },
            new[] { "food", @"{'':{'f':1},'f':{'o':1},'o':{'o':1,'d':1},'d':{'':1}}" },
            new[] { "loose", @"{'':{'l':1},'l':{'o':1},'o':{'o':1,'s':1},'s':{'e':1},'e':{'':1}}" },
        };

        public static object[][] SamplesNoSerialized => new[]
        {
            new[] { "fool" },
            new[] { "food" },
            new[] { "loose" },
        };

        [Theory]
        [MemberData(nameof(Samples))]
        public void Add_WhenEmpty_AddsTheValuesToTheState(string sample, string serialized)
        {
            var chain = new MarkovChain<char>(1);

            chain.Add(sample);

            var result = Serialize(chain);
            Assert.Equal(serialized, result);
        }

        [Theory]
        [MemberData(nameof(SamplesNoSerialized))]
        public void Add_WithOppositeWeight_ResetsInternalsToInitialState(string sample)
        {
            var chain = new MarkovChain<char>(1);
            chain.Add(sample, 1);

            chain.Add(sample, -1);

            var result = Serialize(chain);
            Assert.Equal(EmptySample, result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public void Ctor_WithOrderLessThanZero_ThrowsArgumentOutOfRangeException(int order)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new MarkovChain<char>(order));
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
