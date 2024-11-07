using DieLayoutDesigner.Enums;
using DieLayoutDesigner.Managers;
using DieLayoutDesigner.Models;
using DieLayoutDesigner.MvvmToolKit.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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

    public static readonly DependencyProperty DieHeightProperty =
        DependencyProperty.Register(
            nameof(DieHeight),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(0.0d));

    public static readonly DependencyProperty DieWidthProperty =
        DependencyProperty.Register(
            nameof(DieWidth),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(0.0d));

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

    public static readonly DependencyProperty GridColorProperty =
        DependencyProperty.Register(
            nameof(GridColor),
            typeof(Color),
            typeof(DieCanvas),
            new PropertyMetadata(Colors.Gray));

    public static readonly DependencyProperty GridVisibleProperty =
        DependencyProperty.Register(
            nameof(GridVisible),
            typeof(bool),
            typeof(DieCanvas),
            new PropertyMetadata(true));

    public static readonly DependencyProperty MaxScaleProperty =
        DependencyProperty.Register(
            nameof(MaxScale),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(10.0d));

    public static readonly DependencyProperty MinScaleProperty =
         DependencyProperty.Register(
             nameof(MinScale),
             typeof(double),
             typeof(DieCanvas),
             new PropertyMetadata(1.0d));

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
            new PropertyMetadata(1.0d));

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

    public static readonly DependencyProperty SelectionModeProperty =
        DependencyProperty.Register(
            nameof(SelectionMode),
            typeof(DieSelectionMode),
            typeof(DieCanvas),
            new PropertyMetadata(DieSelectionMode.Single));

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

    public static readonly DependencyProperty ShowCoordinatesProperty =
        DependencyProperty.Register(
            nameof(ShowCoordinates),
            typeof(bool),
            typeof(DieCanvas),
            new PropertyMetadata(true));

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
            new PropertyMetadata("Click on an empty space to start drawing, or click on the graphic to edit it"));
    
    public static readonly DependencyProperty XOffsetProperty =
            DependencyProperty.Register(
            nameof(XOffset),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(0.0d, OnOffsetChanged));

    public static readonly DependencyProperty YOffsetProperty =
            DependencyProperty.Register(
            nameof(YOffset),
            typeof(double),
            typeof(DieCanvas),
            new PropertyMetadata(0.0d, OnOffsetChanged));

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
            new PropertyMetadata(1.2d));

    private readonly PreviewManager _previewManager;

    private readonly List<Point> _selectedDies = new();
    private readonly ShapeManager _shapeManager;

    private EditMode _currentMode;
    private bool _isRegionSelecting;
    private Point? _regionEndDie;
    private Point _regionStartDie;
    private ItemsControl? _shapesContainer;
    private Point _startPoint;
    private Canvas? _workArea;

    #endregion Fields

    #region Events

    public event EventHandler<DiePositionEventArgs>? DiePositionChanged;

    public event EventHandler<DieSelectionEventArgs>? DieSelectionChanged;

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

    public double DieHeight
    {
        get => (double)GetValue(DieHeightProperty);
        set => SetValue(DieHeightProperty, value);
    }

    public double DieWidth
    {
        get => (double)GetValue(DieWidthProperty);
        set => SetValue(DieWidthProperty, value);
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

    public Color GridColor
    {
        get => (Color)GetValue(GridColorProperty);
        set => SetValue(GridColorProperty, value);
    }

    public bool GridVisible
    {
        get => (bool)GetValue(GridVisibleProperty);
        set => SetValue(GridVisibleProperty, value);
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

    public DieSelectionMode SelectionMode
    {
        get => (DieSelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
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

    public bool ShowCoordinates
    {
        get => (bool)GetValue(ShowCoordinatesProperty);
        set => SetValue(ShowCoordinatesProperty, value);
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
        _workArea = GetTemplateChild("PART_WorkArea") as Canvas;
        _shapesContainer = GetTemplateChild("PART_ShapesContainer") as ItemsControl;
    }

    public void SelectShape(DieShape? shape)
    {
        if (shape != null)
        {
            SelectedShape = shape;
        }
    }

    protected virtual void OnDieSelectionChanged(DieSelectionEventArgs e)
    {
        DieSelectionChanged?.Invoke(this, e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (SelectionMode == DieSelectionMode.Region)
            {
                StartRegionSelection(e.GetPosition(this));
            }
            else
            {
                HandleDieSelection(e.GetPosition(this));
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (e.LeftButton == MouseButtonState.Pressed && SelectionMode == DieSelectionMode.Region)
        {
            UpdateRegionSelection(e.GetPosition(this));
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (SelectionMode == DieSelectionMode.Region)
        {
            EndRegionSelection();
        }
    }

    protected virtual void OnOffsetChanged(Point oldOffset, Point newOffset)
    {
        OffsetChanged?.Invoke(this, new OffsetChangedEventArgs(oldOffset, newOffset));
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        // 繪製已選取的 Die
        foreach (var die in _selectedDies)
        {
            var rect = new Rect(
                die.X * DieWidth,
                die.Y * DieHeight,
                DieWidth,
                DieHeight);

            drawingContext.DrawRectangle(
                new SolidColorBrush(Color.FromArgb(64, 0, 122, 204)),
                new Pen(Brushes.DodgerBlue, 1),
                rect);
        }

        // 繪製區域選取框
        if (_isRegionSelecting && _regionEndDie.HasValue)
        {
            var rect = new Rect(
                _regionStartDie.X * DieWidth,
                _regionStartDie.Y * DieHeight,
                (_regionEndDie.Value.X - _regionStartDie.X + 1) * DieWidth,
                (_regionEndDie.Value.Y - _regionStartDie.Y + 1) * DieHeight);

            drawingContext.DrawRectangle(
                null,
                new Pen(Brushes.DodgerBlue, 1) { DashStyle = DashStyles.Dash },
                rect);
        }
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
                _previewManager.EndPreview(_workArea);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cancelling drawing: {ex.Message}");
            }
        }

        _currentMode = EditMode.None;
        ClearAllSelections();
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

        if (_shapesContainer != null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(_shapesContainer);
            if (adornerLayer != null)
            {
                var adorners = adornerLayer.GetAdorners(_shapesContainer);
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

                _previewManager.EndPreview(_workArea);

                if (width > 5 && height > 5)
                {
                    SelectedShape = _shapeManager.CreateShape(_startPoint, endPoint);
                }

                _currentMode = EditMode.None;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ending drawing: {ex.Message}");
                _currentMode = EditMode.None;
            }
        }
    }

    private void EndRegionSelection()
    {
        _isRegionSelecting = false;
        _regionEndDie = null;
    }

    private Point GetDiePosition(Point screenPoint)
    {
        // 考慮縮放和偏移的座標轉換
        var transformedPoint = new Point(
            (screenPoint.X - XOffset) / ScaleValue,
            (screenPoint.Y - YOffset) / ScaleValue
        );
        return DieUnit.ToMicrons(transformedPoint);
    }

    private IEnumerable<Point> GetDieRegion(Point start, Point end)
    {
        int minX = (int)Math.Min(start.X, end.X);
        int maxX = (int)Math.Max(start.X, end.X);
        int minY = (int)Math.Min(start.Y, end.Y);
        int maxY = (int)Math.Max(start.Y, end.Y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                yield return new Point(x, y);
            }
        }
    }

    private void HandleDieSelection(Point position)
    {
        var diePosition = DieUnit.ToMicrons(position);
        var dieX = Math.Floor(diePosition.X / DieWidth);
        var dieY = Math.Floor(diePosition.Y / DieHeight);
        var dieIndex = new Point(dieX, dieY);

        var localOffset = new Point(
            diePosition.X % DieWidth,
            diePosition.Y % DieHeight);

        bool isSelected;

        switch (SelectionMode)
        {
            case DieSelectionMode.Single:
                _selectedDies.Clear();
                _selectedDies.Add(dieIndex);
                isSelected = true;
                break;

            case DieSelectionMode.Multiple:
                if (_selectedDies.Contains(dieIndex))
                {
                    _selectedDies.Remove(dieIndex);
                    isSelected = false;
                }
                else
                {
                    _selectedDies.Add(dieIndex);
                    isSelected = true;
                }
                break;

            case DieSelectionMode.Region:
                if (_isRegionSelecting && _regionEndDie.HasValue)
                {
                    var region = GetDieRegion(_regionStartDie, _regionEndDie.Value);
                    _selectedDies.Clear();
                    _selectedDies.AddRange(region);
                    isSelected = true;
                }
                else
                {
                    return;
                }
                break;

            default:
                return;
        }

        OnDieSelectionChanged(new DieSelectionEventArgs(
            dieIndex,
            localOffset,
            SelectionMode,
            isSelected,
            _selectedDies.ToList()));

        InvalidateVisual();
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

        var mousePosition = e.GetPosition(_workArea);
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

            if (_workArea != null)
            {
                _previewManager.StartPreview(_workArea, point, ScaleValue);
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error starting drawing: {ex.Message}");
            _currentMode = EditMode.None;
        }
    }

    private void StartRegionSelection(Point position)
    {
        var diePosition = DieUnit.ToMicrons(position);
        _isRegionSelecting = true;
        _regionStartDie = new Point(
            Math.Floor(diePosition.X / DieWidth),
            Math.Floor(diePosition.Y / DieHeight));
        _regionEndDie = null;
    }

    private void UpdateDiePosition(Point position)
    {
        var diePosition = DieUnit.ToMicrons(position);
        var dieX = Math.Floor(diePosition.X / DieWidth);
        var dieY = Math.Floor(diePosition.Y / DieHeight);

        DiePositionChanged?.Invoke(this, new DiePositionEventArgs(
            new Point(dieX, dieY),
            new Point(diePosition.X % DieWidth, diePosition.Y % DieHeight)));

        if (ShowCoordinates)
            UpdateStatusText(diePosition);
    }

    private void UpdateRegionSelection(Point position)
    {
        if (!_isRegionSelecting) return;

        var diePosition = DieUnit.ToMicrons(position);
        _regionEndDie = new Point(
            Math.Floor(diePosition.X / DieWidth),
            Math.Floor(diePosition.Y / DieHeight));

        HandleDieSelection(position);
    }
    private void UpdateStatusText(Point position)
    {
        var diePosition = GetDiePosition(position);

        int dieX = (int)Math.Floor(diePosition.X / DieWidth);
        int dieY = (int)Math.Floor(diePosition.Y / DieHeight);

        double relativeDieX = diePosition.X % DieWidth;
        double relativeDieY = diePosition.Y % DieHeight;

        StatusText = $"Die({dieX}, {dieY}), Offset({relativeDieX:F2}μm, {relativeDieY:F2}μm)";
    }

    #endregion Methods

}