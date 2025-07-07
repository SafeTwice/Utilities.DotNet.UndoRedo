/// @file
/// @copyright  Copyright (c) 2024-2025 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using UndoRedoFramework.GenericActions;
using Utilities.DotNet;
using Utilities.DotNet.Collections.Observables;

namespace UndoRedoFramework.Generators
{
    /// <summary>
    /// Generates Undo/Redo actions for an observable collection.
    /// </summary>
    /// <remarks>
    /// When the observed collection changes, the generator creates the corresponding action:
    /// <see cref="InsertIntoCollectionAction"/>, <see cref="RemoveFromCollectionAction"/> or <see cref="ReplaceInCollectionAction"/>.
    /// </remarks>
    public class ObservableCollectionActionGenerator<T> : DisposableObject, ICollectionManager
    {
        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="actionManager">Manager for the actions.</param>
        /// <param name="collection">Collection to observe.</param>
        /// <param name="descriptionGenerator">Function to generate the description of the actions.</param>
        /// <param name="maxMergeTimeDiff">Maximum time difference to merge with another action.</param>
        public ObservableCollectionActionGenerator( IObservableCollection collection, Func<IUndoRedoActionManager?> actionManagerProvider,
                                                    Func<NotifyCollectionChangedAction, string> descriptionGenerator,
                                                    TimeSpan? maxMergeTimeDiff = null )
        {
            m_actionManagerProvider = actionManagerProvider;
            m_collection = collection;
            m_descriptionGenerator = descriptionGenerator;
            m_maxMergeTimeDiff = maxMergeTimeDiff ?? DEFAULT_MAX_MERGE_TIME_DIFF;

            m_collection.CollectionChanged += OnCollectionChanged;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        public void InsertItems( IList items, int insertionIndex )
        {
            m_ignoreEvents = true;

            try
            {
                m_collection.AddRange( items );
            }
            finally
            {
                m_ignoreEvents = false;
            }
        }

        public void RemoveItems( IList items )
        {
            m_ignoreEvents = true;

            try
            {
                m_collection.RemoveRange( items );
            }
            finally
            {
                m_ignoreEvents = false;
            }
        }

        public void ReplaceItem( object oldItem, object newItem )
        {
            m_ignoreEvents = true;

            try
            {
                m_collection.Replace( (T) oldItem, (T) newItem );
            }
            finally
            {
                m_ignoreEvents = false;
            }
        }

        public void MoveItem( int oldIndex, int newIndex ) => throw new NotImplementedException();

        //===========================================================================
        //                            PROTECTED METHODS
        //===========================================================================

        protected override void Dispose( bool disposing )
        {
            m_collection.CollectionChanged -= OnCollectionChanged;

            base.Dispose( disposing );
        }

        //===========================================================================
        //                            PRIVATE METHODS
        //===========================================================================

        private void OnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
        {
            var actionManager = m_actionManagerProvider();

            if( m_ignoreEvents || ( actionManager == null ) )
            {
                return;
            }

            Debug.Assert( sender == m_collection );

            switch( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    if( e.NewItems != null )
                    {
                        actionManager.Register( new InsertIntoCollectionAction( this, e.NewItems, e.NewStartingIndex, m_descriptionGenerator( e.Action ),
                                                                                m_maxMergeTimeDiff ) );
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if( e.OldItems != null )
                    {
                        actionManager.Register( new RemoveFromCollectionAction( this, e.OldItems, e.OldStartingIndex, m_descriptionGenerator( e.Action ),
                                                                                m_maxMergeTimeDiff ) );
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if( ( e.NewItems != null ) && ( e.OldItems != null ) )
                    {
                        Debug.Assert( e.NewItems.Count == 1 );
                        Debug.Assert( e.OldItems.Count == 1 );

                        actionManager.Register( new ReplaceInCollectionAction( this, e.OldItems[ 0 ]!, e.NewItems[ 0 ]!, m_descriptionGenerator( e.Action ),
                                                                               m_maxMergeTimeDiff ) );
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        //===========================================================================
        //                           PRIVATE CONSTANTS
        //===========================================================================

        private static readonly TimeSpan DEFAULT_MAX_MERGE_TIME_DIFF = TimeSpan.FromSeconds( 1 );

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly Func<IUndoRedoActionManager?> m_actionManagerProvider;
        private readonly IObservableCollection m_collection;

        private readonly Func<NotifyCollectionChangedAction, string> m_descriptionGenerator;

        private readonly TimeSpan m_maxMergeTimeDiff;

        private bool m_ignoreEvents;
    }
}
