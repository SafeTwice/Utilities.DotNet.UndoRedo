/// @file
/// @copyright  Copyright (c) 2022 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System;

namespace UndoRedoFramework
{
    /// <summary>
    /// Represents an action than can be executed (done/redone) and unexecuted (undone).
    /// </summary>
    public interface IAction
    {
        //===========================================================================
        //                              PROPERTIES
        //===========================================================================

        string Description { get; }

        //===========================================================================
        //                               METHODS
        //===========================================================================

        void Execute();

        void UnExecute();

        bool TryMerge( IAction action );
    }
}
