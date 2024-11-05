using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace DieLayoutDesigner.Behaviors;

public class MousePositionBehavior : Behavior<UIElement>
{

    #region Fields

    public static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.Register(
        nameof(MouseDownCommand),
        typeof(ICommand),
        typeof(MousePositionBehavior)
    );

    public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.Register(
        nameof(MouseMoveCommand),
        typeof(ICommand),
        typeof(MousePositionBehavior)
    );

    public static readonly DependencyProperty MouseUpCommandProperty = DependencyProperty.Register(
        nameof(MouseUpCommand),
        typeof(ICommand),
        typeof(MousePositionBehavior)
    );

    public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register(
                        nameof(ScaleValue),
            typeof(double),
            typeof(MousePositionBehavior),
            new PropertyMetadata(1.0)
    );

    public static readonly DependencyProperty XOffsetProperty = DependencyProperty.Register(
            nameof(XOffset),
            typeof(double),
            typeof(MousePositionBehavior),
            new PropertyMetadata(0.0)
    );

    public static readonly DependencyProperty YOffsetProperty = DependencyProperty.Register(
            nameof(YOffset),
            typeof(double),
            typeof(MousePositionBehavior),
            new PropertyMetadata(0.0)
    );

    private ICoordinateSystem? _coordinateSystem;

    #endregion Fields

    #region Properties

    public ICommand MouseDownCommand
    {
        get => (ICommand)GetValue(MouseDownCommandProperty);
        set => SetValue(MouseDownCommandProperty, value);
    }

    public ICommand MouseMoveCommand
    {
        get => (ICommand)GetValue(MouseMoveCommandProperty);
        set => SetValue(MouseMoveCommandProperty, value);
    }

    public ICommand MouseUpCommand
    {
        get => (ICommand)GetValue(MouseUpCommandProperty);
        set => SetValue(MouseUpCommandProperty, value);
    }

    public double ScaleValue
    {
        get => (double)GetValue(ScaleValueProperty);
        set => SetValue(ScaleValueProperty, value);
    }

    public double XOffset
    {
        get => (double)GetValue(XOffsetProperty);
        set => SetValue(XOffsetProperty, value);
    }

    public double YOffset
    {
        get => (double)GetValue(YOffsetProperty);
        set => SetValue(YOffsetProperty, value);
    }

    #endregion Properties

    #region Methods

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseLeftButtonDown += OnMouseDown;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseUp;

        _coordinateSystem = new ScaledCoordinateSystem(ScaleValue, new Point(XOffset, YOffset));
    }



    protected override void OnDetaching()
    {
        AssociatedObject.MouseLeftButtonDown -= OnMouseDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseUp;
        base.OnDetaching();
    }

    private Point GetTransformedPosition(MouseEventArgs e)
    {
        var position = e.GetPosition(AssociatedObject);
        //return position;
        return _coordinateSystem?.ToLogical(position) ?? position;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (MouseDownCommand?.CanExecute(null) == true)
        {
            var position = GetTransformedPosition(e);
            MouseDownCommand.Execute(position);
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (MouseMoveCommand?.CanExecute(null) == true)
        {
            var position = GetTransformedPosition(e);
            MouseMoveCommand.Execute(position);
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (MouseUpCommand?.CanExecute(null) == true)
        {
            var position = GetTransformedPosition(e);
            MouseUpCommand.Execute(position);
        }
    }

    #endregion Methods
}