# Utilities.DotNet.UndoRedo

## About

The _Utilities.DotNet.UndoRedo_ package provides a framework for managing undo/redo actions in WPF applications.

## Installation

The easiest way to install Utilities.DotNet.UndoRedo is using the NuGet package: https://www.nuget.org/packages/Utilities.DotNet.UndoRedo/

## Getting Started

### 1. Define Undoable Actions

Implement the `IUndoRedoAction` interface for the actions you want to support undo/redo for.

Undoable actions generally have a constructor that takes the necessary parameters to perform (do/redo) 
and reverse (undo) the action (e.g., the changed object and the old and new values for a property).

The `Do` method performs the action (e.g., changes the object property to the new value), 
while the `Undo` method reverses it (e.g., changes the object property to the old value).

###### Examples (C#):
```CSharp
public class MyObject : ObservableObjectEx
{
  public string Name { get; init; }

  public string Value
  {
    get => m_value;
    set => SetProperty( ref m_value, value );
  }

  private string m_value;
}

public class MyObjectEditAction : IUndoRedoAction
{
  public string Description => $"Edit {m_myObject.Name}";

  public MyObjectEditAction( MyObject myObject, string oldValue, string newValue )
  {
    m_myObject = myObject;
    m_oldValue = oldValue;
    m_newValue = newValue;
  }

  public void Do()
  {
    m_myObject.Value = m_newValue;
  }

  public void Undo()
  {
    m_myObject.Value = m_oldValue;
  }

  private MyObject m_myObject;
  private string m_oldValue;
  private string m_newValue;
}

public class MyObjectAddAction : IUndoRedoAction
{
  public string Description => $"Add {m_myObject.Name}";

  public MyObjectAddAction( IList<MyObject> list, MyObject myObject )
  {
    m_list = list;
    m_myObject = myObject;
  }

  public void Do()
  {
    m_list.Add( m_myObject );
  }

  public void Undo()
  {
    m_list.Remove( m_myObject );
  }

  private IList<MyObject> m_list;
  private MyObject m_myObject;
}
```

### 2. Create an UndoRedoActionManager

In simple applications you can use a single instance of `UndoRedoActionManager` for the whole application.

More complex applications may require multiple instances (e.g., one per opened document).

###### Example (C# / WPF):

```csharp
public class WorkspaceViewModel : ObservableObject
{
  public static UndoRedoActionManager ActionManager { get; } = new();

  public string UndoCaption
  {
    get
    {
      var description = ActionManager.UndoActionDescription;
      return ( description != null ) ? $"_Undo: {description}" : "_Undo";
    }
  }

  public string RedoCaption
  {
    get
    {
      var description = ActionManager.RedoActionDescription;
      return ( description != null ) ? $"_Redo: {description}" : "_Redo";
    }
  }

  public IDelegateCommand UndoCommand => new DelegateCommand(
    () => ActionManager.Undo(),
    () => ActionManager.CanUndo );

  public IDelegateCommand RedoCommand => new DelegateCommand(
    () => ActionManager.Redo(),
    () => ActionManager.CanRedo );

  public WorkspaceViewModel()
  {
    ActionManager.UndoRedoStateChanged += ( _ ) =>
    {
      OnPropertyChanged( nameof( UndoCaption ) );
      OnPropertyChanged( nameof( RedoCaption ) );

      UndoCommand.RaiseCanExecuteChanged();
      RedoCommand.RaiseCanExecuteChanged();
    };
  }
}
```
```XML
<Window x:Class="MyApp.Views.WorkspaceWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
        xmlns:bh="clr-namespace:Utilities.DotNet.WPF.Behaviors;assembly=Utilities.DotNet.WPF"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        Title="My Application"
        Width="800"
        Height="800">
  <Window.DataContext>
    <vm:WorkspaceViewModel/>
  </Window.DataContext>
  <i:Interaction.Behaviors>
      <bh:RedirectCommandBinding SourceCommand="ApplicationCommands.Undo" TargetCommand="{Binding UndoCommand}"/>
      <bh:RedirectCommandBinding SourceCommand="ApplicationCommands.Redo" TargetCommand="{Binding RedoCommand}"/>
  </i:Interaction.Behaviors>
  <DockPanel>
    <Menu DockPanel.Dock="Top">
      <MenuItem Header="_Edit">
          <MenuItem Header="Cu_t" InputGestureText="Ctrl+X" Command="ApplicationCommands.Cut"/>
          <MenuItem Header="_Copy" InputGestureText="Ctrl+C" Command="ApplicationCommands.Copy"/>
          <MenuItem Header="_Paste" InputGestureText="Ctrl+V" Command="ApplicationCommands.Paste"/>
          <Separator/>
          <MenuItem Header="{Binding UndoCaption}" InputGestureText="Ctrl+Z" Command="ApplicationCommands.Undo"/>
          <MenuItem Header="{Binding RedoCaption}" InputGestureText="Ctrl+Y" Command="ApplicationCommands.Redo"/>
      </MenuItem>
    </Menu>
  </DockPanel>
</Window>
```

### 3. Register actions with the UndoRedoActionManager in response to user operations

When the user performs an operation that must be undoable/redoable, create an instance of the corresponding `IUndoRedoAction` implementation and register it with the `UndoRedoActionManager`.

There are two ways to register the actions:

- Call `UndoRedoActionManager.Register()`, which just adds the action to the undo stack. This is suitable for actions associated with operations that are already executed when the action is registered, e.g., an action registered on the event handler for an INotifyPropertyChanged.PropertyChanged event (i.e., the property is already changed to the new value when the event handler is executed, so the action manager just needs to care for undoing and redoing the operation).

- Call `UndoRedoActionManager.RegisterAndDo()`, which first calls the action's `Do()` method to perform the operation, and then adds the action to the undo stack. This is suitable for actions associated with operations that are not yet executed when the action is registered, e.g., an action registered on the "click" event handler of a button that performs the operation (i.e., the property is still at the old value when the event handler is executed, so the action manager needs to both perform and manage undoing/redoing the operation).

###### Example (C#):

```CSharp
public class MyViewModel
{
  public ObservableList<MyObject> ObjectList { get; } = new();

  public ICommand AddObjectCommand { get; }

  public MyViewModel()
  {
    AddObjectCommand = new DelegateCommand( OnAddObject );
  }

  private void OnAddObject()
  {
    var newObject = new MyObject { Name = GetNewName(), Value = "Default Value" };
    newObject.PropertyChangedEx += OnMyObjectPropertyChanged;
    
    var action = new MyObjectAddAction( ObjectList, newObject );
    
    WorkspaceViewModel.ActionManager.RegisterAndDo( action );
  }

  private static void OnMyObjectPropertyChanged( object sender, IPropertyChangedExEventArgs e )
  {
    if ( e.PropertyName == nameof( IMyObject.Value ) )
    {
      var action = new MyObjectEditAction( (IMyObject) sender, e.OldValue, e.NewValue );

      WorkspaceViewModel.ActionManager.Register( action );
    }
  }
}
```
