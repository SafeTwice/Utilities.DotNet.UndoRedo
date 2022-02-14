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

        /// <summary>
        /// Executes an action and stores it in the undo queue.
        /// </summary>
        /// <param name="action">Action to be executed</param>
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

        /// <summary>
        /// Unexecutes the newest action in the undo queue and moves it to the redo queue.
        /// </summary>
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

        /// <summary>
        /// Indicates if an action is available to be undone.
        /// </summary>
        /// <returns><c>true</c> if an action to be undone is available, <c>false</c> otherwise</returns>
        public bool CanUndo()
        {
            return m_undoActions.Count > 0;
        }

        /// <summary>
        /// Re-executes the oldest action in the redo queue and moves it to the undo queue.
        /// </summary>
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

        /// <summary>
        /// Indicates if an action is available to be redone.
        /// </summary>
        /// <returns><c>true</c> if an action to be redone is available, <c>false</c> otherwise</returns>
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