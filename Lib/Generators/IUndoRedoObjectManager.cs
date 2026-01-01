//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

namespace Utilities.DotNet.UndoRedo.Generators
{
    /// <summary>
    /// Represents an object that can manage an observed property in a managed object.
    /// </summary>
    internal interface IUndoRedoObjectManager
    {
        //===========================================================================
        //                            INTERNAL METHODS
        //===========================================================================

        /// <summary>
        /// Sets the value of the observed property.
        /// </summary>
        /// <param name="value">The new value of the observed property.</param>
        void SetManagedPropertyValue( object? value );
    }
}
