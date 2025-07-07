/// @file
/// @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

namespace UndoRedoFramework.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action for moving items in a collection.
    /// </summary>
    public class MoveInCollectionAction : IUndoRedoAction
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
        /// <param name="oldIndex">Index of the item to move.</param>
        /// <param name="newIndex">Index where the item will be moved.</param>
        /// <param name="description">Description of the action.</param>
        /// <param name="maxMergeTimeDiff">Maximum time difference to merge with another action.</param>
        public MoveInCollectionAction( ICollectionManager collectionManager, int oldIndex, int newIndex,
                                       string description, TimeSpan maxMergeTimeDiff )
        {
            m_collectionManager = collectionManager;

            m_oldIndex = oldIndex;
            m_newIndex = newIndex;

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
            m_collectionManager.MoveItem( m_oldIndex, m_newIndex );
        }

        /// <inheritdoc/>
        public void Undo()
        {
            m_collectionManager.MoveItem( m_newIndex, m_oldIndex );
        }

        /// <inheritdoc/>
        public bool TryMerge( IUndoRedoAction action )
        {
            if( ( action is MoveInCollectionAction updateAction ) &&
                ( m_collectionManager == updateAction.m_collectionManager ) &&
                ( ( updateAction.m_time - m_time ) < m_maxMergeTimeDiff ) &&
                ( m_newIndex == updateAction.m_oldIndex ) &&
                ( m_oldIndex != updateAction.m_newIndex ) ) // Avoid having a "do nothing" action
            {
                m_newIndex = updateAction.m_newIndex;

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

        private readonly int m_oldIndex;
        private int m_newIndex;

        private readonly TimeSpan m_maxMergeTimeDiff;
        private DateTime m_time;
    }
}
