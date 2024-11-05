using System.Windows;

namespace DieLayoutDesigner.Behaviors;

public interface ICoordinateSystem
{
    Point ToLogical(Point visualPosition);
    Point ToVisual(Point logicalPosition);
}