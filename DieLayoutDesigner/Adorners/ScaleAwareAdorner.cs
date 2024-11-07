using System.Windows;
using System.Windows.Documents;

namespace DieLayoutDesigner.Adorners;

public abstract class ScaleAwareAdorner : Adorner
{
    protected readonly double _scaleValue;

    protected ScaleAwareAdorner(UIElement adornedElement, double scaleValue)
        : base(adornedElement)
    {
        _scaleValue = scaleValue;
    }

    protected double GetScaledThickness(double baseThickness)
    {
        return baseThickness / _scaleValue;
    }

    protected double GetScaledThumbSize(double baseSize)
    {
        const double minThumbSize = 4.0;
        const double adjustmentFactor = 1.2;

        double scaledSize = baseSize / Math.Sqrt(_scaleValue) * adjustmentFactor;
        return Math.Max(scaledSize, minThumbSize);
    }
}