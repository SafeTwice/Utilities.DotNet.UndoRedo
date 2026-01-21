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
    }
}
