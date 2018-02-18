using System;
using System.Collections.Generic;
using System.Text;

namespace BBTransaction.Transaction.Session.StepEnumerator.StepMove
{
    /// <summary>
    /// The move info.
    /// </summary>
    /// <typeparam name="TStepId">The type of the step id.</typeparam>
    internal class MoveInfo<TStepId> : IMoveInfo<TStepId>
    {
        /// <summary>
        /// The step id to which the transaciton should move.
        /// </summary>
        public TStepId Id
        {
            get;
            set;
        }

        /// <summary>
        /// The comaprer for the step.
        /// </summary>
        public IEqualityComparer<TStepId> Comparer
        {
            get;
            set;
        }

        /// <summary>
        /// The move type.
        /// </summary>
        public MoveType MoveType
        {
            get;
            set;
        }
    }
}
