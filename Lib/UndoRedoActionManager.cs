/// @file
/// @copyright  Copyright (c) 2022-2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Utilities.DotNet;

namespace UndoRedoFramework
{
    /// <summary>
    /// Manages actions that can be done and undone.
    /// </summary>
    public sealed class UndoRedoActionManager : IUndoRedoActionManager
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <inheritdoc/>
        public string? UndoActionDescription => m_undoActions.Last?.Value.Description;

        /// <inheritdoc/>
        public string? RedoActionDescription => m_redoActions.Last?.Value.Description;

        /// <inheritdoc/>
        public bool CanUndo => ( m_undoActions.Count > 0 );

        /// <inheritdoc/>
        public bool CanRedo => ( m_redoActions.Count > 0 );

        /// <inheritdoc/>
        public IUndoRedoAction? UndoAction
        {
            get
            {
                var actionNode = m_undoActions.Last;
                return actionNode?.Value;
            }
        }

        //===========================================================================
        //                             PUBLIC EVENTS
        //===========================================================================

        /// <inheritdoc/>
        public event Action<IUndoRedoActionManager>? UndoRedoStateChanged;

        /// <inheritdoc/>
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
        /// <param name="maxUndoSize">Max size of actions to be undone that are stored.</param>
        public UndoRedoActionManager( uint maxUndoSize = uint.MaxValue )
        {
            m_maxActions = maxUndoSize;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public void RegisterAndDo( IUndoRedoAction action )
        {
            action.Do();

            Register( action );
        }

        /// <inheritdoc/>
        public void Register( IUndoRedoAction action )
        {
            bool currentCanRedo = CanRedo;

            if( m_undoActions.Last?.Value.TryMerge( action ) != true )
            {
                m_undoActions.AddLast( action );

                if( m_undoActions.Count > m_maxActions )
                {
                    var firstAction = m_undoActions.First?.Value as IReleaseNotifiedUndoRedoAction;
                    firstAction?.OnReleased( true );

                    m_undoActions.RemoveFirst();
                }
            }

            ClearRedoActions();

            InvokeUpdateEvents( currentCanRedo );
        }

        /// <inheritdoc/>
        public void Undo()
        {
            var actionNode = m_undoActions.Last;

            if( actionNode != null )
            {
                actionNode.Value.Undo();

                m_undoActions.Remove( actionNode );
                m_redoActions.AddLast( actionNode );

                InvokeUpdateEvents( true );
            }
        }

        /// <inheritdoc/>
        public void Redo()
        {
            var actionNode = m_redoActions.Last;

            if( actionNode != null )
            {
                actionNode.Value.Do();

                m_redoActions.Remove( actionNode );
                m_undoActions.AddLast( actionNode );

                InvokeUpdateEvents( true );
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            CleanUndoActions();
            ClearRedoActions();
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void CleanUndoActions()
        {
            m_undoActions.OfType<IReleaseNotifiedUndoRedoAction>().ForEach( action => action.OnReleased( true ) );
            m_undoActions.Clear();
        }

        private void ClearRedoActions()
        {
            m_redoActions.OfType<IReleaseNotifiedUndoRedoAction>().ForEach( action => action.OnReleased( false ) );
            m_redoActions.Clear();
        }

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

        private readonly LinkedList<IUndoRedoAction> m_undoActions = new();
        private readonly LinkedList<IUndoRedoAction> m_redoActions = new();
        private readonly uint m_maxActions;
    }
}