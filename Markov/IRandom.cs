// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Markov
{
    /// <summary>
    /// Represents the interface for a device that produces a sequence of numbers that meet certain statistical requirements for randomness.
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; that is, the range of return values ordinarily includes
        /// zero but not maxValue. However, if maxValue equals zero, maxValue is returned.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than zero</exception>
        int Next(int maxValue);
    }
}
