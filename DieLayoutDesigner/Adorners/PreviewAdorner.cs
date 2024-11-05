using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DieLayoutDesigner.Adorners;

public class PreviewAdorner : Adorner
{
    #region Constructors

    public PreviewAdorner(UIElement adornedElement, Point startPoint, double scaleValue)
        : base(adornedElement)
    {
        _scaleValue = scaleValue;

        _startPoint = new Point(
            startPoint.X / _scaleValue,
            startPoint.Y / _scaleValue
        );
        _currentPoint = _startPoint;

        _fillBrush = new SolidColorBrush(Colors.Blue) { Opacity = 0.3 };
        _strokePen = new Pen(Brushes.Black, 1 / _scaleValue)
        {
            DashStyle = new DashStyle([2 / _scaleValue, 2 / _scaleValue], 0)
        };

        IsHitTestVisible = false;
    }

    #endregion Constructors

    #region Fields

    private readonly Brush _fillBrush;
    private readonly double _scaleValue;
    private readonly Pen _strokePen;
    private Point _currentPoint;
    private Point _startPoint;

    #endregion Fields

    #region Methods

    public void UpdatePosition(Point currentPoint)
    {
        _currentPoint = new Point(
            currentPoint.X / _scaleValue,
            currentPoint.Y / _scaleValue
        );
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var rect = new Rect(
            Math.Min(_startPoint.X, _currentPoint.X),
            Math.Min(_startPoint.Y, _currentPoint.Y),
            Math.Abs(_currentPoint.X - _startPoint.X),
            Math.Abs(_currentPoint.Y - _startPoint.Y));

        drawingContext.PushTransform(new ScaleTransform(_scaleValue, _scaleValue));
        drawingContext.DrawRectangle(_fillBrush, _strokePen, rect);
        drawingContext.Pop();
    }

    #endregion Methods
}