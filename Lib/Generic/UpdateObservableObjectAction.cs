/// @file
/// @copyright  Copyright (c) 2024 SafeTwice S.L. All rights reserved.
/// @license    See LICENSE.txt

namespace UndoRedoFramework.Generic
{
    /// <summary>
    /// Undo/Redo action for updating an observable object.
    /// </summary>
    public class UpdateObservableObjectAction : IAction
    {
        //===========================================================================
        //                           PUBLIC PROPERTIES
        //===========================================================================

        public string Description { get; }

        //===========================================================================
        //                          PUBLIC CONSTRUCTORS
        //===========================================================================

        public UpdateObservableObjectAction( IObservableObjectManager objectManager, object? oldValue, object? newValue,
                                             string description, TimeSpan maxMergeTimeDiff )
        {
            m_objectManager = objectManager;

            m_oldValue = oldValue;
            m_newValue = newValue;

            Description = description;

            m_maxMergeTimeDiff = maxMergeTimeDiff;
            m_time = DateTime.Now;
        }

        //===========================================================================
        //                            PUBLIC METHODS
        //===========================================================================

        public void Do()
        {
            m_objectManager.SetObservedPropertyValue( m_newValue );
        }

        public void Undo()
        {
            m_objectManager.SetObservedPropertyValue( m_oldValue );
        }

        public bool TryMerge( IAction action )
        {
            if( ( action is UpdateObservableObjectAction updateAction ) &&
                ( updateAction.m_objectManager == m_objectManager ) &&
                ( updateAction.m_oldValue == m_newValue ) &&
                ( ( updateAction.m_time - m_time)  < m_maxMergeTimeDiff ) )
            {
                m_newValue = updateAction.m_newValue;
                m_time = updateAction.m_time;
                return true;
            }

            return false;
        }

        //===========================================================================
        //                           PRIVATE ATTRIBUTES
        //===========================================================================

        private readonly IObservableObjectManager m_objectManager;

        private readonly object? m_oldValue;
        private object? m_newValue;

        private readonly TimeSpan m_maxMergeTimeDiff;
       
        private DateTime m_time;
    }
}
