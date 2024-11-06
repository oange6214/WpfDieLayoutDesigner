using DieLayoutDesigner.Adorners;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DieLayoutDesigner.Managers;

public class PreviewManager : IDisposable
{
    private PreviewAdorner? _currentPreview;
    private bool _disposed;

    public void StartPreview(Canvas canvas, Point startPoint, double scaleValue)
    {
        var contentControl = canvas.FindName("PART_ShapesContainer") as ItemsControl;
        if (contentControl == null) return;

        var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
        _currentPreview = new PreviewAdorner(contentControl, startPoint, scaleValue);
        adornerLayer?.Add(_currentPreview);
    }

    public void UpdatePreview(Point currentPoint)
    {
        _currentPreview?.UpdatePosition(currentPoint);
    }

    public void EndPreview(Canvas contentControl)
    {
        ArgumentNullException.ThrowIfNull(contentControl);

        if (_currentPreview != null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(contentControl);
            adornerLayer?.Remove(_currentPreview);
            _currentPreview = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _currentPreview = null;
            }
            _disposed = true;
        }
    }
}