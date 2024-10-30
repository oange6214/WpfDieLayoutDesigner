using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using DieLayoutDesigner.Models;
using DieLayoutDesigner.ViewModels;

namespace DieLayoutDesigner.Controls;

// Controls/SelectableRectangle.xaml.cs
public partial class SelectableRectangle : UserControl
{
    private Point _startPoint;
    private bool _isDragging;
    private bool _isResizing;
    private string _currentHandle = string.Empty;

    public SelectableRectangle()
    {
        InitializeComponent();

        // 添加拖曳事件
        MainRectangle.MouseLeftButtonDown += OnRectangleMouseDown;
        MainRectangle.MouseMove += OnRectangleMouseMove;
        MainRectangle.MouseLeftButtonUp += OnRectangleMouseUp;

        // 添加調整大小事件
        foreach (var handle in ResizeHandles.Children.OfType<Rectangle>())
        {
            handle.MouseLeftButtonDown += OnResizeHandleMouseDown;
            handle.MouseMove += OnResizeHandleMouseMove;
            handle.MouseLeftButtonUp += OnResizeHandleMouseUp;
        }
    }

    private void OnRectangleMouseDown(object sender, MouseButtonEventArgs e)
    {
        var shape = DataContext as DieShape;
        if (shape != null)
        {
            // 確保事件不會繼續傳播到 Canvas
            e.Handled = true;

            // 開始拖曳
            _isDragging = true;
            _startPoint = e.GetPosition(this.Parent as UIElement);
            MainRectangle.CaptureMouse();

            // 設置選取狀態
            if (this.DataContext is DieShape currentShape)
            {
                // 獲取 ViewModel
                if (this.DataContext != null &&
                    Window.GetWindow(this)?.DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.SelectedShape = currentShape;
                }
            }
        }
    }

    private void OnRectangleMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging)
        {
            var shape = DataContext as DieShape;
            if (shape != null)
            {
                var currentPoint = e.GetPosition(this.Parent as UIElement);
                var delta = currentPoint - _startPoint;

                // 更新位置
                shape.TopLeft = new Point(
                    shape.TopLeft.X + delta.X,
                    shape.TopLeft.Y + delta.Y
                );

                _startPoint = currentPoint;
            }
        }
    }

    private void OnRectangleMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            MainRectangle.ReleaseMouseCapture();
        }
    }

    private void OnResizeHandleMouseDown(object sender, MouseButtonEventArgs e)
    {
        var handle = sender as Rectangle;
        if (handle != null)
        {
            _isResizing = true;
            _currentHandle = handle.Tag.ToString();
            _startPoint = e.GetPosition(this.Parent as UIElement);
            handle.CaptureMouse();
            e.Handled = true;
        }
    }

    private void OnResizeHandleMouseMove(object sender, MouseEventArgs e)
    {
        if (_isResizing)
        {
            var currentPos = e.GetPosition(this.Parent as UIElement);
            var delta = currentPos - _startPoint;
            var shape = DataContext as DieShape;

            if (shape != null)
            {
                // 保存原始值以便驗證
                var originalTopLeft = shape.TopLeft;
                var originalSize = shape.DieSize;

                switch (_currentHandle)
                {
                    case "TopLeft":
                        shape.TopLeft = new Point(
                            originalTopLeft.X + delta.X,
                            originalTopLeft.Y + delta.Y);
                        shape.DieSize = new Size(
                            Math.Max(20, originalSize.Width - delta.X),
                            Math.Max(20, originalSize.Height - delta.Y));
                        break;

                    case "TopRight":
                        shape.TopLeft = new Point(
                            originalTopLeft.X,
                            originalTopLeft.Y + delta.Y);
                        shape.DieSize = new Size(
                            Math.Max(20, originalSize.Width + delta.X),
                            Math.Max(20, originalSize.Height - delta.Y));
                        break;

                    case "BottomLeft":
                        shape.TopLeft = new Point(
                            originalTopLeft.X + delta.X,
                            originalTopLeft.Y);
                        shape.DieSize = new Size(
                            Math.Max(20, originalSize.Width - delta.X),
                            Math.Max(20, originalSize.Height + delta.Y));
                        break;

                    case "BottomRight":
                        shape.DieSize = new Size(
                            Math.Max(20, originalSize.Width + delta.X),
                            Math.Max(20, originalSize.Height + delta.Y));
                        break;
                }
            }
            _startPoint = currentPos;
        }
    }

    private void OnResizeHandleMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isResizing)
        {
            _isResizing = false;
            var handle = sender as Rectangle;
            handle?.ReleaseMouseCapture();
        }
    }
}