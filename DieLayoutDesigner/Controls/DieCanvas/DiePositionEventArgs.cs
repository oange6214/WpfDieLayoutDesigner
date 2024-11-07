using System.Windows;

namespace DieLayoutDesigner.Controls;

public class DiePositionEventArgs : EventArgs
{
    public Point DieIndex { get; }
    public Point Offset { get; }

    public DiePositionEventArgs(Point dieIndex, Point offset)
    {
        DieIndex = dieIndex;
        Offset = offset;
    }
}