//! @file
//! @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

namespace Utilities.DotNet.UndoRedo.Generators
{
#pragma warning disable S1694 // This class is not an interface because interface methods cannot be internal.

    /// <summary>
    /// Represents an object that can manage an observed property in a managed object.
    /// </summary>
    public abstract class UndoRedoObjectManager : DisposableObject
    {
        //===========================================================================
        //                            INTERNAL METHODS
        //===========================================================================

        /// <summary>
        /// Sets the value of the observed property.
        /// </summary>
        /// <param name="value">The new value of the observed property.</param>
        internal abstract void SetManagedPropertyValue( object? value );
    }

#pragma warning restore S1694
}
