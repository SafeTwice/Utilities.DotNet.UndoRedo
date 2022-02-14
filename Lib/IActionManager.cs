/// @file
/// @copyright  Copyright (c) 2022 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt


namespace UndoRedoFramework
{
    /// <summary>
    /// Interface for an undo/redo framework that can store actions available for being
    /// undone and redone, and manages their execution and un-execution.
    /// </summary>
    public interface IActionManager
    {
        //===========================================================================
        //                                PROPERTIES
        //===========================================================================

        /// <summary>
        /// Description of the action available for being undone (<c>null</c> if none available).
        /// </summary>
        string? UndoActionDescription { get; }

        /// <summary>
        /// Description of the action available for being redone (<c>null</c> if none available).
        /// </summary>
        string? RedoActionDescription { get; }

        //===========================================================================
        //                                  EVENTS
        //===========================================================================

        /// <summary>
        /// Invoked when the state of available undo/redo actions changes.
        /// </summary>
        event EventHandler? UndoRedoStateChanged;

        //===========================================================================
        //                                  METHODS
        //===========================================================================

        /// <summary>
        /// Executes an action and stores is as available for being undone.
        /// </summary>
        /// <param name="action">Action to be executed and stored</param>
        void Execute( IAction action );

        /// <summary>
        /// Stores an action as available for being undone (without executing it).
        /// </summary>
        /// <remarks>
        /// This method is mainly intended for adding actions that represent undo/redo actions managed
        /// by other undo/redo frameworks.
        /// </remarks>
        /// <param name="action">Action to be stored</param>
        void Register( IAction action );

        /// <summary>
        /// Indicates if an action is available for being undone (i.e., un-executed).
        /// </summary>
        /// <returns><c>true</c> if an undo action is available, <c>false</c> otherwise</returns>
        bool CanUndo();

        /// <summary>
        /// Un-executes the next action available for being undone, and makes it available for being redone.
        /// </summary>
        void Undo();

        /// <summary>
        /// Indicates if a action is available for being redone (i.e., re-executed).
        /// </summary>
        /// <returns><c>true</c> if a redo action is available, <c>false</c> otherwise</returns>
        bool CanRedo();

        /// <summary>
        /// Re-executes the next action available for being redone, and makes it available for being undone.
        /// </summary>
        void Redo();
    }
}