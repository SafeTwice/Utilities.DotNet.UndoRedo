//! @file
//! @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System.ComponentModel;

namespace Utilities.DotNet.UndoRedo.Generators
{
    /// <summary>
    /// Represents an object which properties can be observed for changes and that can be 
    /// manipulated to undo and redo those changes.
    /// </summary>
    public interface IUndoRedoManagedObject : INotifyPropertyChanged
    {
        //===========================================================================
        //                                PROPERTIES
        //===========================================================================

        /// <summary>
        /// Action manager that manages undo/redo actions for this object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property should return a non-<c>null</c> value while the object is active
        /// (i.e., while it is being used and its properties are observed).
        /// </para>
        /// <para>
        /// However, when the object is inactive (e.g., if it has been "deleted" and it
        /// exists only in the undo/redo stack), this property may return <c>null</c>.
        /// </para>
        /// <para>
        /// Property changes while this property is <c>null</c> are ignored.
        /// </para>
        /// </remarks>
        IUndoRedoActionManager? ActionManager { get; }

        /// <summary>
        /// Gets or sets the value of a property of the observed object.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        object? this[ string propertyName ] { get; set; }
    }
}
