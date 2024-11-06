using System.Windows;

namespace DieLayoutDesigner.Controls;

public class OffsetChangedEventArgs : EventArgs
{
    public Point OldOffset { get; }
    public Point NewOffset { get; }

    public OffsetChangedEventArgs(Point oldOffset, Point newOffset)
    {
        OldOffset = oldOffset;
        NewOffset = newOffset;
    }
}