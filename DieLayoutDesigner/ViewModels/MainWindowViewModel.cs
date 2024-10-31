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
using DieLayoutDesigner.Enums;
using DieLayoutDesigner.Managers;
using System.Diagnostics;

namespace DieLayoutDesigner.ViewModels;

public class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly ShapeManager _shapeManager;
    private readonly PreviewManager _previewManager;
    private bool _disposed;

    #region Properties

    private DieShape? _selectedShape;
    private EditMode _currentMode;
    private Point _startPoint;
    private string _statusText = "點擊空白處開始繪製，或點擊圖形進行編輯";

    public ObservableCollection<DieShape> Shapes => _shapeManager.Shapes;

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

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    #endregion

    #region Commands

    public ICommand StartDrawingCommand { get; private set; }
    public ICommand DrawingCommand { get; private set; }
    public ICommand EndDrawingCommand { get; private set; }
    public ICommand SelectShapeCommand { get; private set; }
    public ICommand CanvasClickCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand SelectAllCommand { get; private set; }
    public ICommand MoveCommand { get; private set; }

    #endregion

    public MainWindowViewModel()
    {
        _shapeManager = new ShapeManager();
        _previewManager = new PreviewManager();
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        StartDrawingCommand = new RelayCommand<Point>(StartDrawing);
        DrawingCommand = new RelayCommand<Point>(Drawing);
        EndDrawingCommand = new RelayCommand<Point>(EndDrawing);
        SelectShapeCommand = new RelayCommand<DieShape>(SelectShape);
        CanvasClickCommand = new RelayCommand<Point>(ClearSelection);
        DeleteCommand = new RelayCommand(DeleteSelected, CanDeleteSelected);
        CancelCommand = new RelayCommand(Cancel);
        SelectAllCommand = new RelayCommand(SelectAll);
        MoveCommand = new RelayCommand<string>(MoveSelected, CanMoveSelected);
    }

    #region Command Methods

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
            _previewManager.StartPreview(canvas, point);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error starting drawing: {ex.Message}");
            CurrentMode = EditMode.None;
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

    public void SelectShape(DieShape? shape)
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

    private bool CanDeleteSelected() => SelectedShape != null;

    private void DeleteSelected()
    {
        if (SelectedShape != null)
        {
            _shapeManager.DeleteShape(SelectedShape);
            SelectedShape = null;
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

    private void SelectAll()
    {
        // TODO: Implement multi-select
    }

    private bool CanMoveSelected(string direction) => SelectedShape != null;

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

    #endregion

    #region Helper Methods

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

    private Canvas GetDrawingCanvas()
    {
        if (Application.Current.MainWindow?.Content is not FrameworkElement rootElement)
        {
            throw new InvalidOperationException("Cannot find root element");
        }

        return rootElement.FindName("DrawingCanvas") as Canvas
            ?? throw new InvalidOperationException("Cannot find canvas");
    }

    #endregion

    #region IDisposable Implementation

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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}