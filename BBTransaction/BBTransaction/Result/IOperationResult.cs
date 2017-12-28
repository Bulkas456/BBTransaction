using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Result
{
    /// <summary>
    /// The operation result.
    /// </summary>
    public interface IOperationResult
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        bool Success
        {
            get;
        }

        /// <summary>
        /// Gets the collection of exceptions for the operation.
        /// </summary>
        IEnumerable<Exception> Errors
        {
            get;
        }
    }
}
