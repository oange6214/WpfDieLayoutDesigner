using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DieLayoutDesigner.Adorners;

public class PreviewAdorner : Adorner
{

    public PreviewAdorner(UIElement adornedElement, Point startPoint)
        : base(adornedElement)
    {
        _startPoint = startPoint;
        _currentPoint = startPoint;

        _fillBrush = new SolidColorBrush(Colors.Blue)
        {
            Opacity = 0.3
        };
        _strokePen = new Pen(Brushes.Black, 1)
        {
            DashStyle = new DashStyle([2, 2], 0)
        };

        IsHitTestVisible = false;
    }

    private readonly Brush _fillBrush;
    private readonly Pen _strokePen;
    private Point _currentPoint;
    private Point _startPoint;

    public void UpdatePosition(Point currentPoint)
    {
        _currentPoint = currentPoint;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var rect = new Rect(
            Math.Min(_startPoint.X, _currentPoint.X),
            Math.Min(_startPoint.Y, _currentPoint.Y),
            Math.Abs(_currentPoint.X - _startPoint.X),
            Math.Abs(_currentPoint.Y - _startPoint.Y));

        drawingContext.DrawRectangle(_fillBrush, _strokePen, rect);
    }

}