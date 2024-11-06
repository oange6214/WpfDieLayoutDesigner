using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DieLayoutDesigner.Behaviors;

public class DiePanelBehavior : Behavior<Canvas>
{

    #region Fields

    public static readonly DependencyProperty ContextMenuCommandProperty =
        DependencyProperty.Register(
            nameof(ContextMenuCommand),
            typeof(ICommand),
            typeof(DiePanelBehavior));

    public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register(
            nameof(MaxScale),
            typeof(double),
            typeof(DiePanelBehavior),
            new PropertyMetadata(10.0));

    public static readonly DependencyProperty MinScaleProperty =
        DependencyProperty.Register(
            nameof(MinScale),
            typeof(double),
            typeof(DiePanelBehavior),
            new PropertyMetadata(0.1));

    public static readonly DependencyProperty MouseDownCommandProperty =
                DependencyProperty.Register(
            nameof(MouseDownCommand),
            typeof(ICommand),
            typeof(DiePanelBehavior));

    public static readonly DependencyProperty MouseMoveCommandProperty =
        DependencyProperty.Register(
            nameof(MouseMoveCommand),
            typeof(ICommand),
            typeof(DiePanelBehavior));

    public static readonly DependencyProperty MouseUpCommandProperty =
        DependencyProperty.Register(
            nameof(MouseUpCommand),
            typeof(ICommand),
            typeof(DiePanelBehavior));

    public static readonly DependencyProperty PanEndCommandProperty =
        DependencyProperty.Register(
            nameof(PanEndCommand),
            typeof(ICommand),
            typeof(DiePanelBehavior));

    public static readonly DependencyProperty PanningCommandProperty =
        DependencyProperty.Register(
            nameof(PanningCommand),
            typeof(ICommand),
            typeof(DiePanelBehavior));

    public static readonly DependencyProperty PanStartCommandProperty =
        DependencyProperty.Register(
            nameof(PanStartCommand),
            typeof(ICommand),
            typeof(DiePanelBehavior));

    public static readonly DependencyProperty ScaleValueProperty =
        DependencyProperty.Register(
            nameof(ScaleValue),
            typeof(double),
            typeof(DiePanelBehavior),
            new FrameworkPropertyMetadata(1.0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnTransformChanged));

    public static readonly DependencyProperty XOffsetProperty =
        DependencyProperty.Register(
            nameof(XOffset),
            typeof(double),
            typeof(DiePanelBehavior),
            new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnTransformChanged));

    public static readonly DependencyProperty YOffsetProperty =
        DependencyProperty.Register(
            nameof(YOffset),
            typeof(double),
            typeof(DiePanelBehavior),
            new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnTransformChanged));

    public static readonly DependencyProperty ZoomFactorProperty =
        DependencyProperty.Register(
            nameof(ZoomFactor),
            typeof(double),
            typeof(DiePanelBehavior),
            new PropertyMetadata(1.2));


    private ICoordinateSystem? _coordinateSystem;

    private bool _isPanning;

    private Point _lastPanPosition;

    #endregion Fields

    #region Properties

    public ICommand ContextMenuCommand
    {
        get => (ICommand)GetValue(ContextMenuCommandProperty);
        set => SetValue(ContextMenuCommandProperty, value);
    }

    public double MaxScale
    {
        get => (double)GetValue(MaxScaleProperty);
        set => SetValue(MaxScaleProperty, value);
    }

    public double MinScale
    {
        get => (double)GetValue(MinScaleProperty);
        set => SetValue(MinScaleProperty, value);
    }
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

    public ICommand PanEndCommand
    {
        get => (ICommand)GetValue(PanEndCommandProperty);
        set => SetValue(PanEndCommandProperty, value);
    }

    public ICommand PanningCommand
    {
        get => (ICommand)GetValue(PanningCommandProperty);
        set => SetValue(PanningCommandProperty, value);
    }

    public ICommand PanStartCommand
    {
        get => (ICommand)GetValue(PanStartCommandProperty);
        set => SetValue(PanStartCommandProperty, value);
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

    public double ZoomFactor
    {
        get => (double)GetValue(ZoomFactorProperty);
        set => SetValue(ZoomFactorProperty, value);
    }

    #endregion Properties

    #region Methods

    protected override void OnAttached()
    {
        base.OnAttached();
        AttachEventHandlers();
        InitializeCoordinateSystem();
    }

    protected override void OnDetaching()
    {
        DetachEventHandlers();
        base.OnDetaching();
    }

    private static void OnTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = (DiePanelBehavior)d;
        behavior.UpdateCoordinateSystem();
    }

    private void AttachEventHandlers()
    {
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
        AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
        AssociatedObject.PreviewMouseUp += OnPreviewMouseUp;
        AssociatedObject.PreviewMouseMove += OnPreviewPanMove;
        AssociatedObject.PreviewMouseRightButtonUp += OnPreviewMouseRightButtonUp;
    }

    private void DetachEventHandlers()
    {
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
        AssociatedObject.PreviewMouseUp -= OnPreviewMouseUp;
        AssociatedObject.PreviewMouseMove -= OnPreviewPanMove;
        AssociatedObject.PreviewMouseRightButtonUp -= OnPreviewMouseRightButtonUp;
    }

    private Point GetTransformedPosition(MouseEventArgs e)
    {
        var position = e.GetPosition(AssociatedObject);
        return _coordinateSystem?.ToLogical(position) ?? position;
    }

    private void InitializeCoordinateSystem()
    {
        _coordinateSystem = new ScaledCoordinateSystem(ScaleValue, new Point(XOffset, YOffset));
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_isPanning)
            return;

        if (MouseDownCommand?.CanExecute(null) == true)
        {
            var position = GetTransformedPosition(e);
            MouseDownCommand.Execute(position);
            e.Handled = true;
        }
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isPanning)
            return;

        if (MouseUpCommand?.CanExecute(null) == true)
        {
            var position = GetTransformedPosition(e);
            MouseUpCommand.Execute(position);
            e.Handled = true;
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isPanning)
            return;

        if (MouseMoveCommand?.CanExecute(null) == true)
        {
            var position = GetTransformedPosition(e);
            MouseMoveCommand.Execute(position);
        }
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton != MouseButtonState.Pressed)
            return;

        _lastPanPosition = e.GetPosition(AssociatedObject);
        _isPanning = true;
        AssociatedObject.Cursor = Cursors.Hand;
        Mouse.Capture((IInputElement)e.OriginalSource);

        PanStartCommand?.Execute(e);
        e.Handled = true;
    }

    private void OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (ContextMenuCommand?.CanExecute(null) == true)
        {
            var position = GetTransformedPosition(e);
            ContextMenuCommand.Execute(position);
            e.Handled = true;
        }
    }
    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isPanning || e.MiddleButton != MouseButtonState.Released) 
            return;

        _isPanning = false;
        AssociatedObject.Cursor = Cursors.Arrow;
        Mouse.Capture(null);

        PanEndCommand?.Execute(e);
        e.Handled = true;
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;

        var mousePosition = e.GetPosition(AssociatedObject);
        ZoomFactor = e.Delta > 0 ? 1.2 : 1 / 1.2;
        var newScale = Math.Clamp(ScaleValue * ZoomFactor, MinScale, MaxScale);

        if (Math.Abs(newScale - ScaleValue) < 0.001) return;

        UpdateZoom(mousePosition, newScale);
    }

    private void OnPreviewPanMove(object sender, MouseEventArgs e)
    {
        if (!_isPanning)
            return;

        var currentPosition = e.GetPosition(AssociatedObject);
        var delta = currentPosition - _lastPanPosition;
        _lastPanPosition = currentPosition;

        PanningCommand?.Execute(delta);
        e.Handled = true;
    }

    private void UpdateCoordinateSystem()
    {
        _coordinateSystem = new ScaledCoordinateSystem(ScaleValue, new Point(XOffset, YOffset));
    }

    private void UpdateZoom(Point mousePosition, double newScale)
    {
        var oldX = XOffset;
        var oldY = YOffset;

        XOffset = mousePosition.X - (mousePosition.X - oldX) * newScale / ScaleValue;
        YOffset = mousePosition.Y - (mousePosition.Y - oldY) * newScale / ScaleValue;
        ScaleValue = newScale;
    }

    #endregion Methods

}
