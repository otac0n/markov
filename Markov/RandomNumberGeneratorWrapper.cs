// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Wraps an instance of <see cref="RandomNumberGenerator"/> to provide the <see cref="IRandom"/> interface.
    /// </summary>
    public class RandomNumberGeneratorWrapper : IRandom
    {
        /// <summary>
        /// Holds the instance of <see cref="RandomNumberGenerator"/> being wrapped.
        /// </summary>
        private readonly RandomNumberGenerator rand;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomNumberGeneratorWrapper"/> class, wrapping a given <see cref="RandomNumberGenerator"/> instance.
        /// </summary>
        /// <param name="rand">The instance of <see cref="RandomNumberGenerator"/> to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public RandomNumberGeneratorWrapper(RandomNumberGenerator rand)
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
        int IRandom.Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            }

            if (maxValue == 0)
            {
                return 0;
            }

            var chop = ulong.MaxValue - (ulong.MaxValue % (ulong)maxValue);

            ulong rand;
            do
            {
                rand = this.NextUlong();
            }
            while (rand >= chop);

            return (int)(rand % (ulong)maxValue);
        }

        /// <summary>
        /// Reads sixty-four bits of data from the wrapped <see cref="RandomNumberGenerator"/> instance, and converts them to a <see cref="ulong"/>.
        /// </summary>
        /// <returns>A random <see cref="ulong"/>.</returns>
        private ulong NextUlong()
        {
            var data = new byte[8];
            this.rand.GetBytes(data);
            return BitConverter.ToUInt64(data, 0);
        }
    }
}
