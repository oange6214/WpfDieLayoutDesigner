using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace DieLayoutDesigner.Behaviors;

public class MouseBehavior : Behavior<UIElement>
{
    public static readonly DependencyProperty MouseDownCommandProperty =
        DependencyProperty.Register(nameof(MouseDownCommand), typeof(ICommand), typeof(MouseBehavior));

    public ICommand MouseDownCommand
    {
        get => (ICommand)GetValue(MouseDownCommandProperty);
        set => SetValue(MouseDownCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnMouseDown;
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
}
