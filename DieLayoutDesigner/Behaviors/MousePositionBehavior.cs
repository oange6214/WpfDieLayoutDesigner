using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DieLayoutDesigner.Behaviors;

public class MousePositionBehavior : Behavior<Canvas>
{
    // MouseLeftButtonDown Command
    public static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.Register(
        nameof(MouseDownCommand),
        typeof(ICommand),
        typeof(MousePositionBehavior)
    );

    public ICommand MouseDownCommand
    {
        get => (ICommand)GetValue(MouseDownCommandProperty);
        set => SetValue(MouseDownCommandProperty, value);
    }

    // MouseMove Command
    public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.Register(
        nameof(MouseMoveCommand),
        typeof(ICommand),
        typeof(MousePositionBehavior)
    );

    public ICommand MouseMoveCommand
    {
        get => (ICommand)GetValue(MouseMoveCommandProperty);
        set => SetValue(MouseMoveCommandProperty, value);
    }

    // MouseLeftButtonUp Command
    public static readonly DependencyProperty MouseUpCommandProperty = DependencyProperty.Register(
        nameof(MouseUpCommand),
        typeof(ICommand),
        typeof(MousePositionBehavior)
    );

    public ICommand MouseUpCommand
    {
        get => (ICommand)GetValue(MouseUpCommandProperty);
        set => SetValue(MouseUpCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseDown;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseUp;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnMouseDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseUp;
        base.OnDetaching();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (MouseDownCommand?.CanExecute(null) == true)
        {
            var position = e.GetPosition(AssociatedObject);
            MouseDownCommand.Execute(position);
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (MouseMoveCommand?.CanExecute(null) == true)
        {
            var position = e.GetPosition(AssociatedObject);
            MouseMoveCommand.Execute(position);
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (MouseUpCommand?.CanExecute(null) == true)
        {
            var position = e.GetPosition(AssociatedObject);
            MouseUpCommand.Execute(position);
        }
    }
}