/// @file
/// @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System;
using System.Collections;
using UndoRedoFramework.Generators;

namespace UndoRedoFramework.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action removing items from a collection.
    /// </summary>
    internal class RemoveFromCollectionAction : IUndoRedoAction
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        /// <inheritdoc/>
        public string Description { get; }

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

            Description = description;

            m_maxMergeTimeDiff = maxMergeTimeDiff;
            m_time = DateTime.Now;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public void Do()
        {
            m_collectionManager.RemoveItems( m_removedItems );
        }

        /// <inheritdoc/>
        public void Undo()
        {
            m_collectionManager.InsertItems( m_removedItems, m_deletionIndex );
        }

        /// <inheritdoc/>
        public bool TryMerge( IUndoRedoAction action )
        {
            if( ( action is RemoveFromCollectionAction updateAction ) &&
                ( m_collectionManager == updateAction.m_collectionManager ) &&
                ( ( updateAction.m_time - m_time ) < m_maxMergeTimeDiff ) )
            {
                if( ( ( m_deletionIndex > 0 ) || ( updateAction.m_deletionIndex > 0 ) ) &&
                    ( updateAction.m_deletionIndex != m_deletionIndex ) )
                {
                    return false;
                }

                foreach( object item in updateAction.m_removedItems )
                {
                    m_removedItems.Add( item );
                }

                m_time = updateAction.m_time;
                return true;
            }
            else
            {
                return false;
            }
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly ICollectionManager m_collectionManager;

        private readonly IList m_removedItems;
        private readonly int m_deletionIndex;

        private readonly TimeSpan m_maxMergeTimeDiff;
        private DateTime m_time;
    }
}
