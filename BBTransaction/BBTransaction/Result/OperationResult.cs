using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Result
{
    /// <summary>
    /// The operation result.
    /// </summary>
    public class OperationResult : IOperationResult
    {
        /// <summary>
        /// The collecion of errors for the operation result.
        /// </summary>
        private readonly List<Exception> errors = new List<Exception>();

        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success
        {
            get
            {
                return this.errors.Count == 0;
            }
        }

        /// <summary>
        /// Gets the collection of exceptions for the operation.
        /// </summary>
        public IEnumerable<Exception> Errors
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Adds an error to the operation result.
        /// </summary>
        /// <param name="error">The error to add.</param>
        /// <returns>The operation result.</returns>
        public OperationResult Add(Exception error)
        {
            if (error != null)
            {
                this.errors.Add(error);
            }

            return this;
        }

        /// <summary>
        /// Merges the instance of the operation result with the other instance of an operation result.
        /// </summary>
        /// <param name="result">The operation result to merge.</param>
        /// <returns>The instance of the operation result.</returns>
        public OperationResult Add(IOperationResult result)
        {
            if (result != null
                && result.Errors != null)
            {
                this.errors.AddRange(result.Errors);
            }

            return this;
        }
    }
}
