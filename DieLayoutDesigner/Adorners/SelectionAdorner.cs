using System.Windows;
using System.Windows.Media;

namespace DieLayoutDesigner.Adorners;

public class SelectionAdorner : ScaleAwareAdorner
{
    private const double _baseThicknessSize = 1.0;

    public SelectionAdorner(UIElement adornedElement, double scaleValue)
        : base(adornedElement, scaleValue)
    {
        IsHitTestVisible = false;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var rect = new Rect(AdornedElement.RenderSize);

        var thickness = GetScaledThickness(_baseThicknessSize);

        var pen = new Pen(new SolidColorBrush(Color.FromRgb(18, 143, 234)), thickness);

        drawingContext.PushTransform(new ScaleTransform(_scaleValue, _scaleValue));
        drawingContext.DrawRectangle(null, pen, new Rect(
            rect.X / _scaleValue,
            rect.Y / _scaleValue,
            rect.Width / _scaleValue,
            rect.Height / _scaleValue));
        drawingContext.Pop();
    }
}