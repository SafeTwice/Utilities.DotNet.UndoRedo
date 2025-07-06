/// @file
/// @copyright  Copyright (c) 2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using Utilities.DotNet;

namespace UndoRedoFramework.GenericActions
{
#pragma warning disable S1694 // This class is not an interface because interface methods cannot be internal.

    /// <summary>
    /// Represents an object that can manage an observed property in an observable object.
    /// </summary>
    public abstract class ObservableObjectManager : DisposableObject
    {
        //===========================================================================
        //                            INTERNAL METHODS
        //===========================================================================

        /// <summary>
        /// Sets the value of the observed property.
        /// </summary>
        /// <param name="value">The new value of the observed property.</param>
        internal abstract void SetObservedPropertyValue( object? value );
    }

#pragma warning restore S1694
}
