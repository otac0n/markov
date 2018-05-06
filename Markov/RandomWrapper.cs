// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov
{
    using System;

    /// <summary>
    /// Wraps an instance of <see cref="System.Random"/> to provide the <see cref="IRandom"/> interface.
    /// </summary>
    public class RandomWrapper : IRandom
    {
        /// <summary>
        /// Holds the instance of <see cref="System.Random"/> being wrapped.
        /// </summary>
        private readonly Random rand;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomWrapper"/> class, wrapping a given <see cref="System.Random"/> instance.
        /// </summary>
        /// <param name="rand">The instance of <see cref="System.Random"/> to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public RandomWrapper(Random rand)
        {
            this.rand = rand ?? throw new ArgumentNullException(nameof(rand));
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; that is, the range of return values ordinarily includes
        /// zero but not maxValue. However, if maxValue equals zero, maxValue is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than zero</exception>
        int IRandom.Next(int maxValue) => this.rand.Next(maxValue);
    }
}
