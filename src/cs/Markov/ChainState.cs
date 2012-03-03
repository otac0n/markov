//-----------------------------------------------------------------------
// <copyright file="ChainState.cs" company="(none)">
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
    using System.Collections.Generic;
    using System.Linq;

    public class ChainState<T> : IEquatable<ChainState<T>>
    {
        private readonly T[] items;

        public ChainState(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.items = items.ToArray();
        }

        public ChainState(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.items = new T[items.Length];
            Array.Copy(items, this.items, items.Length);
        }

        public static bool operator ==(ChainState<T> a, ChainState<T> b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(ChainState<T> a, ChainState<T> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            var code = this.items.Length.GetHashCode();

            for (int i = 0; i < this.items.Length; i++)
            {
                code ^= this.items[i].GetHashCode();
            }

            return code;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return this.Equals(obj as ChainState<T>);
        }

        public bool Equals(ChainState<T> other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (this.items.Length != other.items.Length)
            {
                return false;
            }

            for (int i = 0; i < this.items.Length; i++)
            {
                if (!this.items[i].Equals(other.items[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
