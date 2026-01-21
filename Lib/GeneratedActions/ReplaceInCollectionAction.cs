//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System;
using Utilities.DotNet.UndoRedo.Generators;

namespace Utilities.DotNet.UndoRedo.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action for replacing items in a collection.
    /// </summary>
    internal class ReplaceInCollectionAction : UndoRedoMergeableAction
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <inheritdoc/>
        public override string Description => m_description;

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="collectionManager">Manager for the collection.</param>
        /// <param name="oldItem">Item to replace.</param>
        /// <param name="newItem">Item to replace with.</param>
        /// <param name="description">Description of the action.</param>
        /// <param name="maxMergeTimeDiff">Maximum time difference to merge with another action.</param>
        public ReplaceInCollectionAction( ICollectionManager collectionManager, object oldItem, object newItem,
                                          string description, TimeSpan maxMergeTimeDiff )
        {
            m_collectionManager = collectionManager;

            m_oldItem = oldItem;
            m_newItem = newItem;

            m_description = description;

            MaxMergeTimeDiff = maxMergeTimeDiff;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void Do()
        {
            m_collectionManager.ReplaceItem( m_oldItem, m_newItem );
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            m_collectionManager.ReplaceItem( m_newItem, m_oldItem );
        }

        /// <inheritdoc/>
        public override bool TryMerge( IUndoRedoAction newAction )
        {
            if( ( newAction is ReplaceInCollectionAction newReplaceAction ) &&
                ( m_collectionManager == newReplaceAction.m_collectionManager ) &&
                Equals( m_newItem, newReplaceAction.m_oldItem ) &&
                !Equals( m_oldItem, newReplaceAction.m_newItem ) && // Avoid merging when creating a no-op action.
                CanMerge( newReplaceAction ) )
            {
                m_newItem = newReplaceAction.m_newItem;
                m_description = newReplaceAction.m_description;
                Time = newReplaceAction.Time;

                return true;
            }
            else
            {
                return false;
            }
        }

        //===========================================================================
        //                           PROTECTED PROPERTIES
        //===========================================================================

        /// <inheritdoc/>
        protected override TimeSpan MaxMergeTimeDiff { get; }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly ICollectionManager m_collectionManager;

        private readonly object m_oldItem;
        private object m_newItem;

        private string m_description;
    }
}
