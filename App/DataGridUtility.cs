using System.Windows;
using System.Windows.Controls;

namespace ZarzadzanieFinansami;

public abstract class DataGridUtility
{
    public static void UpdateDataGridView(DataGrid myDataGridView)
    {
        var scaleRation = 0.20;
        var gridView = myDataGridView;

        var totalWidth = myDataGridView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        if (gridView.Columns.Count == Constants.STATICNUMBEROFCOLUMNS)
        {
            gridView.Columns[0].Width = 0.01;                                   // "ID"
            gridView.Columns[1].Width = totalWidth * 1.1 * scaleRation;         // "Nazwa"
            gridView.Columns[2].Width = totalWidth * scaleRation;               // "Kwota"
            gridView.Columns[3].Width = totalWidth * scaleRation;               // "Data"
            gridView.Columns[4].Width = totalWidth * 2 * scaleRation;           // "Uwagi"
        }
    }
}