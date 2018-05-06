//-----------------------------------------------------------------------
// <copyright file="RandomWrapper.cs" company="(none)">
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
        /// Initializes a new instance of the RandomWrapper class, wrapping a given <see cref="System.Random"/> instance.
        /// </summary>
        /// <param name="rand">The instance of <see cref="System.Random"/> to wrap.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="rand"/> is null.</exception>
        public RandomWrapper(Random rand)
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
            return this.rand.Next(maxValue);
        }
    }
}
