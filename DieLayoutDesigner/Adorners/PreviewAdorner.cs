using System.Windows;
using System.Windows.Media;

namespace DieLayoutDesigner.Adorners;

public class PreviewAdorner : ScaleAwareAdorner
{

    #region Constructors

    public PreviewAdorner(UIElement adornedElement, Point startPoint, double scaleValue)
         : base(adornedElement, scaleValue)
    {
        _newStartPoint = new Point(
            startPoint.X / _scaleValue,
            startPoint.Y / _scaleValue
        );
        _currentPoint = _newStartPoint;

        IsHitTestVisible = false;
    }

    #endregion Constructors

    #region Fields

    private Point _currentPoint;
    private Point _newStartPoint;

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
            Math.Min(_newStartPoint.X, _currentPoint.X),
            Math.Min(_newStartPoint.Y, _currentPoint.Y),
            Math.Abs(_currentPoint.X - _newStartPoint.X),
            Math.Abs(_currentPoint.Y - _newStartPoint.Y));

        var fillBrush = new SolidColorBrush(Colors.Blue) { Opacity = 0.3 };
        var pen = new Pen();

        drawingContext.PushTransform(new ScaleTransform(_scaleValue, _scaleValue));
        drawingContext.DrawRectangle(fillBrush, pen, rect);
        drawingContext.Pop();
    }

    #endregion Methods

}