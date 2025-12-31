//! @file
//! @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System.Collections;

namespace Utilities.DotNet.UndoRedo.Generators
{
    /// <summary>
    /// Represents an object that can manage a collection.
    /// </summary>
    /// <remarks>
    /// This interface is used internally by the library.
    /// </remarks>
    public interface ICollectionManager
    {
        //===========================================================================
        //                                  METHODS
        //===========================================================================

        /// <summary>
        /// Inserts a list of items to the collection at the specified index.
        /// </summary>
        /// <param name="items">Items to insert.</param>
        /// <param name="insertionIndex">Index where the items will be inserted.</param>
        void InsertItems( IList items, int insertionIndex );

        /// <summary>
        /// Removes a list of items from the collection.
        /// </summary>
        /// <param name="items">Items to remove.</param>
        void RemoveItems( IList items );

        /// <summary>
        /// Replaces an items in the collection with another item.
        /// </summary>
        /// <param name="oldItem">Item to be replaced.</param>
        /// <param name="newItem">Item to replace with.</param>
        void ReplaceItem( object oldItem, object newItem );

        /// <summary>
        /// Moves an item in the collection from one index to another.
        /// </summary>
        /// <param name="oldIndex">Index of the item to move.</param>
        /// <param name="newIndex">Index where the item will be moved.</param>
        void MoveItem( int oldIndex, int newIndex );
    }
}
