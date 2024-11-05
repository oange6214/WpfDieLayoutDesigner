using DieLayoutDesigner.Adorners;
using DieLayoutDesigner.Models;
using DieLayoutDesigner.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace DieLayoutDesigner.Behaviors;

public class ShapeEditBehavior : Behavior<Rectangle>
{
    #region Fields

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected),
        typeof(bool),
        typeof(ShapeEditBehavior),
        new PropertyMetadata(false, OnIsSelectedChanged)
    );

    private AdornerLayer? _adornerLayer;
    private bool _isDragging;
    private ResizeAdorner? _resizeAdorner;
    private SelectionAdorner? _selectionAdorner;
    private Point _startPoint;

    #endregion Fields

    #region Properties

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    #endregion Properties

    #region Methods

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;

        _adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;

        RemoveAdorners();
        base.OnDetaching();
    }

    private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShapeEditBehavior behavior)
        {
            behavior.UpdateAdorners();
        }
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (IsSelected)
        {
            UpdateAdorners();
        }
    }
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (AssociatedObject.DataContext is DieShape shape &&
            Window.GetWindow(AssociatedObject)?.DataContext is MainWindowViewModel vm)
        {
            e.Handled = true;
            _isDragging = true;
            _startPoint = e.GetPosition(AssociatedObject.Parent as UIElement);
            AssociatedObject.CaptureMouse();
            vm.SelectShape(shape);
        }
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            AssociatedObject.ReleaseMouseCapture();
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && AssociatedObject.DataContext is DieShape shape)
        {
            var currentPoint = e.GetPosition(AssociatedObject.Parent as UIElement);
            var delta = currentPoint - _startPoint;

            shape.TopLeft = new Point(
                shape.TopLeft.X + delta.X,
                shape.TopLeft.Y + delta.Y
            );

            _startPoint = currentPoint;
        }
    }

    private void RemoveAdorners()
    {
        if (_adornerLayer != null)
        {
            if (_selectionAdorner != null)
                _adornerLayer.Remove(_selectionAdorner);
            if (_resizeAdorner != null)
                _adornerLayer.Remove(_resizeAdorner);
        }
    }

    private void UpdateAdorners()
    {
        RemoveAdorners();

        if (IsSelected && _adornerLayer != null)
        {
            _selectionAdorner = new SelectionAdorner(AssociatedObject);
            _resizeAdorner = new ResizeAdorner(AssociatedObject);
            _adornerLayer.Add(_selectionAdorner);
            _adornerLayer.Add(_resizeAdorner);
        }
    }

    #endregion Methods
}