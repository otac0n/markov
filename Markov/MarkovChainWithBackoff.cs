// Copyright © John Gietzen and Contributors. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Builds and walks interconnected states based on probabilities probed at different depths.
    /// </summary>
    /// <typeparam name="T">The type of the constituent parts of each state in the Markov chain.</typeparam>
    public class MarkovChainWithBackoff<T> : MarkovChain<T>
        where T : IEquatable<T>
    {
        private readonly List<MarkovChain<T>> chains = new List<MarkovChain<T>>();
        private readonly int desiredNumNextStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkovChainWithBackoff{T}"/> class.
        /// </summary>
        /// <param name="maximumOrder">Indicates the maximum/starting order of the <see cref="MarkovChainWithBackoff{T}"/>.</param>
        /// <param name="desiredNumNextStates">Indicates the desired number of next states of the <see cref="MarkovChainWithBackoff{T}"/>.</param>
        /// <remarks>
        /// <para>The order of a traditional markov generator indicates the depth of its internal state. A generator
        /// with an order of 1 will choose items based on the previous item, a generator with an order of 2
        /// will choose items based on the previous 2 items, and so on.</para>
        /// <para>In a <see cref="MarkovChainWithBackoff{T}"/> we start with an order of <paramref name="maximumOrder"/>.
        /// We peak at the next possible and if we have more than <paramref name="desiredNumNextStates"/> we generate
        /// at that order. If we don't have enough next possible states we lower the order by 1 and repeat. If we reach
        /// an order of one we generate regardless of the number of possible states.</para>
        /// <para>One is the lowest valid <paramref name="maximumOrder"/> and zero is the lowest valid<paramref name="desiredNumNextStates"/>.</para>
        /// </remarks>
        public MarkovChainWithBackoff(int maximumOrder, int desiredNumNextStates)
            : base(maximumOrder)
        {
            if (maximumOrder < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumOrder));
            }

            if (desiredNumNextStates < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(desiredNumNextStates));
            }

            this.desiredNumNextStates = desiredNumNextStates;

            for (var order = maximumOrder - 1; order > 0; order--)
            {
                this.chains.Add(new MarkovChain<T>(order));
            }
        }

        /// <inheritdoc/>
        public override void Add(ChainState<T> state, T next, int weight)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            for (var orderTarget = this.Order; orderTarget >= 1; orderTarget--)
            {
                if (orderTarget == this.Order)
                {
                    base.Add(state, next, weight);
                }
                else
                {
                    if (orderTarget < state.Count)
                    {
                        state = new ChainState<T>(state.Skip(state.Count - orderTarget));
                    }

                    this.chains[this.Order - orderTarget - 1].Add(state, next, weight);
                }
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<ChainState<T>> GetStates()
        {
            foreach (var state in base.GetStates())
            {
                yield return state;
            }

            foreach (var chain in this.chains)
            {
                foreach (var state in chain.GetStates())
                {
                    yield return state;
                }
            }
        }

        /// <inheritdoc/>
        public override int GetTerminalWeight(ChainState<T> state)
        {
            var orderTarget = this.GetDesiredOrderTarget(ref state, out var _);
            return orderTarget == this.Order
                ? base.GetTerminalWeight(state)
                : this.chains[this.Order - orderTarget - 1].GetTerminalWeight(state);
        }

        /// <inheritdoc/>
        protected internal override void AddTerminalInternal(ChainState<T> state, int weight)
        {
            for (var orderTarget = this.Order; orderTarget >= 1; orderTarget--)
            {
                if (orderTarget == this.Order)
                {
                    base.AddTerminalInternal(state, weight);
                }
                else
                {
                    if (orderTarget < state.Count)
                    {
                        state = new ChainState<T>(state.Skip(state.Count - orderTarget));
                    }

                    this.chains[this.Order - orderTarget - 1].AddTerminalInternal(state, weight);
                }
            }
        }

        /// <inheritdoc/>
        protected internal override Dictionary<T, int> GetNextStatesInternal(ChainState<T> state)
        {
            this.GetDesiredOrderTarget(ref state, out var nextStates);
            return nextStates;
        }

        private int GetDesiredOrderTarget(ref ChainState<T> state, out Dictionary<T, int> nextStates)
        {
            for (var orderTarget = this.Order; ; orderTarget--)
            {
                if (orderTarget == this.Order)
                {
                    nextStates = base.GetNextStatesInternal(state);
                }
                else
                {
                    if (orderTarget < state.Count)
                    {
                        state = new ChainState<T>(state.Skip(state.Count - orderTarget));
                    }

                    nextStates = this.chains[this.Order - orderTarget - 1].GetNextStatesInternal(state);
                }

                if (nextStates?.Count >= this.desiredNumNextStates || orderTarget <= 1)
                {
                    return orderTarget;
                }
            }
        }
    }
}
