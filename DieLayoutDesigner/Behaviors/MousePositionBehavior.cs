using DieLayoutDesigner.Controls;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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

    public static readonly DependencyProperty ClearSelectionCommandProperty =
        DependencyProperty.Register(nameof(ClearSelectionCommand), typeof(ICommand),
            typeof(MousePositionBehavior));

    public ICommand ClearSelectionCommand
    {
        get => (ICommand)GetValue(ClearSelectionCommandProperty);
        set => SetValue(ClearSelectionCommandProperty, value);
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

        var position = e.GetPosition(AssociatedObject);
        var hitTarget = e.OriginalSource as DependencyObject;

        var isControlHit = hitTarget is Rectangle ||
                          hitTarget is Thumb ||
                          VisualTreeHelper.GetParent(hitTarget) is SelectableRectangle;

        if (!isControlHit)
        {
            if (ClearSelectionCommand?.CanExecute(null) == true)
            {
                ClearSelectionCommand.Execute(position);
            }
        }

        if (MouseDownCommand?.CanExecute(null) == true)
        {
            MouseDownCommand.Execute(position);
        }

        e.Handled = true;
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