using DieLayoutDesigner.Models;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DieLayoutDesigner.Adorners;

public class ResizeAdorner : Adorner
{
    #region Constructors

    public ResizeAdorner(Rectangle adornerElement) : base(adornerElement)
    {
        _adornerElement = adornerElement;
        _visualChildren = new VisualCollection(this);
        CreateThumbs();
    }

    #endregion Constructors

    #region Fields

    private readonly Rectangle _adornerElement;
    private readonly List<Thumb> _thumbs = new();
    private readonly VisualCollection _visualChildren;

    #endregion Fields

    #region Properties

    protected override int VisualChildrenCount => _visualChildren.Count;

    #endregion Properties

    #region Methods

    protected override Size ArrangeOverride(Size finalSize)
    {
        var rect = new Rect(finalSize);

        foreach (var thumb in _thumbs)
        {
            var index = _thumbs.IndexOf(thumb);
            var position = GetThumbPosition(index, rect);
            thumb.Arrange(new Rect(position.X - 4, position.Y - 4, 8, 8));
        }

        return finalSize;
    }

    protected override Visual GetVisualChild(int index) => _visualChildren[index];

    private void CreateThumbs()
    {
        var positions = new[]
        {
            ("TopLeft", Cursors.SizeNWSE),
            ("TopRight", Cursors.SizeNESW),
            ("BottomLeft", Cursors.SizeNESW),
            ("BottomRight", Cursors.SizeNWSE)
        };

        foreach (var (position, cursor) in positions)
        {
            var thumb = new Thumb
            {
                Style = Application.Current.Resources["ModernResizeThumbStyle"] as Style,
                Cursor = cursor
            };
            thumb.DragDelta += (s, e) => OnThumbDragDelta(position, e);
            _thumbs.Add(thumb);
            _visualChildren.Add(thumb);
        }
    }
    private Point GetThumbPosition(int index, Rect rect)
    {
        return index switch
        {
            0 => new Point(rect.Left, rect.Top), // TopLeft
            1 => new Point(rect.Right, rect.Top), // TopRight
            2 => new Point(rect.Left, rect.Bottom), // BottomLeft
            3 => new Point(rect.Right, rect.Bottom), // BottomRight
            _ => new Point()
        };
    }

    private void OnThumbDragDelta(string position, DragDeltaEventArgs e)
    {
        if (_adornerElement.DataContext is not DieShape shape) return;

        var deltaX = e.HorizontalChange;
        var deltaY = e.VerticalChange;
        var topLeft = shape.TopLeft;
        var size = shape.DieSize;

        switch (position)
        {
            case "TopLeft":
                shape.TopLeft = new Point(
                    topLeft.X + deltaX,
                    topLeft.Y + deltaY);
                shape.DieSize = new Size(
                    Math.Max(20, size.Width - deltaX),
                    Math.Max(20, size.Height - deltaY));
                break;

            case "TopRight":
                shape.TopLeft = new Point(
                    topLeft.X,
                    topLeft.Y + deltaY);
                shape.DieSize = new Size(
                    Math.Max(20, size.Width + deltaX),
                    Math.Max(20, size.Height - deltaY));
                break;

            case "BottomLeft":
                shape.TopLeft = new Point(
                    topLeft.X + deltaX,
                    topLeft.Y);
                shape.DieSize = new Size(
                    Math.Max(20, size.Width - deltaX),
                    Math.Max(20, size.Height + deltaY));
                break;

            case "BottomRight":
                shape.DieSize = new Size(
                    Math.Max(20, size.Width + deltaX),
                    Math.Max(20, size.Height + deltaY));
                break;
        }
    }

    #endregion Methods
}