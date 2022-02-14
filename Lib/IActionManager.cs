/// @file
/// @copyright  Copyright (c) 2022 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt


namespace UndoRedoFramework
{
    public interface IActionManager
    {
        //===========================================================================
        //                                PROPERTIES
        //===========================================================================

        string? RedoActionDescription { get; }
        string? UndoActionDescription { get; }

        //===========================================================================
        //                                  EVENTS
        //===========================================================================

        event EventHandler? UndoRedoStateChanged;

        //===========================================================================
        //                                  METHODS
        //===========================================================================

        void Execute( IAction action );
        void Register( IAction action );

        bool CanUndo();
        void Undo();

        bool CanRedo();
        void Redo();
    }
}