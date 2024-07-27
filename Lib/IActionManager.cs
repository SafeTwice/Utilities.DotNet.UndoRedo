/// @file
/// @copyright  Copyright (c) 2022 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;

namespace UndoRedoFramework
{
    /// <summary>
    /// Interface for an undo/redo framework that can store actions available for being
    /// undone and redone, and manages their execution and un-execution.
    /// </summary>
    public interface IActionManager : INotifyPropertyChanged
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

        /// <summary>
        /// Indicates if an action is available for being undone (i.e., un-executed).
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Indicates if a action is available for being redone (i.e., re-executed).
        /// </summary>
        bool CanRedo { get; }

        //===========================================================================
        //                                  EVENTS
        //===========================================================================

        /// <summary>
        /// Invoked when the state of available undo/redo actions changes.
        /// </summary>
        event Action<IActionManager>? UndoRedoStateChanged;

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
        /// This method is mainly intended for adding actions that represent undo/redo actions that
        /// have already been executed (e.g., observed).
        /// </remarks>
        /// <param name="action">Action to be stored</param>
        void Register( IAction action );

        /// <summary>
        /// Un-executes the next action available for being undone, and makes it available for being redone.
        /// </summary>
        void Undo();

        /// <summary>
        /// Re-executes the next action available for being redone, and makes it available for being undone.
        /// </summary>
        void Redo();
    }
}