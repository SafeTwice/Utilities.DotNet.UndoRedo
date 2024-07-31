/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;

namespace UndoRedoFramework.Generators
{
    /// <summary>
    /// Represents an object that can be observed for changes.
    /// </summary>
    public interface IObservableObject : INotifyPropertyChanged
    {
        //===========================================================================
        //                                PROPERTIES
        //===========================================================================

        IActionManager? ActionManager { get; }

        /// <summary>
        /// Gets or sets the value of a property of the observed object.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        object? this[ string propertyName ] { get; set; }
    }
}
