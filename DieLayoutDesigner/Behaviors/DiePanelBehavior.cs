using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DieLayoutDesigner.Behaviors;

public class DiePanelBehavior : Behavior<Canvas>
{

    #region Fields

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

    private ICoordinateSystem? _coordinateSystem;
    private bool _isPanning;
    private Point _lastPanPosition;

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

    // 處理平移相關的滑鼠事件
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
        var zoomFactor = e.Delta > 0 ? 1.2 : 1 / 1.2;
        var newScale = Math.Clamp(ScaleValue * zoomFactor, 0.1, 10.0);

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
