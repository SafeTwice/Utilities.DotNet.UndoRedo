/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

namespace UndoRedoFramework.Generic
{
    /// <summary>
    /// Undo/Redo action for replacing items in a collection.
    /// </summary>
    public class ReplaceInCollectionAction : IAction
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

            Description = description;

            m_maxMergeTimeDiff = maxMergeTimeDiff;
            m_time = DateTime.Now;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public void Execute()
        {
            m_collectionManager.ReplaceItem( m_oldItem, m_newItem );
        }

        /// <inheritdoc/>
        public void UnExecute()
        {
            m_collectionManager.ReplaceItem( m_newItem, m_oldItem );
        }

        /// <inheritdoc/>
        public bool TryMerge(IAction action)
        {
            if( ( action is ReplaceInCollectionAction updateAction ) &&
                ( m_collectionManager == updateAction.m_collectionManager ) &&
                ( ( updateAction.m_time - m_time ) < m_maxMergeTimeDiff ) &&
                ( m_newItem == updateAction.m_oldItem ) )
            {
                m_newItem = updateAction.m_newItem;

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

        private readonly object m_oldItem;
        private object m_newItem;

        private readonly TimeSpan m_maxMergeTimeDiff;
        private DateTime m_time;
    }
}
