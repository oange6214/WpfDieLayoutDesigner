using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DieLayoutDesigner.Models;
using DieLayoutDesigner.MvvmToolKit.Input;
using DieLayoutDesigner.Controls;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DieLayoutDesigner.ViewModels;

public class MainWindowViewModel : ObservableObject
{

    private ObservableCollection<DieShape> _shapes = [];

    public ObservableCollection<DieShape> Shapes
    {
        get => _shapes;
        set => SetProperty(ref _shapes, value);
    }

    public MainWindowViewModel()
    {
        _shapes = new ObservableCollection<DieShape>();

        StartDrawingCommand = new RelayCommand<Point>(StartDrawing);
        DrawingCommand = new RelayCommand<Point>(Drawing);
        EndDrawingCommand = new RelayCommand<Point>(EndDrawing);
        SelectShapeCommand = new RelayCommand<DieShape>(SelectShape);
        CanvasClickCommand = new RelayCommand<Point>(ClearSelection);
        DeleteCommand = new RelayCommand(DeleteSelected, () => SelectedShape != null);
        CancelCommand = new RelayCommand(Cancel);
        SelectAllCommand = new RelayCommand(SelectAll);
        MoveCommand = new RelayCommand<string>(MoveSelected);
    }


    #region Mouse Events


    private Point _startPoint;
    private bool _isDrawing;

    public ICommand StartDrawingCommand { get; }
    public ICommand DrawingCommand { get; }
    public ICommand EndDrawingCommand { get; }
    public ICommand CanvasClickCommand { get; }

    private void StartDrawing(Point point)
    {
        if (Application.Current.MainWindow?.Content is FrameworkElement rootElement)
        {
            var canvas = rootElement.FindName("DrawingCanvas") as Canvas;
            if (canvas != null)
            {
                _isDrawing = true;
                _startPoint = point;

                var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                _previewAdorner = new PreviewAdorner(canvas, point);
                adornerLayer?.Add(_previewAdorner);
            }
        }
    }

    private void Drawing(Point currentPoint)
    {
        if (_isDrawing && _previewAdorner != null)
        {
            _previewAdorner.UpdatePosition(currentPoint);
        }
    }

    private void EndDrawing(Point endPoint)
    {
        if (_isDrawing && _previewAdorner != null)
        {
            StatusText = "點擊空白處開始繪製，或點擊圖形進行編輯";

            // 計算移動距離
            double width = Math.Abs(endPoint.X - _startPoint.X);
            double height = Math.Abs(endPoint.Y - _startPoint.Y);

            // 如果大小足夠，創建實際的形狀
            if (width > 5 && height > 5)
            {
                var newShape = CreateShape(_startPoint, endPoint);
                Shapes.Add(newShape);
            }

            // 移除預覽

            if (Application.Current.MainWindow?.Content is FrameworkElement rootElement)
            {
                var canvas = rootElement.FindName("DrawingCanvas") as Canvas;

                var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                adornerLayer?.Remove(_previewAdorner);
                _previewAdorner = null;
                _isDrawing = false;
            }
        }
    }

    private DieShape CreateShape(Point start, Point end)
    {
        double width = Math.Abs(end.X - start.X);
        double height = Math.Abs(end.Y - start.Y);
        double left = Math.Min(end.X, start.X);
        double top = Math.Min(end.Y, start.Y);

        _currentMaxZIndex++;

        return new DieShape
        {
            TopLeft = new Point(left, top),
            DieSize = new Size(width, height),
            Data = new RectangleGeometry(new Rect(0, 0, width, height)),
            FillColor = new SolidColorBrush(Color.FromRgb(
                (byte)Random.Shared.Next(256),
                (byte)Random.Shared.Next(256),
                (byte)Random.Shared.Next(256))),
            ZIndex = _currentMaxZIndex
        };
    }

    #endregion

    #region Shape interaction

    private PreviewAdorner? _previewAdorner;

    private int _currentMaxZIndex = 0;

    private DieShape? _selectedShape;

    public DieShape? SelectedShape
    {
        get => _selectedShape;
        set
        {
            if (_selectedShape != null)
                _selectedShape.IsSelected = false;

            SetProperty(ref _selectedShape, value);

            if (_selectedShape != null)
                _selectedShape.IsSelected = true;
        }
    }

    public ICommand SelectShapeCommand { get; }

    public void SelectShape(DieShape? shape)
    {
        if (shape != null)
        {
            SelectedShape = shape;
            BringToFront(shape);  // 選取時自動置頂
        }
    }

    private void BringToFront(DieShape shape)
    {
        if (shape != null)
        {
            _currentMaxZIndex++;
            shape.ZIndex = _currentMaxZIndex;
        }
    }

    private void ClearSelection(Point point)
    {
        SelectedShape = null;
    }

    #endregion

    #region State Tip

    private string _statusText = "點擊空白處開始繪製，或點擊圖形進行編輯";
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    #endregion

    #region Ux interactions

    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SelectAllCommand { get; }
    public ICommand MoveCommand { get; }

    private void DeleteSelected()
    {
        if (SelectedShape != null)
        {
            Shapes.Remove(SelectedShape);
            SelectedShape = null;
        }
    }

    private void Cancel()
    {
        if (_isDrawing)
        {
            _isDrawing = false;
        }
        SelectedShape = null;
    }

    private void SelectAll()
    {
        // 需要修改為支援多選
        // ...
    }

    private void MoveSelected(string direction)
    {
        if (SelectedShape != null)
        {
            var delta = 1;
            switch (direction)
            {
                case "Left":
                    SelectedShape.TopLeft = new Point(SelectedShape.TopLeft.X - delta, SelectedShape.TopLeft.Y);
                    break;
                case "Right":
                    SelectedShape.TopLeft = new Point(SelectedShape.TopLeft.X + delta, SelectedShape.TopLeft.Y);
                    break;
                case "Up":
                    SelectedShape.TopLeft = new Point(SelectedShape.TopLeft.X, SelectedShape.TopLeft.Y - delta);
                    break;
                case "Down":
                    SelectedShape.TopLeft = new Point(SelectedShape.TopLeft.X, SelectedShape.TopLeft.Y + delta);
                    break;
            }
        }
    }

    #endregion

}