//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System;
using System.Collections;
using System.Linq;
using Utilities.DotNet.UndoRedo.Generators;

namespace Utilities.DotNet.UndoRedo.GeneratedActions
{
    /// <summary>
    /// Undo/Redo action for inserting items into a collection.
    /// </summary>
    internal class InsertIntoCollectionAction : UndoRedoMergeableAction
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
        /// <param name="insertedItems">Items to insert.</param>
        /// <param name="insertionIndex">Index where to insert the items.</param>
        /// <param name="description">Description of the action.</param>
        /// <param name="maxMergeTimeDiff">Maximum time difference to merge with another action.</param>
        public InsertIntoCollectionAction( ICollectionManager collectionManager, IList insertedItems, int insertionIndex,
                                           string description, TimeSpan maxMergeTimeDiff )
        {
            m_collectionManager = collectionManager;

            m_insertedItems = insertedItems.Cast<object>().ToList();
            m_insertionIndex = insertionIndex;

            m_description = description;

            MaxMergeTimeDiff = maxMergeTimeDiff;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        /// <inheritdoc/>
        public override void Do()
        {
            m_collectionManager.InsertItems( m_insertedItems, m_insertionIndex );
        }

        /// <inheritdoc/>
        public override void Undo()
        {
            m_collectionManager.RemoveItems( m_insertedItems );
        }

        /// <inheritdoc/>
        public override bool TryMerge( IUndoRedoAction newAction )
        {
            if( ( newAction is InsertIntoCollectionAction newInsertAction ) &&
                ( m_collectionManager == newInsertAction.m_collectionManager ) &&
                ( newInsertAction.m_insertionIndex == ( m_insertionIndex + m_insertedItems.Count ) ) &&
                CanMerge( newInsertAction ) )
            {
                foreach( object item in newInsertAction.m_insertedItems )
                {
                    m_insertedItems.Add( item );
                }

                m_description = newInsertAction.m_description;
                Time = newInsertAction.Time;

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

        private readonly IList m_insertedItems;
        private readonly int m_insertionIndex;

        private string m_description;
    }
}
