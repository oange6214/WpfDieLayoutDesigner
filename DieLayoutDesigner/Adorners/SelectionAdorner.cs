using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DieLayoutDesigner.Adorners;

public class SelectionAdorner : Adorner
{
    #region Constructors

    public SelectionAdorner(UIElement adornedElement) : base(adornedElement)
    {
        IsHitTestVisible = false;
    }

    #endregion Constructors

    #region Methods

    protected override void OnRender(DrawingContext drawingContext)
    {
        var rect = new Rect(AdornedElement.RenderSize);
        var pen = new Pen(Brushes.Blue, 2);
        pen.DashStyle = new DashStyle(new double[] { 2, 2 }, 0);
        drawingContext.DrawRectangle(null, pen, rect);
    }

    #endregion Methods
}