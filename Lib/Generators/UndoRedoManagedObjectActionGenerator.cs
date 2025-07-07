/// @file
/// @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;
using UndoRedoFramework.GeneratedActions;

namespace UndoRedoFramework.Generators
{
    /// <summary>
    /// Generates Undo/Redo actions for an observed property in a managed object.
    /// </summary>
    /// <remarks>
    /// When the observed property changes, the generator creates the corresponding undo/redo action.
    /// </remarks>
    public class UndoRedoManagedObjectActionGenerator : UndoRedoObjectManager
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
        public UndoRedoManagedObjectActionGenerator( IUndoRedoManagedObject managedObject, string propertyName,
                                                Func<(object? oldValue, object? newValue), string> descriptionGenerator,
                                                TimeSpan? maxMergeTimeDiff = null )
        {
            m_managedObject = managedObject;
            m_propertyName = propertyName;

            m_descriptionGenerator = descriptionGenerator;

            m_maxMergeTimeDiff = maxMergeTimeDiff ?? DEFAULT_MAX_MERGE_TIME_DIFF;

            m_currentValue = m_managedObject[ propertyName ];

            m_managedObject.PropertyChanged += OnPropertyChanged;
        }

        //===========================================================================
        //                            INTERNAL METHODS
        //===========================================================================

        internal override void SetManagedPropertyValue( object? value )
        {
            m_ignoreEvents = true;

            m_managedObject[ m_propertyName ] = value;

            m_ignoreEvents = false;
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        protected override void Dispose( bool disposing )
        {
            m_managedObject.PropertyChanged -= OnPropertyChanged;

            base.Dispose( disposing );
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OnPropertyChanged( object? sender, PropertyChangedEventArgs e )
        {
            if( e.PropertyName == m_propertyName )
            {
                var oldValue = m_currentValue;
                var newValue = m_managedObject[ m_propertyName ];

                var actionManager = m_managedObject.ActionManager;

                if( !m_ignoreEvents && ( actionManager != null ) && !Equals( oldValue, newValue ) )
                {
                    var actionDescription = m_descriptionGenerator( (oldValue, newValue) );

                    actionManager.Register( new UpdateObservableObjectAction( this, oldValue, newValue, actionDescription,
                                                                              m_maxMergeTimeDiff ) );
                }

                m_currentValue = newValue;
            }
        }

        //===========================================================================
        //                           PRIVATE CONSTANTS
        //===========================================================================

        private static readonly TimeSpan DEFAULT_MAX_MERGE_TIME_DIFF = TimeSpan.FromSeconds( 1 );

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly IUndoRedoManagedObject m_managedObject;
        private readonly string m_propertyName;

        private readonly Func<(object?, object?), string> m_descriptionGenerator;

        private readonly TimeSpan m_maxMergeTimeDiff;

        private object? m_currentValue;

        private bool m_ignoreEvents;
    }
}
