//! @file
//! @copyright  Copyright (c) 2026 SafeTwice S.L. All rights reserved.
//! @license    See LICENSE.txt

using System;

namespace Utilities.DotNet.UndoRedo.Generators
{
    /// <summary>
    /// Generates Undo/Redo actions for an observed property in a managed object.
    /// </summary>
    /// <remarks>
    /// When the observed property changes, the generator creates the corresponding undo/redo action.
    /// </remarks>
    public class ManagedObjectActionGenerator : DisposableObject
    {
        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// The <paramref name="descriptionGenerator"/> function is invoked when the observed property changes
        /// to generate the description of the new undo/redo action. It receives as parameter a tuple with the old 
        /// and new values of the property.
        /// </remarks>
        /// <param name="managedObject">Managed object to observe for changes.</param>
        /// <param name="propertyName">Name of the property in the managed object to observe for changes.</param>
        /// <param name="descriptionGenerator">Function that generates the description of the undo/redo action.</param>
        /// <param name="maxMergeTimeDiff">Maximum time between consecutively generated actions to automatically merge them.</param>
        public ManagedObjectActionGenerator( IUndoRedoManagedObject managedObject, string propertyName,
                                             Func<(object? oldValue, object? newValue), string> descriptionGenerator,
                                             TimeSpan? maxMergeTimeDiff = null )
        {
            m_objectManager = new UndoRedoObjectManager( managedObject, propertyName, descriptionGenerator, maxMergeTimeDiff );
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            m_objectManager.Dispose();

            base.Dispose( disposing );
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly UndoRedoObjectManager m_objectManager;
    }
}
