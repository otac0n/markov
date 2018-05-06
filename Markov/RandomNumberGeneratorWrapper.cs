//-----------------------------------------------------------------------
// <copyright file="RandomNumberGeneratorWrapper.cs" company="(none)">
//  Copyright © 2011 John Gietzen.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace Markov
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Wraps an instance of <see cref="System.Security.Cryptography.RandomNumberGenerator"/> to provide the <see cref="IRandom"/> interface.
    /// </summary>
    public class RandomNumberGeneratorWrapper : IRandom
    {
        /// <summary>
        /// Holds the instance of <see cref="System.Security.Cryptography.RandomNumberGenerator"/> being wrapped.
        /// </summary>
        private readonly RandomNumberGenerator rand;

        /// <summary>
        /// Initializes a new instance of the RandomNumberGeneratorWrapper class, wrapping a given <see cref="System.Security.Cryptography.RandomNumberGenerator"/> instance.
        /// </summary>
        /// <param name="rand">The instance of <see cref="System.Security.Cryptography.RandomNumberGenerator"/> to wrap.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public RandomNumberGeneratorWrapper(RandomNumberGenerator rand)
        {
            if (rand == null)
            {
                throw new ArgumentNullException("rand");
            }

            this.rand = rand;
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; that is, the range of return values ordinarily includes
        /// zero but not maxValue. However, if maxValue equals zero, maxValue is returned.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than zero</exception>
        int IRandom.Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue");
            }

            if (maxValue == 0)
            {
                return 0;
            }

            ulong chop = ulong.MaxValue - (ulong.MaxValue % (ulong)maxValue);

            ulong rand;
            do
            {
                rand = this.NextUlong();
            }
            while (rand >= chop);

            return (int)(rand % (ulong)maxValue);
        }

        /// <summary>
        /// Reads sixty-four bits of data from the wrapped <see cref="System.Security.Cryptography.RandomNumberGenerator"/> instance, and converts them to a <see cref="System.UInt64"/>.
        /// </summary>
        /// <returns>A random <see cref="System.UInt64"/>.</returns>
        private ulong NextUlong()
        {
            byte[] data = new byte[8];
            this.rand.GetBytes(data);
            return BitConverter.ToUInt64(data, 0);
        }
    }
}
