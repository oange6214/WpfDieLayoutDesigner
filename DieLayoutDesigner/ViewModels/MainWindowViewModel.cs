using CommunityToolkit.Mvvm.ComponentModel;
using DieLayoutDesigner.Models;
using System.Collections.ObjectModel;


namespace DieLayoutDesigner.ViewModels;

public class MainWindowViewModel : ObservableObject
{

    #region Fields

    private double _scaleValue = 1;
    private DieShape? _selectedShape;
    private ObservableCollection<DieShape> _shapes = [];
    private double _xDiePitch = 100;
    private double _xOffset = 350;
    private double _yDiePitch = 100;
    private double _yOffset = 200;

    #endregion Fields

    #region Properties

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

    public double XDiePitch
    {
        get => _xDiePitch;
        set => SetProperty(ref _xDiePitch, value);
    }

    public double XOffset
    {
        get => _xOffset;
        set => SetProperty(ref _xOffset, value);
    }

    public double YDiePitch
    {
        get => _yDiePitch;
        set => SetProperty(ref _yDiePitch, value);
    }

    public double YOffset
    {
        get => _yOffset;
        set => SetProperty(ref _yOffset, value);
    }

    #endregion Properties
}