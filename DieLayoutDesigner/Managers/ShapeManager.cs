using DieLayoutDesigner.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace DieLayoutDesigner.Managers;

public class ShapeManager
{
    private int _currentMaxZIndex;
    public ObservableCollection<DieShape> Shapes { get; } = [];

    public DieShape CreateShape(Point start, Point end)
    {
        double width = Math.Abs(end.X - start.X);
        double height = Math.Abs(end.Y - start.Y);
        double left = Math.Min(end.X, start.X);
        double top = Math.Min(end.Y, start.Y);

        _currentMaxZIndex++;

        var newShape = new DieShape
        {
            TopLeft = new Point(left, top),
            DieSize = new Size(width, height),
            Data = new RectangleGeometry(new Rect(0, 0, width, height)),
            FillColor = new SolidColorBrush(Color.FromRgb(
                (byte)Random.Shared.Next(256),
                (byte)Random.Shared.Next(256),
                (byte)Random.Shared.Next(256))),
            ZIndex = _currentMaxZIndex,
            IsSelected = true
        };

        Shapes.Add(newShape);
        return newShape;
    }

    public void BringToFront(DieShape shape)
    {
        if (shape != null)
        {
            _currentMaxZIndex++;
            shape.ZIndex = _currentMaxZIndex;
        }
    }

    public void DeleteShape(DieShape shape)
    {
        Shapes.Remove(shape);
    }
}