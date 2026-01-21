//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System;
using Utilities.DotNet.UndoRedo.Generators;

namespace Utilities.DotNet.UndoRedo.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action for the update of an observable property of a managed object.
    /// </summary>
    internal class UpdateManagedObjectAction : UndoRedoMergeableAction
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <inheritdoc/>
        public override string Description => m_description;

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void Do()
        {
            m_objectManager.SetManagedPropertyValue( m_newValue );
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            m_objectManager.SetManagedPropertyValue( m_oldValue );
        }

        /// <inheritdoc/>
        public override bool TryMerge( IUndoRedoAction newAction )
        {
            if( ( newAction is UpdateManagedObjectAction newUpdateAction ) &&
                ( newUpdateAction.m_objectManager == m_objectManager ) &&
                Equals( newUpdateAction.m_oldValue, m_newValue ) &&
                !Equals( m_oldValue, newUpdateAction.m_newValue ) && // Avoid merging when creating a no-op action.
                CanMerge( newUpdateAction ) )
            {
                m_newValue = newUpdateAction.m_newValue;
                m_description = newUpdateAction.m_description;
                Time = newUpdateAction.Time;

                return true;
            }
            else
            {
                return false;
            }
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
        internal UpdateManagedObjectAction( IUndoRedoObjectManager objectManager, object? oldValue, object? newValue,
                                            string description, TimeSpan maxMergeTimeDiff )
        {
            m_objectManager = objectManager;

            m_oldValue = oldValue;
            m_newValue = newValue;

            m_description = description;

            MaxMergeTimeDiff = maxMergeTimeDiff;
        }

        //===========================================================================
        //                           PROTECTED PROPERTIES
        //===========================================================================

        /// <inheritdoc/>
        protected override TimeSpan MaxMergeTimeDiff { get; }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly IUndoRedoObjectManager m_objectManager;

        private readonly object? m_oldValue;
        private object? m_newValue;

        private string m_description;
    }
}
