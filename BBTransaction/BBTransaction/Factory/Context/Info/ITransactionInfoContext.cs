﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Factory.Context.Info
{
    /// <summary>
    /// The context for transaction info.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface ITransactionInfoContext<TStepId>
    {
        /// <summary>
        /// Gets or sets the name of the transaction (required).
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unciton which return the current time (optional).
        /// </summary>
        Func<DateTime> GetCurrentTimeFunction
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the session id creator (optional).
        /// </summary>
        Func<Guid> SessionIdCreator
        {
            get;
            set;
        }
    }
}
