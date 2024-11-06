namespace DieLayoutDesigner.Controls;

public class ScaleChangedEventArgs : EventArgs
{
    public double OldScale { get; }
    public double NewScale { get; }

    public ScaleChangedEventArgs(double oldScale, double newScale)
    {
        OldScale = oldScale;
        NewScale = newScale;
    }
}