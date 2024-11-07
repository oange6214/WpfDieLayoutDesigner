using CommunityToolkit.Mvvm.ComponentModel;
using DieLayoutDesigner.Controls;
using DieLayoutDesigner.Models;
using System.Collections.ObjectModel;

namespace DieLayoutDesigner.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    #region Constructors

    public MainWindowViewModel()
    {
        ScaleValue = 1;
        DieWidth = DieMapConverter.MicronsToPixels(100) / ScaleValue;
        DieHeight = DieMapConverter.MicronsToPixels(100) / ScaleValue;
        XOffset = 400 - DieWidth / 2;
        YOffset = 250 - DieHeight / 2;
    }

    #endregion Constructors

    #region Fields

    private double _dieHeight;
    private double _dieWidth;
    private double _scaleValue;
    private DieShape? _selectedShape;
    private ObservableCollection<DieShape> _shapes = [];
    private double _xOffset;
    private double _yOffset;

    #endregion Fields

    #region Properties

    public double DieHeight
    {
        get => _dieHeight;
        set => SetProperty(ref _dieHeight, value);
    }

    public double DieWidth
    {
        get => _dieWidth;
        set => SetProperty(ref _dieWidth, value);
    }

    public double ScaleValue
    {
        get => _scaleValue;
        set => SetProperty(ref _scaleValue, value);
    }

    public DieShape? SelectedShape
    {
        get => _selectedShape;
        set => SetProperty(ref _selectedShape, value);
    }

    public ObservableCollection<DieShape> Shapes
    {
        get => _shapes;
        set => SetProperty(ref _shapes, value);
    }
    public double XOffset
    {
        get => _xOffset;
        set => SetProperty(ref _xOffset, value);
    }
    public double YOffset
    {
        get => _yOffset;
        set => SetProperty(ref _yOffset, value);
    }

    #endregion Properties

}