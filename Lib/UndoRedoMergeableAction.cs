//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System;

namespace Utilities.DotNet.UndoRedo
{
    /// <summary>
    /// Represents an action than can be executed (done/redone) and un-executed (undone), and
    /// merged with other actions.
    /// </summary>
    public abstract class UndoRedoMergeableAction : IUndoRedoMergeableAction
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <inheritdoc/>
        public abstract string Description { get; }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public abstract void Do();

        /// <inheritdoc/>
        public abstract void Undo();

        /// <inheritdoc/>
        public abstract bool TryMerge( IUndoRedoAction newAction );

        //===========================================================================
        //                           PROTECTED PROPERTIES
        //===========================================================================

        /// <summary>
        /// Time when the action was performed.
        /// </summary>
        protected DateTime Time { get; set; }

        /// <summary>
        /// Maximum time difference to merge with another action.
        /// </summary>
        /// <remarks>
        /// Derived classes must override this property to provide the maximum time difference.
        /// </remarks>
        protected abstract TimeSpan MaxMergeTimeDiff { get; }

        //===========================================================================
        //                          PROTECTED CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        protected UndoRedoMergeableAction()
        {
            Time = DateTime.UtcNow;
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        /// <summary>
        /// Indicates whether this action can be merged with another action performed at the given time.
        /// </summary>
        /// <param name="newActionTime">Time when the other action was performed.</param>
        /// <returns><see langword="true"/> if the actions can be merged; otherwise, <see langword="false"/>.</returns>
        protected bool CanMerge( DateTime newActionTime )
        {
            return ( newActionTime - Time ) < MaxMergeTimeDiff;
        }

        /// <summary>
        /// Indicates whether this action can be merged with another action.
        /// </summary>
        /// <param name="newAction">Action to be merged.</param>
        /// <returns><see langword="true"/> if the actions can be merged; otherwise, <see langword="false"/>.</returns>
        protected bool CanMerge( UndoRedoMergeableAction newAction )
        {
            return CanMerge( newAction.Time );
        }
    }
}
