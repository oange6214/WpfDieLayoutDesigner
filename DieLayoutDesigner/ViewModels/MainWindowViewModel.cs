using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DieLayoutDesigner.Models;
using DieLayoutDesigner.MvvmToolKit.Input;

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
    private DieShape? _previewShape;

    public ICommand StartDrawingCommand { get; }
    public ICommand DrawingCommand { get; }
    public ICommand EndDrawingCommand { get; }
    public ICommand CanvasClickCommand { get; }

    public DieShape? PreviewShape
    {
        get => _previewShape;
        set => SetProperty(ref _previewShape, value);
    }

    private void StartDrawing(Point point)
    {
        StatusText = "拖曳滑鼠來繪製圖形";

        SelectedShape = null;
        _startPoint = point;
        _isDrawing = true;
    }

    private void Drawing(Point currentPoint)
    {
        if (_isDrawing)
        {
            StatusText = $"目前大小: {PreviewShape?.DieSize.Width:F0} x {PreviewShape?.DieSize.Height:F0}";
            PreviewShape = CreateShape(_startPoint, currentPoint);
        }
    }

    private void EndDrawing(Point endPoint)
    {
        if (_isDrawing)
        {
            StatusText = "點擊空白處開始繪製，或點擊圖形進行編輯";

            // 計算移動距離
            double width = Math.Abs(endPoint.X - _startPoint.X);
            double height = Math.Abs(endPoint.Y - _startPoint.Y);

            // 如果移動距離太小，就不創建形狀
            if (width < 5 || height < 5)  // 可以根據需求調整這個最小值
            {
                PreviewShape = null;
                _isDrawing = false;
                return;
            }

            var newShape = CreateShape(_startPoint, endPoint);
            Shapes.Add(newShape);
            PreviewShape = null;
            _isDrawing = false;

            // 自動選取新創建的形狀
            SelectedShape = newShape;
        }
    }

    private DieShape CreateShape(Point start, Point end)
    {
        double width = Math.Abs(end.X - start.X);
        double height = Math.Abs(end.Y - start.Y);
        double left = Math.Min(end.X, start.X);
        double top = Math.Min(end.Y, start.Y);

        return new DieShape
        {
            TopLeft = new Point(left, top),
            DieSize = new Size(width, height),
            Data = new RectangleGeometry(new Rect(0, 0, width, height)),
            FillColor = new SolidColorBrush(Color.FromRgb(
                (byte)Random.Shared.Next(256),
                (byte)Random.Shared.Next(256),
                (byte)Random.Shared.Next(256)))
        };
    }


    #endregion

    #region Shape interaction

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

    private void SelectShape(DieShape? shape)
    {
        if (shape != null)
        {
            SelectedShape = shape;
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
            PreviewShape = null;
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
            var delta = 1; // 或其他合適的值
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
                    // ... Up/Down
            }
        }
    }

    #endregion

}