using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DieLayoutDesigner.Models;

public class DieShape : ObservableObject
{
    #region Constructors

    public DieShape()
    {
        Index = ++_counter;
        IsVisible = true;
    }

    #endregion Constructors

    #region Fields

    private int _zIndex;
    public int ZIndex
    {
        get => _zIndex;
        set => SetProperty(ref _zIndex, value);
    }

    private static int _counter = 0;
    private Geometry? _data;
    private Size _dieSize;
    private Brush _fillColor;
    private int _index;
    private bool _isSelected;
    private bool _isVisible = true;
    private string _name;
    private Point _topLeft;
    private Point _bottomLeft;

    #endregion Fields

    #region Properties

    public Geometry? Data
    {
        get => _data;
        set => SetProperty(ref _data, value);
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

    public int Index
    {
        get => _index;
        set => SetProperty(ref _index, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public string Name
    {
        get => _name ?? $"矩形 {Index}";
        set => SetProperty(ref _name, value);
    }

    public Point TopLeft
    {
        get => _topLeft;
        set => SetProperty(ref _topLeft, value);
    }

    public Point BottomLeft
    {
        get => _bottomLeft;
        set => SetProperty(ref _bottomLeft, value);
    }

    #endregion Properties
}