/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
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
    public class ObservableObjectActionGenerator : IObservableObjectManager
    {
        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        public ObservableObjectActionGenerator( IObservableObject observableObject, string propertyName,
                                                string actionDescription, TimeSpan? maxMergeTimeDiff = null )
        {
            m_observableObject = observableObject;
            m_propertyName = propertyName;

            m_actionDescription = actionDescription;

            m_maxMergeTimeDiff = maxMergeTimeDiff ?? DEFAULT_MAX_MERGE_TIME_DIFF;

            m_currentValue = m_observableObject[ propertyName ];

            m_observableObject.PropertyChanged += OnPropertyChanged;
        }

        //===========================================================================
        //                               FINALIZER
        //===========================================================================

        ~ObservableObjectActionGenerator()
        {
            m_observableObject.PropertyChanged -= OnPropertyChanged;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        public void SetObservedPropertyValue( object? value )
        {
            m_ignoreEvents = true;

            m_observableObject[ m_propertyName ] = value;

            m_ignoreEvents = false;
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
                    actionManager.Register( new UpdateObservableObjectAction( this, oldValue, newValue, m_actionDescription, m_maxMergeTimeDiff ) );
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

        private readonly string m_actionDescription;

        private readonly TimeSpan m_maxMergeTimeDiff;

        private object? m_currentValue;

        private bool m_ignoreEvents;
    }
}
