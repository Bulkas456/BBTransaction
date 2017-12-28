using System;
using System.Collections.Generic;
using System.Text;
using BBTransaction.Info;
using BBTransaction.Logger;

namespace BBTransaction.Factory.Context.Part
{
    /// <summary>
    /// The create part of transaction core context.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    /// <typeparam name="TData">The type of the transaciton data.</typeparam>
    internal struct CreatePartContext<TStepId, TData> : ICreatePartContext<TStepId, TData>
    {
        /// <summary>
        /// Gets the create transaction context.
        /// </summary>
        public ICreateTransactionContext<TStepId, TData> Context
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the transaction info.
        /// </summary>
        public ITransactionCreateInfo Info
        {
            get;
            set;
        }
    }
}
