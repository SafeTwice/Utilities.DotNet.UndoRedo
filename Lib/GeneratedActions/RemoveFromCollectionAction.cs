//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System;
using System.Collections;
using Utilities.DotNet.UndoRedo.Generators;

namespace Utilities.DotNet.UndoRedo.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action removing items from a collection.
    /// </summary>
    internal class RemoveFromCollectionAction : UndoRedoMergeableAction
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
        /// <param name="removedItems">Items to remove.</param>
        /// <param name="deletionIndex">Index where the items are removed.</param>
        /// <param name="description">Description of the action.</param>
        /// <param name="maxMergeTimeDiff">Maximum time difference to merge with another action.</param>
        public RemoveFromCollectionAction( ICollectionManager collectionManager, IList removedItems, int deletionIndex,
                                           string description, TimeSpan maxMergeTimeDiff )
        {
            m_collectionManager = collectionManager;

            m_removedItems = removedItems;
            m_deletionIndex = deletionIndex;

            m_description = description;

            MaxMergeTimeDiff = maxMergeTimeDiff;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void Do()
        {
            m_collectionManager.RemoveItems( m_removedItems );
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            m_collectionManager.InsertItems( m_removedItems, m_deletionIndex );
        }

        /// <inheritdoc/>
        public override bool TryMerge( IUndoRedoAction newAction )
        {
            if( ( newAction is RemoveFromCollectionAction newRemoveAction ) &&
                ( m_collectionManager == newRemoveAction.m_collectionManager ) &&
                ( newRemoveAction.m_deletionIndex == m_deletionIndex ) &&
                CanMerge( newRemoveAction ) )
            {
                foreach( object item in newRemoveAction.m_removedItems )
                {
                    m_removedItems.Add( item );
                }

                m_description = newRemoveAction.m_description;
                Time = newRemoveAction.Time;

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

        private readonly IList m_removedItems;
        private readonly int m_deletionIndex;

        private string m_description;
    }
}
