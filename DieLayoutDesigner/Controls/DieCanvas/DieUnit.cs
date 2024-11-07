using System.Windows;

namespace DieLayoutDesigner.Controls;

public static class DieUnit
{
    // μm（微米）到 DIU 的轉換比例
    private const double MicronsPerDiu = 1.0;

    public static double ToPixels(double microns) => microns * MicronsPerDiu;
    public static double ToMicrons(double pixels) => pixels / MicronsPerDiu;

    public static Point ToPixels(Point micronPoint) =>
        new(ToPixels(micronPoint.X), ToPixels(micronPoint.Y));

    public static Point ToMicrons(Point pixelPoint) =>
        new(ToMicrons(pixelPoint.X), ToMicrons(pixelPoint.Y));

    public static Size ToPixels(Size micronSize) =>
        new(ToPixels(micronSize.Width), ToPixels(micronSize.Height));

    public static Size ToMicrons(Size pixelSize) =>
        new(ToMicrons(pixelSize.Width), ToMicrons(pixelSize.Height));
}