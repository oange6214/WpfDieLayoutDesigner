using DieLayoutDesigner.Models;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DieLayoutDesigner.Adorners;

public class ResizeAdorner : ScaleAwareAdorner
{
    #region Constructors

    public ResizeAdorner(Rectangle adornerElement, double scaleValue)
           : base(adornerElement, scaleValue)
    {
        _adornerElement = adornerElement;
        _visualChildren = new VisualCollection(this);
        CreateThumbs();
    }

    #endregion Constructors

    #region Fields

    private const double _minSize = 1.0;
    private const double _baseThumbSize = 2.0;
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
        var scaledThumbSize = GetScaledThumbSize(_baseThumbSize);

        foreach (var thumb in _thumbs)
        {
            var index = _thumbs.IndexOf(thumb);
            var position = GetThumbPosition(index, rect);

            thumb.Width = scaledThumbSize;
            thumb.Height = scaledThumbSize;

            thumb.Arrange(new Rect(
                position.X - scaledThumbSize / 2,
                position.Y - scaledThumbSize / 2,
                scaledThumbSize,
                scaledThumbSize));
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

        var scaledThumbSize = GetScaledThumbSize(_baseThumbSize);

        foreach (var (position, cursor) in positions)
        {
            var thumb = new Thumb
            {
                Width = scaledThumbSize,
                Height = scaledThumbSize,
                Style = Application.Current.Resources["ResizeThumbStyle"] as Style,
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
            0 => new Point(rect.Left, rect.Top),
            1 => new Point(rect.Right, rect.Top),
            2 => new Point(rect.Left, rect.Bottom),
            3 => new Point(rect.Right, rect.Bottom),
            _ => new Point()
        };
    }

    private void OnThumbDragDelta(string position, DragDeltaEventArgs e)
    {
        if (_adornerElement.DataContext is not DieShape shape)
            return;

        var deltaX = e.HorizontalChange;
        var deltaY = e.VerticalChange;
        var topLeft = shape.TopLeft;
        var size = shape.DieSize;

        var newSize = size;
        var newTopLeft = topLeft;

        switch (position)
        {
            case "TopLeft":
                newSize = new Size(
                    Math.Max(_minSize, size.Width - deltaX),
                    Math.Max(_minSize, size.Height - deltaY)
                );

                newTopLeft = new Point(
                    topLeft.X + (size.Width - newSize.Width),
                    topLeft.Y + (size.Height - newSize.Height)
                );
                break;

            case "TopRight":
                newSize = new Size(
                    Math.Max(_minSize, size.Width + deltaX),
                    Math.Max(_minSize, size.Height - deltaY)
                );

                newTopLeft = new Point(
                    topLeft.X,
                    topLeft.Y + (size.Height - newSize.Height)
                );
                break;

            case "BottomLeft":
                newSize = new Size(
                    Math.Max(_minSize, size.Width - deltaX),
                    Math.Max(_minSize, size.Height + deltaY)
                );

                newTopLeft = new Point(
                    topLeft.X + (size.Width - newSize.Width),
                    topLeft.Y
                );
                break;

            case "BottomRight":
                newSize = new Size(
                    Math.Max(_minSize, size.Width + deltaX),
                    Math.Max(_minSize, size.Height + deltaY)
                );

                newTopLeft = topLeft;
                break;
        }

        if (newSize != size)
        {
            shape.DieSize = newSize;
            shape.TopLeft = newTopLeft;
        }
    }

    #endregion Methods
}