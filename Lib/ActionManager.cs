/// @file
/// @copyright  Copyright (c) 2022 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System;

namespace UndoRedoFramework
{
    /// <summary>
    /// Manages actions that can be done and undone.
    /// </summary>
    public class ActionManager : IActionManager
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        public string? UndoActionDescription => m_undoActions.Last?.Value.Description;

        public string? RedoActionDescription => m_redoActions.Last?.Value.Description;

        //===========================================================================
        //                             PUBLIC EVENTS
        //===========================================================================

        public event EventHandler? UndoRedoStateChanged;

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// When the number of stored actions to be undone exceeds <paramref name="maxUndoSize"/>, older actions to be
        /// undone are deleted.
        /// </remarks>
        /// <param name="maxUndoSize">Max size of actions to be undone that are stored</param>
        public ActionManager( uint maxUndoSize = uint.MaxValue )
        {
            m_maxActions = maxUndoSize;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        public void Execute( IAction action )
        {
            action.Execute();

            Register( action );
        }

        public void Register( IAction action )
        {
            if( m_undoActions.Last?.Value.TryMerge( action ) != true )
            {
                m_undoActions.AddLast( action );

                if( m_undoActions.Count > m_maxActions )
                {
                    m_undoActions.RemoveFirst();
                }
            }

            m_redoActions.Clear();

            UndoRedoStateChanged?.Invoke( this, new() );
        }

        public void Undo()
        {
            var action = m_undoActions.Last?.Value;

            if( action != null )
            {
                action.UnExecute();

                m_undoActions.RemoveLast();
                m_redoActions.AddLast( action );

                UndoRedoStateChanged?.Invoke( this, new() );
            }
        }

        public bool CanUndo()
        {
            return m_undoActions.Count > 0;
        }

        public void Redo()
        {
            var action = m_redoActions.Last?.Value;

            if( action != null )
            {
                action.Execute();

                m_redoActions.RemoveLast();
                m_undoActions.AddLast( action );

                UndoRedoStateChanged?.Invoke( this, new() );
            }
        }

        public bool CanRedo()
        {
            return m_redoActions.Count > 0;
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private LinkedList<IAction> m_undoActions = new();
        private LinkedList<IAction> m_redoActions = new();
        private uint m_maxActions;
    }
}