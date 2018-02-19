using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Step.Adapter
{
    /// <summary>
    /// The adapter for an equality comparer.
    /// </summary>
    /// <typeparam name="TFrom">The source type.</typeparam>
    /// <typeparam name="TTo">The destination type.</typeparam>
    public class EqualityComparerAdapter<TFrom, TTo> : IEqualityComparer<TTo>
    {
        /// <summary>
        /// The original comparer.
        /// </summary>
        private readonly IEqualityComparer<TFrom> original;

        /// <summary>
        /// The converter.
        /// </summary>
        private readonly Func<TTo, TFrom> converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityComparerAdapter<TFrom, TTo>"/> class.
        /// </summary>
        /// <param name="original">The original comparer.</param>
        /// <param name="converter">The converter.</param>
        public EqualityComparerAdapter(IEqualityComparer<TFrom> original, Func<TTo, TFrom> converter)
        {
            this.original = original ?? throw new ArgumentNullException(nameof(original));
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>True</c> if the specified objects are equal, otherwise <c>false</c>.</returns>
        public bool Equals(TTo x, TTo y)
        {
            return this.original.Equals(this.converter(x), this.converter(y));
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The System.Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(TTo obj)
        {
            return this.original.GetHashCode(this.converter(obj));
        }
    }
}