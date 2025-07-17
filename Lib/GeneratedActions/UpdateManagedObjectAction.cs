/// @file
/// @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System;
using UndoRedoFramework.Generators;

namespace UndoRedoFramework.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action for the update of an observable property of a managed object.
    /// </summary>
    internal class UpdateManagedObjectAction : IUndoRedoAction
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <inheritdoc/>
        public string Description { get; }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public void Do()
        {
            m_objectManager.SetManagedPropertyValue( m_newValue );
        }

        /// <inheritdoc/>
        public void Undo()
        {
            m_objectManager.SetManagedPropertyValue( m_oldValue );
        }

        /// <inheritdoc/>
        public bool TryMerge( IUndoRedoAction action )
        {
            if( ( action is UpdateManagedObjectAction updateAction ) &&
                ( updateAction.m_objectManager == m_objectManager ) &&
                ( updateAction.m_oldValue == m_newValue ) &&
                ( ( updateAction.m_time - m_time ) < m_maxMergeTimeDiff ) )
            {
                m_newValue = updateAction.m_newValue;
                m_time = updateAction.m_time;
                return true;
            }

            return false;
        }

        //===========================================================================
        //                          INTERNAL CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="objectManager">Manager for an observable object.</param>
        /// <param name="oldValue">Old value of the property being updated.</param>
        /// <param name="newValue">New value of the property being updated.</param>
        /// <param name="description">Description of the action.</param>
        /// <param name="maxMergeTimeDiff">Maximum time difference between this action and the previous one to allow merging.</param>
        internal UpdateManagedObjectAction( UndoRedoObjectManager objectManager, object? oldValue, object? newValue,
                                            string description, TimeSpan maxMergeTimeDiff )
        {
            m_objectManager = objectManager;

            m_oldValue = oldValue;
            m_newValue = newValue;

            Description = description;

            m_maxMergeTimeDiff = maxMergeTimeDiff;
            m_time = DateTime.Now;
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly UndoRedoObjectManager m_objectManager;

        private readonly object? m_oldValue;
        private object? m_newValue;

        private readonly TimeSpan m_maxMergeTimeDiff;

        private DateTime m_time;
    }
}
