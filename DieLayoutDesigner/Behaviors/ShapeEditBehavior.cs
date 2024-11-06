using DieLayoutDesigner.Adorners;
using DieLayoutDesigner.Controls;
using DieLayoutDesigner.Models;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DieLayoutDesigner.Behaviors;

public class ShapeEditBehavior : Behavior<Rectangle>
{

    #region Fields

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(
            nameof(IsSelected),
            typeof(bool),
            typeof(ShapeEditBehavior),
            new PropertyMetadata(false, OnIsSelectedChanged));

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
        AssociatedObject.Unloaded += OnUnloaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        AssociatedObject.Unloaded -= OnUnloaded;
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
    
    private DieCanvas? FindParentDieCanvas()
    {
        DependencyObject? current = AssociatedObject;
        while (current != null)
        {
            if (current is DieCanvas canvas)
                return canvas;

            if (current is FrameworkElement fe)
            {
                current = fe.Parent ?? VisualTreeHelper.GetParent(fe);
            }
            else
            {
                current = VisualTreeHelper.GetParent(current);
            }
        }
        return null;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
        if (IsSelected)
        {
            UpdateAdorners();
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (AssociatedObject.DataContext is DieShape shape)
        {
            e.Handled = true;
            _isDragging = true;

            _startPoint = e.GetPosition(AssociatedObject.Parent as UIElement);
            AssociatedObject.CaptureMouse();

            var dieCanvas = FindParentDieCanvas();
            if (dieCanvas != null)
            {
                dieCanvas.SelectedShape = shape;
            }
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
            var dieCanvas = FindParentDieCanvas();
            if (dieCanvas == null) return;

            var currentPoint = e.GetPosition(AssociatedObject.Parent as UIElement);
            var delta = currentPoint - _startPoint;

            var adjustedDelta = new Vector(
                delta.X / dieCanvas.ScaleValue,
                delta.Y / dieCanvas.ScaleValue
            );

            shape.TopLeft = new Point(
                shape.TopLeft.X + adjustedDelta.X,
                shape.TopLeft.Y + adjustedDelta.Y
            );

            _startPoint = currentPoint;
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        RemoveAdorners();
    }

    private void RemoveAdorners()
    {
        if (_adornerLayer != null)
        {
            if (_selectionAdorner != null)
            {
                _adornerLayer.Remove(_selectionAdorner);
                _selectionAdorner = null;
            }
            if (_resizeAdorner != null)
            {
                _adornerLayer.Remove(_resizeAdorner);
                _resizeAdorner = null;
            }

            if (AssociatedObject != null)
            {
                var existingAdorners = _adornerLayer.GetAdorners(AssociatedObject);
                if (existingAdorners != null)
                {
                    foreach (var adorner in existingAdorners)
                    {
                        _adornerLayer.Remove(adorner);
                    }
                }
            }

            _adornerLayer.Update();
        }
    }

    private void UpdateAdorners()
    {
        RemoveAdorners();

        if (IsSelected && _adornerLayer != null && AssociatedObject != null)
        {
            var existingAdorners = _adornerLayer.GetAdorners(AssociatedObject);
            if (existingAdorners != null)
            {
                foreach (var adorner in existingAdorners)
                {
                    _adornerLayer.Remove(adorner);
                }
            }

            _selectionAdorner = new SelectionAdorner(AssociatedObject);
            _resizeAdorner = new ResizeAdorner(AssociatedObject);
            
            _adornerLayer.Add(_selectionAdorner);
            _adornerLayer.Add(_resizeAdorner);
        }

        _adornerLayer?.Update();
    }

    #endregion Methods

}