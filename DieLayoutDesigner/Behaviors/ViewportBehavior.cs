using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DieLayoutDesigner.Behaviors;

public class ViewportBehavior : Behavior<Canvas>
{
    private Point _lastPanPosition;
    private bool _isPanning;

    #region Commands
    public static readonly DependencyProperty ZoomCommandProperty =
        DependencyProperty.Register(nameof(ZoomCommand), typeof(ICommand), typeof(ViewportBehavior));

    public static readonly DependencyProperty PanStartCommandProperty =
        DependencyProperty.Register(nameof(PanStartCommand), typeof(ICommand), typeof(ViewportBehavior));

    public static readonly DependencyProperty PanningCommandProperty =
        DependencyProperty.Register(nameof(PanningCommand), typeof(ICommand), typeof(ViewportBehavior));

    public static readonly DependencyProperty PanEndCommandProperty =
        DependencyProperty.Register(nameof(PanEndCommand), typeof(ICommand), typeof(ViewportBehavior));

    public ICommand ZoomCommand
    {
        get => (ICommand)GetValue(ZoomCommandProperty);
        set => SetValue(ZoomCommandProperty, value);
    }

    public ICommand PanStartCommand
    {
        get => (ICommand)GetValue(PanStartCommandProperty);
        set => SetValue(PanStartCommandProperty, value);
    }

    public ICommand PanningCommand
    {
        get => (ICommand)GetValue(PanningCommandProperty);
        set => SetValue(PanningCommandProperty, value);
    }

    public ICommand PanEndCommand
    {
        get => (ICommand)GetValue(PanEndCommandProperty);
        set => SetValue(PanEndCommandProperty, value);
    }
    #endregion

    protected override void OnAttached()
    {
        base.OnAttached();

        // 註冊事件
        AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
        AssociatedObject.PreviewMouseUp += OnPreviewMouseUp;
        AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
        AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
        AssociatedObject.PreviewMouseUp -= OnPreviewMouseUp;
        AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
        AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        base.OnDetaching();
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        // 檢查是否為中鍵按下
        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            _lastPanPosition = e.GetPosition(null);
            _isPanning = true;
            AssociatedObject.Cursor = Cursors.Hand;
            Mouse.Capture((IInputElement)e.OriginalSource);

            if (PanStartCommand?.CanExecute(e) == true)
            {
                PanStartCommand.Execute(e);
            }

            e.Handled = true;
        }
    }

    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        // 檢查是否為中鍵釋放且正在平移
        if (e.MiddleButton == MouseButtonState.Released && _isPanning)
        {
            _isPanning = false;
            AssociatedObject.Cursor = Cursors.Arrow;
            Mouse.Capture(null);

            if (PanEndCommand?.CanExecute(e) == true)
            {
                PanEndCommand.Execute(e);
            }

            e.Handled = true;
        }
    }

    private void OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_isPanning)
        {
            var currentPosition = e.GetPosition(null);
            var delta = currentPosition - _lastPanPosition;
            _lastPanPosition = currentPosition;

            if (PanningCommand?.CanExecute(delta) == true)
            {
                PanningCommand.Execute(delta);
            }

            e.Handled = true;
        }
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (ZoomCommand?.CanExecute(e) == true)
        {
            ZoomCommand.Execute(e);
            e.Handled = true;
        }
    }
}