using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.StepEnumerator.StepMove
{
    /// <summary>
    /// The move info.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    public interface IMoveInfo<TStepId>
    {
        /// <summary>
        /// The step id to which the transaciton should move.
        /// </summary>
        TStepId Id
        {
            get;
        }

        /// <summary>
        /// The comaprer for the step.
        /// </summary>
        IEqualityComparer<TStepId> Comparer
        {
            get;
        }

        /// <summary>
        /// The move type.
        /// </summary>
        MoveType MoveType
        {
            get;
        }
    }
}
