// Copyright © John Gietzen and Contributors. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Builds and walks interconnected states based on a weighted probability.
    /// </summary>
    /// <typeparam name="T">The type of the constituent parts of each state in the Markov chain.</typeparam>
    public class MarkovChain<T>
        where T : IEquatable<T>
    {
        private readonly Dictionary<ChainState<T>, Dictionary<T, int>> items = new Dictionary<ChainState<T>, Dictionary<T, int>>();
        private readonly int order;
        private readonly Dictionary<ChainState<T>, int> terminals = new Dictionary<ChainState<T>, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkovChain{T}"/> class.
        /// </summary>
        /// <param name="order">Indicates the desired order of the <see cref="MarkovChain{T}"/>.</param>
        /// <remarks>
        /// <para>The <paramref name="order"/> of a generator indicates the depth of its internal state.  A generator
        /// with an order of 1 will choose items based on the previous item, a generator with an order of 2
        /// will choose items based on the previous 2 items, and so on.</para>
        /// <para>Zero is not classically a valid order value, but it is allowed here.  Choosing a zero value has the
        /// effect that every state is equivalent to the starting state, and so items will be chosen based on their
        /// total frequency.</para>
        /// </remarks>
        public MarkovChain(int order)
        {
            if (order < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(order));
            }

            this.order = order;
        }

        /// <summary>
        /// Gets the order of the chain.
        /// </summary>
        public int Order => this.order;

        /// <summary>
        /// Adds the items to the generator with a weight of one.
        /// </summary>
        /// <param name="items">The items to add to the generator.</param>
        public void Add(IEnumerable<T> items) => this.Add(items, 1);

        /// <summary>
        /// Adds the items to the generator with the weight specified.
        /// </summary>
        /// <param name="items">The items to add to the generator.</param>
        /// <param name="weight">The weight at which to add the items.</param>
        public void Add(IEnumerable<T> items, int weight)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var previous = new Queue<T>();
            foreach (var item in items)
            {
                var key = new ChainState<T>(previous);

                this.Add(key, item, weight);

                previous.Enqueue(item);
                if (previous.Count > this.order)
                {
                    previous.Dequeue();
                }
            }

            this.AddTerminalInternal(new ChainState<T>(previous), weight);
        }

        /// <summary>
        /// Adds the item to the generator, with the specified items preceding it.
        /// </summary>
        /// <param name="previous">The items preceding the item.</param>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// See <see cref="MarkovChain{T}.Add(IEnumerable{T}, T, int)"/> for remarks.
        /// </remarks>
        public void Add(IEnumerable<T> previous, T item)
        {
            if (previous == null)
            {
                throw new ArgumentNullException(nameof(previous));
            }

            var state = new Queue<T>(previous);
            while (state.Count > this.order)
            {
                state.Dequeue();
            }

            this.Add(new ChainState<T>(state), item, 1);
        }

        /// <summary>
        /// Adds the item to the generator, with the specified state preceding it.
        /// </summary>
        /// <param name="state">The state preceding the item.</param>
        /// <param name="next">The item to add.</param>
        /// <remarks>
        /// See <see cref="MarkovChain{T}.Add(ChainState{T}, T, int)"/> for remarks.
        /// </remarks>
        public void Add(ChainState<T> state, T next) => this.Add(state, next, 1);

        /// <summary>
        /// Adds the item to the generator, with the specified items preceding it and the specified weight.
        /// </summary>
        /// <param name="previous">The items preceding the item.</param>
        /// <param name="item">The item to add.</param>
        /// <param name="weight">The weight of the item to add.</param>
        /// <remarks>
        /// This method does not add all of the preceding states to the generator.
        /// Notably, the empty state is not added, unless the <paramref name="previous"/> parameter is empty.
        /// </remarks>
        public void Add(IEnumerable<T> previous, T item, int weight)
        {
            if (previous == null)
            {
                throw new ArgumentNullException(nameof(previous));
            }

            var state = new Queue<T>(previous);
            while (state.Count > this.order)
            {
                state.Dequeue();
            }

            this.Add(new ChainState<T>(state), item, weight);
        }

        /// <summary>
        /// Adds the item to the generator, with the specified state preceding it and the specified weight.
        /// </summary>
        /// <param name="state">The state preceding the item.</param>
        /// <param name="next">The item to add.</param>
        /// <param name="weight">The weight of the item to add.</param>
        /// <remarks>
        /// This adds the state as-is.  The state may not be reachable if, for example, the
        /// number of items in the state is greater than the order of the generator, or if the
        /// combination of items is not available in the other states of the generator.
        ///
        /// A negative weight may be passed, which will have the impact of reducing the weight
        /// of the specified state transition.  This can therefore be used to remove items from
        /// the generator. The resulting weight will never be allowed below zero.
        /// </remarks>
        public virtual void Add(ChainState<T> state, T next, int weight)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (!this.items.TryGetValue(state, out var weights))
            {
                weights = new Dictionary<T, int>();
                this.items.Add(state, weights);
            }

            var newWeight = Math.Max(0, weights.ContainsKey(next)
                ? weight + weights[next]
                : weight);
            if (newWeight == 0)
            {
                weights.Remove(next);
                if (weights.Count == 0)
                {
                    this.items.Remove(state);
                }
            }
            else
            {
                weights[next] = newWeight;
            }
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of the items chosen.</returns>
        /// <remarks>Assumes an empty starting state.</remarks>
        public IEnumerable<T> Chain() => this.Chain(Enumerable.Empty<T>(), new Random());

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="previous">The items preceding the first item in the chain.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the items chosen.</returns>
        public IEnumerable<T> Chain(IEnumerable<T> previous) => this.Chain(previous, new Random());

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="seed">The seed for the random number generator, used as the random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the items chosen.</returns>
        /// <remarks>Assumes an empty starting state.</remarks>
        public IEnumerable<T> Chain(int seed) => this.Chain(Enumerable.Empty<T>(), new Random(seed));

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="previous">The items preceding the first item in the chain.</param>
        /// <param name="seed">The seed for the random number generator, used as the random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the items chosen.</returns>
        public IEnumerable<T> Chain(IEnumerable<T> previous, int seed) => this.Chain(previous, new Random(seed));

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="rand">The random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the items chosen.</returns>
        /// <remarks>Assumes an empty starting state.</remarks>
        public IEnumerable<T> Chain(Random rand) => this.Chain(Enumerable.Empty<T>(), rand);

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="previous">The items preceding the first item in the chain.</param>
        /// <param name="rand">The random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of the items chosen.</returns>
        public IEnumerable<T> Chain(IEnumerable<T> previous, Random rand)
        {
            if (previous == null)
            {
                throw new ArgumentNullException(nameof(previous));
            }
            else if (rand == null)
            {
                throw new ArgumentNullException(nameof(rand));
            }

            var state = new Queue<T>(previous);
            while (true)
            {
                while (state.Count > this.order)
                {
                    state.Dequeue();
                }

                var key = new ChainState<T>(state);

                var weights = this.GetNextStatesInternal(key);
                if (weights == null)
                {
                    yield break;
                }

                var terminalWeight = this.GetTerminalWeight(key);

                var total = weights.Sum(w => w.Value);
                var value = rand.Next(total + terminalWeight) + 1;

                if (value > total)
                {
                    yield break;
                }

                var currentWeight = 0;
                foreach (var nextItem in weights)
                {
                    currentWeight += nextItem.Value;
                    if (currentWeight >= value)
                    {
                        yield return nextItem.Key;
                        state.Enqueue(nextItem.Key);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the items from the generator that follow from an empty state.
        /// </summary>
        /// <returns>A dictionary of the items and their weight.</returns>
        public Dictionary<T, int> GetInitialStates() => this.GetNextStates(new ChainState<T>(Enumerable.Empty<T>()));

        /// <summary>
        /// Gets the items from the generator that follow from the specified items preceding it.
        /// </summary>
        /// <param name="previous">The items preceding the items of interest.</param>
        /// <returns>A dictionary of the items and their weight.</returns>
        public Dictionary<T, int> GetNextStates(IEnumerable<T> previous)
        {
            var state = new Queue<T>(previous);
            while (state.Count > this.order)
            {
                state.Dequeue();
            }

            return this.GetNextStates(new ChainState<T>(state));
        }

        /// <summary>
        /// Gets the items from the generator that follow from the specified state preceding it.
        /// </summary>
        /// <param name="state">The state preceding the items of interest.</param>
        /// <returns>A dictionary of the items and their weight.</returns>
        public Dictionary<T, int> GetNextStates(ChainState<T> state)
        {
            var weights = this.GetNextStatesInternal(state);
            return weights != null ? new Dictionary<T, int>(weights) : null;
        }

        /// <summary>
        /// Gets all of the states that exist in the generator.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="ChainState{T}"/> containing all of the states in the generator.</returns>
        public virtual IEnumerable<ChainState<T>> GetStates()
        {
            foreach (var state in this.items.Keys)
            {
                yield return state;
            }

            foreach (var state in this.terminals.Keys)
            {
                if (!this.items.ContainsKey(state))
                {
                    yield return state;
                }
            }
        }

        /// <summary>
        /// Gets the weight of termination following from the specified items.
        /// </summary>
        /// <param name="previous">The items preceding the items of interest.</param>
        /// <returns>A dictionary of the items and their weight.</returns>
        public int GetTerminalWeight(IEnumerable<T> previous)
        {
            var state = new Queue<T>(previous);
            while (state.Count > this.order)
            {
                state.Dequeue();
            }

            return this.GetTerminalWeight(new ChainState<T>(state));
        }

        /// <summary>
        /// Gets the weights of termination following from the specified state.
        /// </summary>
        /// <param name="state">The state preceding the items of interest.</param>
        /// <returns>A dictionary of the items and their weight.</returns>
        public virtual int GetTerminalWeight(ChainState<T> state)
        {
            this.terminals.TryGetValue(state, out var weight);
            return weight;
        }

        /// <summary>
        /// Add a terminal with the given weight. Can be overridden by inheriting classes.
        /// </summary>
        /// <param name="state">The state preceding the items of interest.</param>
        /// <param name="weight">The weight of the terminal to add.</param>
        protected internal virtual void AddTerminalInternal(ChainState<T> state, int weight)
        {
            var newWeight = Math.Max(0, this.terminals.ContainsKey(state)
                ? weight + this.terminals[state]
                : weight);
            if (newWeight == 0)
            {
                this.terminals.Remove(state);
            }
            else
            {
                this.terminals[state] = newWeight;
            }
        }

        /// <summary>
        /// Gets the items from the generator that follow from the specified state preceding it without copying the values.
        /// </summary>
        /// <param name="state">The state preceding the items of interest.</param>
        /// <returns>The raw dictionary of the items and their weight.</returns>
        protected internal virtual Dictionary<T, int> GetNextStatesInternal(ChainState<T> state) =>
            this.items.TryGetValue(state, out var weights)
                ? weights
                : null;
    }
}
