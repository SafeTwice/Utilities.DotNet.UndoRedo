/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

namespace UndoRedoFramework.Generic
{
    /// <summary>
    /// Represents an object that can manage an observed property in an observable object.
    /// </summary>
    public interface IObservableObjectManager
    {
        //===========================================================================
        //                                  METHODS
        //===========================================================================

        /// <summary>
        /// Sets the value of the observed property.
        /// </summary>
        /// <param name="value">The new value of the observed property.</param>
        void SetObservedPropertyValue( object? value );
    }
}
