using System.Windows;

namespace DieLayoutDesigner.Controls;


public class DieSelectionEventArgs : EventArgs
{
    #region Properties

    /// <summary>
    /// 選取的 Die 座標（X, Y 為整數座標，表示第幾個 Die）
    /// </summary>
    public Point DieIndex { get; }

    /// <summary>
    /// Die 內的相對位置（微米）
    /// </summary>
    public Point LocalOffset { get; }

    /// <summary>
    /// 選取模式（單選、多選）
    /// </summary>
    public DieSelectionMode SelectionMode { get; }

    /// <summary>
    /// 是否為選取狀態（false 表示取消選取）
    /// </summary>
    public bool IsSelected { get; }

    /// <summary>
    /// 當前所有已選取的 Die 座標列表
    /// </summary>
    public IReadOnlyList<Point> SelectedDies { get; }

    #endregion

    #region Constructors

    public DieSelectionEventArgs(
        Point dieIndex,
        Point localOffset,
        DieSelectionMode selectionMode,
        bool isSelected,
        IReadOnlyList<Point> selectedDies)
    {
        DieIndex = dieIndex;
        LocalOffset = localOffset;
        SelectionMode = selectionMode;
        IsSelected = isSelected;
        SelectedDies = selectedDies;
    }

    #endregion
}

public enum DieSelectionMode
{
    /// <summary>
    /// 單選模式
    /// </summary>
    Single,

    /// <summary>
    /// 多選模式
    /// </summary>
    Multiple,

    /// <summary>
    /// 區域選取模式
    /// </summary>
    Region
}