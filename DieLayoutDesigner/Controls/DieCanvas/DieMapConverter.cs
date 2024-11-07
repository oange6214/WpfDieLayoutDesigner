namespace DieLayoutDesigner.Controls;

public class DieMapConverter
{
    // 假設 1 微米(μm) = 1 DIU
    private const double MicronsPerDiu = 1.0;

    public static double MicronsToPixels(double microns)
    {
        return microns * MicronsPerDiu;
    }

    public static double PixelsToMicrons(double pixels)
    {
        return pixels / MicronsPerDiu;
    }
}