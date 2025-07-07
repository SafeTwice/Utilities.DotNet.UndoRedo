/// @file
/// @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;

namespace UndoRedoFramework.Generators
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

        IUndoRedoActionManager? ActionManager { get; }

        /// <summary>
        /// Gets or sets the value of a property of the observed object.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        object? this[ string propertyName ] { get; set; }
    }
}
