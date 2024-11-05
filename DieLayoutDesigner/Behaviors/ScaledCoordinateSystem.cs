using System.Windows;

namespace DieLayoutDesigner.Behaviors;

public class ScaledCoordinateSystem : ICoordinateSystem
{
    #region Constructors

    public ScaledCoordinateSystem(double scale, Point offset)
    {
        _scale = scale;
        _offset = offset;
    }

    #endregion Constructors

    #region Fields

    private readonly Point _offset;
    private readonly double _scale;

    #endregion Fields

    #region Methods

    public Point ToLogical(Point visualPosition)
    {
        return new Point(
            (visualPosition.X - _offset.X) / _scale,
            (visualPosition.Y - _offset.Y) / _scale
        );
    }

    public Point ToVisual(Point logicalPosition)
    {
        return new Point(
            logicalPosition.X * _scale + _offset.X,
            logicalPosition.Y * _scale + _offset.Y
        );
    }

    #endregion Methods
}