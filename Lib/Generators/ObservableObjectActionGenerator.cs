/// @file
/// @copyright  Copyright (c) 2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.ComponentModel;
using UndoRedoFramework.GenericActions;

namespace UndoRedoFramework.Generators
{
    /// <summary>
    /// Generates Undo/Redo actions for an observed property in an observable object.
    /// </summary>
    /// <remarks>
    /// When the observed property changes, the generator creates the corresponding action:
    /// </remarks>
    public class ObservableObjectActionGenerator : ObservableObjectManager
    {
        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// The <paramref name="descriptionGenerator"/> function is invoked when the observed property changes
        /// to generate the description of the new Undo/Redo action. It receives as parameter a tuple with the old 
        /// and new values of the property.
        /// </remarks>
        /// <param name="observableObject">Object to observe for changes.</param>
        /// <param name="propertyName">Property in the object to observe for changes.</param>
        /// <param name="descriptionGenerator">Function that generates the description of the Undo/Redo action.</param>
        /// <param name="maxMergeTimeDiff">Maximum time between consecutively generated actions to automatically merge them.</param>
        public ObservableObjectActionGenerator( IObservableObject observableObject, string propertyName,
                                                Func<(object? oldValue, object? newValue), string> descriptionGenerator,
                                                TimeSpan? maxMergeTimeDiff = null )
        {
            m_observableObject = observableObject;
            m_propertyName = propertyName;

            m_descriptionGenerator = descriptionGenerator;

            m_maxMergeTimeDiff = maxMergeTimeDiff ?? DEFAULT_MAX_MERGE_TIME_DIFF;

            m_currentValue = m_observableObject[ propertyName ];

            m_observableObject.PropertyChanged += OnPropertyChanged;
        }

        //===========================================================================
        //                            INTERNAL METHODS
        //===========================================================================

        internal override void SetObservedPropertyValue( object? value )
        {
            m_ignoreEvents = true;

            m_observableObject[ m_propertyName ] = value;

            m_ignoreEvents = false;
        }

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        protected override void Dispose( bool disposing )
        {
            m_observableObject.PropertyChanged -= OnPropertyChanged;

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
                var newValue = m_observableObject[ m_propertyName ];

                var actionManager = m_observableObject.ActionManager;

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

        private readonly IObservableObject m_observableObject;
        private readonly string m_propertyName;

        private readonly Func<(object?, object?), string> m_descriptionGenerator;

        private readonly TimeSpan m_maxMergeTimeDiff;

        private object? m_currentValue;

        private bool m_ignoreEvents;
    }
}
