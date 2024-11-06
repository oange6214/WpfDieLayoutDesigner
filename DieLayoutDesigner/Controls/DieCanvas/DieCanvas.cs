using DieLayoutDesigner.Enums;
using DieLayoutDesigner.Managers;
using DieLayoutDesigner.Models;
using DieLayoutDesigner.MvvmToolKit.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DieLayoutDesigner.Controls;

public class DieCanvas : Control
{

    #region Constructors

    static DieCanvas()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(DieCanvas),
            new FrameworkPropertyMetadata(typeof(DieCanvas)));
    }

    public DieCanvas()
    {
        _shapeManager = new ShapeManager();
        _previewManager = new PreviewManager();

        InitializeCommands();
        InitializeInputBindings();
    }

    #endregion Constructors

    #region Fields

    public static readonly DependencyProperty CancelCommandProperty =
        DependencyProperty.Register(
            nameof(CancelCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty ClearSelectionCommandProperty =
        DependencyProperty.Register(
            nameof(ClearSelectionCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty ContextMenuCommandProperty =
        DependencyProperty.Register(
            nameof(ContextMenuCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register(
            nameof(DeleteCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty DrawingCommandProperty =
        DependencyProperty.Register(
            nameof(DrawingCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty EndDrawingCommandProperty =
        DependencyProperty.Register(
            nameof(EndDrawingCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty MaxScaleProperty =
        DependencyProperty.Register(
            nameof(MaxScale),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(10.0));

    public static readonly DependencyProperty MinScaleProperty =
     DependencyProperty.Register(
         nameof(MinScale),
         typeof(double),
         typeof(DieCanvas),
         new PropertyMetadata(0.1));

    public static readonly DependencyProperty MoveCommandProperty =
        DependencyProperty.Register(
            nameof(MoveCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty PanEndCommandProperty =
        DependencyProperty.Register(
            nameof(PanEndCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty PanningCommandProperty =
        DependencyProperty.Register(
            nameof(PanningCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty PanStartCommandProperty =
        DependencyProperty.Register(
            nameof(PanStartCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty ScaleValueProperty =
        DependencyProperty.Register(
            nameof(ScaleValue),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(1.0));

    public static readonly DependencyProperty SelectAllCommandProperty =
        DependencyProperty.Register(
            nameof(SelectAllCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty SelectedShapeProperty =
        DependencyProperty.Register(
            nameof(SelectedShape),
            typeof(DieShape),
            typeof(DieCanvas),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedShapeChanged));

    public static readonly DependencyProperty SelectShapeCommandProperty =
        DependencyProperty.Register(
            nameof(SelectShapeCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty ShapesProperty =
        DependencyProperty.Register(
            nameof(Shapes),
            typeof(ObservableCollection<DieShape>),
            typeof(DieCanvas),
            new PropertyMetadata(null, OnShapesChanged));

    public static readonly DependencyProperty StartDrawingCommandProperty =
        DependencyProperty.Register(
            nameof(StartDrawingCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty StatusTextProperty =
        DependencyProperty.Register(
            nameof(StatusText),
            typeof(string),
            typeof(DieCanvas),
            new PropertyMetadata("點擊空白處開始繪製，或點擊圖形進行編輯"));

    public static readonly DependencyProperty XDiePitchProperty =
        DependencyProperty.Register(
            nameof(XDiePitch),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(0d));

    public static readonly DependencyProperty XOffsetProperty =
            DependencyProperty.Register(
            nameof(XOffset),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(0d));

    public static readonly DependencyProperty YDiePitchProperty =
        DependencyProperty.Register(
            nameof(YDiePitch),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(0d));

    public static readonly DependencyProperty YOffsetProperty =
            DependencyProperty.Register(
            nameof(YOffset),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(0d));

    public static readonly DependencyProperty ZoomCommandProperty =
        DependencyProperty.Register(
            nameof(ZoomCommand),
            typeof(ICommand),
            typeof(DieCanvas));

    public static readonly DependencyProperty ZoomFactorProperty =
        DependencyProperty.Register(
            nameof(ZoomFactor),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(1.2));

    private readonly PreviewManager _previewManager;

    private readonly ShapeManager _shapeManager;

    private ItemsControl? _contentCanvas;

    private EditMode _currentMode;

    private Canvas? _drawingCanvas;

    private Point _startPoint;

    #endregion Fields

    #region Events

    public event EventHandler<OffsetChangedEventArgs>? OffsetChanged;

    public event EventHandler<ScaleChangedEventArgs>? ScaleChanged;

    #endregion Events

    #region Properties

    public ICommand CancelCommand
    {
        get => (ICommand)GetValue(CancelCommandProperty);
        private set => SetValue(CancelCommandProperty, value);
    }

    public ICommand ClearSelectionCommand
    {
        get => (ICommand)GetValue(ClearSelectionCommandProperty);
        private set => SetValue(ClearSelectionCommandProperty, value);
    }

    public ICommand ContextMenuCommand
    {
        get => (ICommand)GetValue(ContextMenuCommandProperty);
        set => SetValue(ContextMenuCommandProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        private set => SetValue(DeleteCommandProperty, value);
    }

    public ICommand DrawingCommand
    {
        get => (ICommand)GetValue(DrawingCommandProperty);
        private set => SetValue(DrawingCommandProperty, value);
    }

    public ICommand EndDrawingCommand
    {
        get => (ICommand)GetValue(EndDrawingCommandProperty);
        private set => SetValue(EndDrawingCommandProperty, value);
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

    public ICommand MoveCommand
    {
        get => (ICommand)GetValue(MoveCommandProperty);
        private set => SetValue(MoveCommandProperty, value);
    }

    public ICommand PanEndCommand
    {
        get => (ICommand)GetValue(PanEndCommandProperty);
        private set => SetValue(PanEndCommandProperty, value);
    }

    public ICommand PanningCommand
    {
        get => (ICommand)GetValue(PanningCommandProperty);
        private set => SetValue(PanningCommandProperty, value);
    }

    public ICommand PanStartCommand
    {
        get => (ICommand)GetValue(PanStartCommandProperty);
        private set => SetValue(PanStartCommandProperty, value);
    }

    public double ScaleValue
    {
        get => (double)GetValue(ScaleValueProperty);
        set => SetValue(ScaleValueProperty, value);
    }

    public ICommand SelectAllCommand
    {
        get => (ICommand)GetValue(SelectAllCommandProperty);
        private set => SetValue(SelectAllCommandProperty, value);
    }

    public DieShape SelectedShape
    {
        get => (DieShape)GetValue(SelectedShapeProperty);
        set => SetValue(SelectedShapeProperty, value);
    }

    public ICommand SelectShapeCommand
    {
        get => (ICommand)GetValue(SelectShapeCommandProperty);
        private set => SetValue(SelectShapeCommandProperty, value);
    }

    public ObservableCollection<DieShape> Shapes
    {
        get => (ObservableCollection<DieShape>)GetValue(ShapesProperty);
        set => SetValue(ShapesProperty, value);
    }

    public ICommand StartDrawingCommand
    {
        get => (ICommand)GetValue(StartDrawingCommandProperty);
        private set => SetValue(StartDrawingCommandProperty, value);
    }

    public string StatusText
    {
        get => (string)GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public double XDiePitch
    {
        get => (double)GetValue(XDiePitchProperty);
        set => SetValue(XDiePitchProperty, value);
    }

    public double XOffset
    {
        get => (double)GetValue(XOffsetProperty);
        set => SetValue(XOffsetProperty, value);
    }

    public double YDiePitch
    {
        get => (double)GetValue(YDiePitchProperty);
        set => SetValue(YDiePitchProperty, value);
    }

    public double YOffset
    {
        get => (double)GetValue(YOffsetProperty);
        set => SetValue(YOffsetProperty, value);
    }

    public ICommand ZoomCommand
    {
        get => (ICommand)GetValue(ZoomCommandProperty);
        private set => SetValue(ZoomCommandProperty, value);
    }

    public double ZoomFactor
    {
        get => (double)GetValue(ZoomFactorProperty);
        set => SetValue(ZoomFactorProperty, value);
    }

    #endregion Properties

    #region Methods

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _drawingCanvas = GetTemplateChild("PART_WorkArea") as Canvas;
        _contentCanvas = GetTemplateChild("PART_ShapesContainer") as ItemsControl;
    }

    public void SelectShape(DieShape? shape)
    {
        if (shape != null)
        {
            SelectedShape = shape;
        }
    }

    protected virtual void OnOffsetChanged(Point oldOffset, Point newOffset)
    {
        OffsetChanged?.Invoke(this, new OffsetChangedEventArgs(oldOffset, newOffset));
    }

    protected virtual void OnScaleChanged(double oldScale, double newScale)
    {
        ScaleChanged?.Invoke(this, new ScaleChangedEventArgs(oldScale, newScale));
    }

    private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DieCanvas canvas &&
            canvas.GetValue(XOffsetProperty) is double x &&
            canvas.GetValue(YOffsetProperty) is double y)
        {
            var oldOffset = new Point(
                e.Property == XOffsetProperty ? (double)e.OldValue : x,
                e.Property == YOffsetProperty ? (double)e.OldValue : y
            );

            var newOffset = new Point(
                e.Property == XOffsetProperty ? (double)e.NewValue : x,
                e.Property == YOffsetProperty ? (double)e.NewValue : y
            );

            canvas.OnOffsetChanged(oldOffset, newOffset);
        }
    }

    private static void OnScaleValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DieCanvas canvas)
        {
            canvas.OnScaleChanged(
                (double)e.OldValue,
                (double)e.NewValue);
        }
    }
    private static void OnSelectedShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var dieCanvas = (DieCanvas)d;
        var newValue = e.NewValue as DieShape;
        var oldValue = e.OldValue as DieShape;

        if (newValue == oldValue) return;

        if (oldValue != null)
        {
            oldValue.IsSelected = false;
        }

        if (newValue != null)
        {
            newValue.IsSelected = true;
            dieCanvas._shapeManager.BringToFront(newValue);
        }

        if (dieCanvas.Shapes != null)
        {
            foreach (var shape in dieCanvas.Shapes)
            {
                if (shape != newValue)
                {
                    shape.IsSelected = false;
                }
            }
        }

        CommandManager.InvalidateRequerySuggested();
    }

    private static void OnShapesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DieCanvas canvas)
        {
            if (e.OldValue is ObservableCollection<DieShape> oldShapes)
            {
                oldShapes.CollectionChanged -= canvas.Shapes_CollectionChanged;
            }

            if (e.NewValue is ObservableCollection<DieShape> newShapes)
            {
                canvas._shapeManager.Shapes = newShapes;
                newShapes.CollectionChanged += canvas.Shapes_CollectionChanged;
            }
        }
    }

    private void Cancel()
    {
        if (_currentMode == EditMode.Drawing)
        {
            try
            {
                _previewManager.EndPreview(_drawingCanvas);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cancelling drawing: {ex.Message}");
            }
        }

        _currentMode = EditMode.None;
        ClearAllSelections();
        UpdateStatusText();
    }

    private bool CanDeleteSelected()
    {
        return SelectedShape != null && Shapes?.Contains(SelectedShape) == true;
    }

    private bool CanMoveSelected(string? direction) => SelectedShape != null;

    private void ClearAllSelections()
    {
        SelectedShape = null;

        if (Shapes != null)
        {
            foreach (var shape in Shapes)
            {
                shape.IsSelected = false;
            }
        }

        if (_contentCanvas != null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(_contentCanvas);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(_contentCanvas);
                if (adorners != null)
                {
                    foreach (var adorner in adorners)
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
                adornerLayer.Update();
            }
        }
    }

    private void ClearSelection(Point point)
    {
        if (_currentMode != EditMode.Drawing)
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
        if (_currentMode == EditMode.Drawing)
        {
            _previewManager.UpdatePreview(currentPoint);
        }
    }

    private void EndDrawing(Point endPoint)
    {
        if (_currentMode == EditMode.Drawing)
        {
            try
            {
                double width = Math.Abs(endPoint.X - _startPoint.X);
                double height = Math.Abs(endPoint.Y - _startPoint.Y);

                _previewManager.EndPreview(_drawingCanvas);

                if (width > 5 && height > 5)
                {
                    SelectedShape = _shapeManager.CreateShape(_startPoint, endPoint);
                }

                _currentMode = EditMode.None;
                UpdateStatusText();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ending drawing: {ex.Message}");
                _currentMode = EditMode.None;
            }
        }
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
        PanStartCommand = new RelayCommand<MouseButtonEventArgs>(OnPanStart);
        PanningCommand = new RelayCommand<Vector>(OnPanning);
        PanEndCommand = new RelayCommand<MouseButtonEventArgs>(OnPanEnd);
        ZoomCommand = new RelayCommand<MouseWheelEventArgs>(OnZoom);

        if (Shapes != null)
        {
            Shapes.CollectionChanged += Shapes_CollectionChanged;
        }
    }

    private void InitializeInputBindings()
    {
        InputBindings.Add(new KeyBinding(DeleteCommand, Key.Delete, ModifierKeys.None));
        InputBindings.Add(new KeyBinding(CancelCommand, Key.Escape, ModifierKeys.None));
        InputBindings.Add(new KeyBinding(SelectAllCommand, Key.A, ModifierKeys.Control));

        InputBindings.Add(new KeyBinding(MoveCommand, Key.Left, ModifierKeys.None)
        { CommandParameter = "Left" });
        InputBindings.Add(new KeyBinding(MoveCommand, Key.Right, ModifierKeys.None)
        { CommandParameter = "Right" });
        InputBindings.Add(new KeyBinding(MoveCommand, Key.Up, ModifierKeys.None)
        { CommandParameter = "Up" });
        InputBindings.Add(new KeyBinding(MoveCommand, Key.Down, ModifierKeys.None)
        { CommandParameter = "Down" });
    }

    private void MoveSelected(string? direction)
    {
        if (SelectedShape == null) return;

        var delta = 1.0;
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
        // Handle pan end
    }

    private void OnPanning(Vector delta)
    {
        XOffset += delta.X;
        YOffset += delta.Y;
    }

    private void OnPanStart(MouseButtonEventArgs e)
    {
        // Handle pan start
    }

    private void OnZoom(MouseWheelEventArgs e)
    {
        e.Handled = true;

        var mousePosition = e.GetPosition(_drawingCanvas);
        var zoomFactor = e.Delta > 0 ? 1.2 : 1 / 1.2;
        var newScale = Math.Clamp(ScaleValue * zoomFactor, 0.1, 10.0);

        if (Math.Abs(newScale - ScaleValue) < 0.001) return;

        var oldX = XOffset;
        var oldY = YOffset;

        XOffset = mousePosition.X - (mousePosition.X - oldX) * newScale / ScaleValue;
        YOffset = mousePosition.Y - (mousePosition.Y - oldY) * newScale / ScaleValue;
        ScaleValue = newScale;
    }

    private void SelectAll()
    {
        if (Shapes == null) return;

        foreach (var shape in Shapes)
        {
            shape.IsSelected = true;
        }
    }

    private void Shapes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();

        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems?.Contains(SelectedShape) == true)
            {
                SelectedShape = null;
            }
        }
    }

    private void StartDrawing(Point point)
    {
        try
        {
            ClearAllSelections();

            _currentMode = EditMode.Drawing;
            _startPoint = point;

            if (_drawingCanvas != null)
            {
                _previewManager.StartPreview(_drawingCanvas, point, ScaleValue);
            }

            UpdateStatusText();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error starting drawing: {ex.Message}");
            _currentMode = EditMode.None;
        }
    }

    private void UpdateStatusText()
    {
        StatusText = _currentMode switch
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