using DieLayoutDesigner.Adorners;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DieLayoutDesigner.Managers;

public class PreviewManager : IDisposable
{
    private PreviewAdorner? _currentPreview;
    private bool _disposed;

    public void StartPreview(Canvas canvas, Point startPoint)
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
        _currentPreview = new PreviewAdorner(canvas, startPoint);
        adornerLayer?.Add(_currentPreview);
    }

    public void UpdatePreview(Point currentPoint)
    {
        _currentPreview?.UpdatePosition(currentPoint);
    }

    public void EndPreview(Canvas canvas)
    {
        if (_currentPreview != null)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(canvas);
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