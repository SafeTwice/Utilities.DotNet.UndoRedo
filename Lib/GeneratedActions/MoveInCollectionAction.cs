//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System;
using Utilities.DotNet.UndoRedo.Generators;

namespace Utilities.DotNet.UndoRedo.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action for moving items in a collection.
    /// </summary>
    internal class MoveInCollectionAction : UndoRedoMergeableAction
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

            m_description = description;

            MaxMergeTimeDiff = maxMergeTimeDiff;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void Do()
        {
            m_collectionManager.MoveItem( m_oldIndex, m_newIndex );
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            m_collectionManager.MoveItem( m_newIndex, m_oldIndex );
        }

        /// <inheritdoc/>
        public override bool TryMerge( IUndoRedoAction newAction )
        {
            if( ( newAction is MoveInCollectionAction newMoveAction ) &&
                ( m_collectionManager == newMoveAction.m_collectionManager ) &&
                ( m_newIndex == newMoveAction.m_oldIndex ) &&
                ( m_oldIndex != newMoveAction.m_newIndex ) && // Avoid merging when creating a no-op action.
                CanMerge( newMoveAction ) )
            {
                m_newIndex = newMoveAction.m_newIndex;
                m_description = newMoveAction.m_description;
                Time = newMoveAction.Time;

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

        private readonly int m_oldIndex;
        private int m_newIndex;

        private string m_description;
    }
}
