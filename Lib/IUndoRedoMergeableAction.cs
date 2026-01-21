//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

namespace Utilities.DotNet.UndoRedo
{
    /// <summary>
    /// Represents an action than can be executed (done/redone) and un-executed (undone), and
    /// merged with other actions.
    /// </summary>
    public interface IUndoRedoMergeableAction : IUndoRedoAction
    {
        //===========================================================================
        //                               METHODS
        //===========================================================================

        /// <summary>
        /// Tries to merge <paramref name="newAction"/> with this action if it is possible.
        /// </summary>
        /// <remarks>
        /// This method is called on the last action available for being undone when a new action
        /// is added to the undo/redo stack.
        /// </remarks>
        /// <param name="newAction">New action to be merged.</param>
        /// <returns><see langword="true"/> if the action was merged; <see langword="false"/> otherwise.</returns>
        bool TryMerge( IUndoRedoAction newAction );
    }
}
