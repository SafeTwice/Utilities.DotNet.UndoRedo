//! @file
//! @copyright  Copyright (c) 2022-2025 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

namespace Utilities.DotNet.UndoRedo
{
    /// <summary>
    /// Represents an <see cref="IUndoRedoAction"/> than is notified when it is released from the undo/redo stack.
    /// </summary>
    public interface IReleaseNotifiedUndoRedoAction : IUndoRedoAction
    {
        //===========================================================================
        //                               METHODS
        //===========================================================================

        /// <summary>
        /// Called when the action is released from the undo/redo stack.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called with <paramref name="done"/> set to <see langword="true"/> when the action is removed from the
        /// undo stack because the undo stack is full and the action is the oldest one in the stack.
        /// </para>
        /// <para>
        /// This method is called with <paramref name="done"/> set to <see langword="false"/> when the action is removed from the
        /// redo stack because the redo stack is cleared.
        /// </para>
        /// <para>
        /// Implementations of this method should release (dispose) any resources that are no longer needed.
        /// </para>
        /// <para>
        /// For example, an action that adds a disposable object to a list should dispose the object when the action is released undone,
        /// however it should not dispose it if the action is released done (because the object is active and owned by the list).
        /// On the other hand, an action that removes a disposable object from a list should dispose the object when the action is
        /// released done (because the object is no longer active), but should not dispose it if the action is released undone.
        /// </para>
        /// </remarks>
        /// <param name="done">Indicates if the action was done (<see langword="true"/>) or undone (<see langword="false"/>).</param>
        void OnReleased( bool done );
    }
}
