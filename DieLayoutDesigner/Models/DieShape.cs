using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DieLayoutDesigner.Models;

public class DieShape : ObservableObject
{
    private Point _topLeft;
    private Size _dieSize;
    private Brush _fillColor;
    private bool _isSelected;
    private Geometry? _data;

    public Point TopLeft
    {
        get => _topLeft;
        set => SetProperty(ref _topLeft, value);
    }

    public Size DieSize
    {
        get => _dieSize;
        set => SetProperty(ref _dieSize, value);
    }

    public Brush FillColor
    {
        get => _fillColor;
        set => SetProperty(ref _fillColor, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public Geometry? Data
    {
        get => _data;
        set => SetProperty(ref _data, value);
    }
}