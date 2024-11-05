using CommunityToolkit.Mvvm.ComponentModel;
using DieLayoutDesigner.Enums;
using DieLayoutDesigner.Managers;
using DieLayoutDesigner.Models;
using DieLayoutDesigner.MvvmToolKit.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DieLayoutDesigner.ViewModels;

public class MainWindowViewModel : ObservableObject, IDisposable
{
    #region Constructors

    public MainWindowViewModel()
    {
        _shapeManager = new ShapeManager();
        _previewManager = new PreviewManager();
        InitializeCommands();
    }

    #endregion Constructors

    #region Fields

    private readonly PreviewManager _previewManager;
    private readonly ShapeManager _shapeManager;
    private EditMode _currentMode;
    private bool _disposed;
    private ICommand _panEndCommand;
    private ICommand _panningCommand;
    private ICommand _panStartCommand;
    private double _scaleValue = 1;
    private DieShape? _selectedShape;
    private Point _startPoint;
    private string _statusText = "點擊空白處開始繪製，或點擊圖形進行編輯";
    private double _xOffset = 0;
    private double _yOffset = 0;
    private ICommand _zoomCommand;

    #endregion Fields

    #region Properties

    public ICommand CancelCommand { get; private set; }
    public ICommand ClearSelectionCommand { get; private set; }
    public EditMode CurrentMode
    {
        get => _currentMode;
        set
        {
            if (SetProperty(ref _currentMode, value))
            {
                UpdateStatusText();
            }
        }
    }

    public ICommand DeleteCommand { get; private set; }
    public ICommand DrawingCommand { get; private set; }
    public ICommand EndDrawingCommand { get; private set; }
    public ICommand MoveCommand { get; private set; }
    public ICommand PanEndCommand => _panEndCommand ??= new RelayCommand<MouseButtonEventArgs>(OnPanEnd);
    public ICommand PanningCommand => _panningCommand ??= new RelayCommand<Vector>(OnPanning);
    public ICommand PanStartCommand => _panStartCommand ??= new RelayCommand<MouseButtonEventArgs>(OnPanStart);
    public double ScaleValue
    {
        get => _scaleValue;
        set => SetProperty(ref _scaleValue, value);
    }

    public ICommand SelectAllCommand { get; private set; }
    public DieShape? SelectedShape
    {
        get => _selectedShape;
        set
        {
            if (value == _selectedShape) return;

            if (_selectedShape != null)
                _selectedShape.IsSelected = false;

            SetProperty(ref _selectedShape, value);

            if (_selectedShape != null)
            {
                _selectedShape.IsSelected = true;
                _shapeManager.BringToFront(_selectedShape);
            }

            CommandManager.InvalidateRequerySuggested();
        }
    }

    public ICommand SelectShapeCommand { get; private set; }
    public ObservableCollection<DieShape> Shapes => _shapeManager.Shapes;
    public ICommand StartDrawingCommand { get; private set; }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }
    public double XOffset
    {
        get => _xOffset;
        set => SetProperty(ref _xOffset, value);
    }

    public double YOffset
    {
        get => _yOffset;
        set => SetProperty(ref _yOffset, value);
    }
    public ICommand ZoomCommand => _zoomCommand ??= new RelayCommand<MouseWheelEventArgs>(OnZoom);

    #endregion Properties

    #region Methods

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void SelectShape(DieShape? shape)
    {
        if (shape != null)
        {
            SelectedShape = shape;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _previewManager.Dispose();
            }
            _disposed = true;
        }
    }

    private void Cancel()
    {
        if (CurrentMode == EditMode.Drawing)
        {
            try
            {
                var canvas = GetDrawingCanvas();
                _previewManager.EndPreview(canvas);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cancelling drawing: {ex.Message}");
            }
        }
        CurrentMode = EditMode.None;
        SelectedShape = null;
    }

    private bool CanDeleteSelected() => SelectedShape != null;

    private bool CanMoveSelected(string direction) => SelectedShape != null;

    private void ClearSelection(Point point)
    {
        if (CurrentMode != EditMode.Drawing)
        {
            SelectedShape = null;
        }
    }

    private void DeleteSelected()
    {
        if (SelectedShape != null)
        {
            _shapeManager.DeleteShape(SelectedShape);
            SelectedShape = null;
        }
    }

    private void Drawing(Point currentPoint)
    {
        if (CurrentMode == EditMode.Drawing)
        {
            _previewManager.UpdatePreview(currentPoint);
        }
    }

    private void EndDrawing(Point endPoint)
    {
        if (CurrentMode == EditMode.Drawing)
        {
            try
            {
                var canvas = GetDrawingCanvas();

                double width = Math.Abs(endPoint.X - _startPoint.X);
                double height = Math.Abs(endPoint.Y - _startPoint.Y);

                if (width > 5 && height > 5)
                {
                    SelectedShape = _shapeManager.CreateShape(_startPoint, endPoint);
                }

                _previewManager.EndPreview(canvas);
                CurrentMode = EditMode.None;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error ending drawing: {ex.Message}");
                CurrentMode = EditMode.None;
            }
        }
    }

    private Canvas GetDrawingCanvas()
    {
        if (Application.Current.MainWindow?.Content is not FrameworkElement rootElement)
        {
            throw new InvalidOperationException("Cannot find root element");
        }

        return rootElement.FindName("DrawingCanvas") as Canvas
            ?? throw new InvalidOperationException("Cannot find canvas");
    }

    private void InitializeCommands()
    {
        ClearSelectionCommand = new RelayCommand<Point>(ClearSelection);
        StartDrawingCommand = new RelayCommand<Point>(StartDrawing);
        DrawingCommand = new RelayCommand<Point>(Drawing);
        EndDrawingCommand = new RelayCommand<Point>(EndDrawing);
        SelectShapeCommand = new RelayCommand<DieShape>(SelectShape);
        DeleteCommand = new RelayCommand(DeleteSelected, CanDeleteSelected);
        CancelCommand = new RelayCommand(Cancel);
        SelectAllCommand = new RelayCommand(SelectAll);
        MoveCommand = new RelayCommand<string>(MoveSelected, CanMoveSelected);
    }

    private void MoveSelected(string direction)
    {
        if (SelectedShape == null) return;

        var delta = 1;
        var currentPoint = SelectedShape.TopLeft;

        var newPoint = direction switch
        {
            "Left" => new Point(currentPoint.X - delta, currentPoint.Y),
            "Right" => new Point(currentPoint.X + delta, currentPoint.Y),
            "Up" => new Point(currentPoint.X, currentPoint.Y - delta),
            "Down" => new Point(currentPoint.X, currentPoint.Y + delta),
            _ => currentPoint
        };

        SelectedShape.TopLeft = newPoint;
    }

    private void OnPanEnd(MouseButtonEventArgs e)
    {
        // 可以在這裡添加其他邏輯
    }

    private void OnPanning(Vector delta)
    {
        XOffset += delta.X;
        YOffset += delta.Y;
    }

    private void OnPanStart(MouseButtonEventArgs e)
    {
        // 可以在這裡添加其他邏輯
    }

    private void OnZoom(MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
        {
            ScaleValue = Math.Min(ScaleValue + 0.5, 10);
        }
        else
        {
            ScaleValue = Math.Max(ScaleValue - 0.5, 1);
        }
    }

    private void SelectAll()
    {
        // TODO: Implement multi-select
    }

    private void StartDrawing(Point point)
    {
        try
        {
            if (Application.Current.MainWindow?.Content is not FrameworkElement rootElement)
            {
                throw new InvalidOperationException("Cannot find root element");
            }

            var canvas = rootElement.FindName("DrawingCanvas") as Canvas
                ?? throw new InvalidOperationException("Cannot find canvas");

            CurrentMode = EditMode.Drawing;
            _startPoint = point;
            _previewManager.StartPreview(canvas, point, ScaleValue);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error starting drawing: {ex.Message}");
            CurrentMode = EditMode.None;
        }
    }
    private void UpdateStatusText()
    {
        StatusText = CurrentMode switch
        {
            EditMode.None => "點擊空白處開始繪製，或點擊圖形進行編輯",
            EditMode.Drawing => "拖曳以繪製圖形",
            EditMode.Moving => "拖曳以移動圖形",
            EditMode.Resizing => "拖曳以調整大小",
            _ => StatusText
        };
    }

    #endregion Methods
}