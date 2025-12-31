//! @file
//! @copyright  Copyright (c) 2022-2025 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

namespace Utilities.DotNet.UndoRedo
{
    /// <summary>
    /// Represents an action than can be executed (done/redone) and un-executed (undone).
    /// </summary>
    public interface IUndoRedoAction
    {
        //===========================================================================
        //                              PROPERTIES
        //===========================================================================

        /// <summary>
        /// Description of the action.
        /// </summary>
        string Description { get; }

        //===========================================================================
        //                               METHODS
        //===========================================================================

        /// <summary>
        /// (Re)Executes an action.
        /// </summary>
        void Do();

        /// <summary>
        /// Un-executes an action (undoes the outcome of its execution).
        /// </summary>
        void Undo();

        /// <summary>
        /// Tries to merge <paramref name="action"/> with this action if it is possible.
        /// </summary>
        /// <remarks>
        /// This method is called on the last action available for being undone when a new action
        /// is added to the undo/redo stack.
        /// </remarks>
        /// <param name="action">Action to be merged.</param>
        /// <returns><c>true</c> if the action was merged; <c>false</c> otherwise.</returns>
        bool TryMerge( IUndoRedoAction action );
    }
}
