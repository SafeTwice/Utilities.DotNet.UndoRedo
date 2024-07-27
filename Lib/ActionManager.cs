/// @file
/// @copyright  Copyright (c) 2022 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System;
using System.ComponentModel;

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

        public bool CanUndo => ( m_undoActions.Count > 0 );

        public bool CanRedo => ( m_redoActions.Count > 0 );

        //===========================================================================
        //                             PUBLIC EVENTS
        //===========================================================================

        public event Action<IActionManager>? UndoRedoStateChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

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
            bool currentCanRedo = CanRedo;

            if( m_undoActions.Last?.Value.TryMerge( action ) != true )
            {
                m_undoActions.AddLast( action );

                if( m_undoActions.Count > m_maxActions )
                {
                    m_undoActions.RemoveFirst();
                }
            }

            m_redoActions.Clear();

            InvokeUpdateEvents( currentCanRedo );
        }

        public void Undo()
        {
            var actionNode = m_undoActions.Last;

            if( actionNode != null )
            {
                actionNode.Value.UnExecute();

                m_undoActions.Remove( actionNode );
                m_redoActions.AddLast( actionNode );

                InvokeUpdateEvents( true );
            }
        }

        public void Redo()
        {
            var actionNode = m_redoActions.Last;

            if( actionNode != null )
            {
                actionNode.Value.Execute();

                m_redoActions.Remove( actionNode );
                m_undoActions.AddLast( actionNode );

                InvokeUpdateEvents( true );
            }
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void InvokeUpdateEvents( bool canRedoUpdated )
        {
            PropertyChanged?.Invoke( this, new( nameof( CanUndo ) ) );
            PropertyChanged?.Invoke( this, new( nameof( UndoActionDescription ) ) );

            if( canRedoUpdated )
            {
                PropertyChanged?.Invoke( this, new( nameof( CanRedo ) ) );
                PropertyChanged?.Invoke( this, new( nameof( RedoActionDescription ) ) );
            }

            UndoRedoStateChanged?.Invoke( this );
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private LinkedList<IAction> m_undoActions = new();
        private LinkedList<IAction> m_redoActions = new();
        private uint m_maxActions;
    }
}