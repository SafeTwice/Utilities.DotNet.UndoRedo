/// @file
/// @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Collections;
using UndoRedoFramework.Generators;

namespace UndoRedoFramework.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action for inserting items into a collection.
    /// </summary>
    internal class InsertIntoCollectionAction : IUndoRedoAction
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
        /// <param name="insertedItems">Items to insert.</param>
        /// <param name="insertionIndex">Index where to insert the items.</param>
        /// <param name="description">Description of the action.</param>
        /// <param name="maxMergeTimeDiff">Maximum time difference to merge with another action.</param>
        public InsertIntoCollectionAction( ICollectionManager collectionManager, IList insertedItems, int insertionIndex,
                                           string description, TimeSpan maxMergeTimeDiff )
        {
            m_collectionManager = collectionManager;

            m_insertedItems = insertedItems;
            m_insertionIndex = insertionIndex;

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
            m_collectionManager.InsertItems( m_insertedItems, m_insertionIndex );
        }

        /// <inheritdoc/>
        public void Undo()
        {
            m_collectionManager.RemoveItems( m_insertedItems );
        }

        /// <inheritdoc/>
        public bool TryMerge( IUndoRedoAction action )
        {
            if( ( action is InsertIntoCollectionAction updateAction ) &&
                ( m_collectionManager == updateAction.m_collectionManager ) &&
                ( ( updateAction.m_time - m_time ) < m_maxMergeTimeDiff ) )
            {
                if( ( ( m_insertionIndex > 0 ) || ( updateAction.m_insertionIndex > 0 ) ) &&
                    ( updateAction.m_insertionIndex != ( m_insertionIndex + m_insertedItems.Count ) ) )
                {
                    return false;
                }

                foreach( object item in updateAction.m_insertedItems )
                {
                    m_insertedItems.Add( item );
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

        private readonly IList m_insertedItems;
        private readonly int m_insertionIndex;

        private readonly TimeSpan m_maxMergeTimeDiff;
        private DateTime m_time;
    }
}
