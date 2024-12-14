using System.Media;
using System.Windows;

namespace ZarzadzanieFinansami;

public partial class NumberOfRecordsOnPage
{
    private int _numberOfRows;
    private bool _isClosing = false;
    private bool _isAccepted = false;

    public NumberOfRecordsOnPage(int numberOfRecords)
    {
        InitializeComponent();
        _numberOfRows = numberOfRecords;
        RowsComboBox.Text = numberOfRecords.ToString();
    }

    private void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        if (-1 == RowsComboBox.SelectedIndex) return;
        var rowValues = Constants.RAWVALUES;
        if (Core.NumberOfRows != rowValues.GetValueOrDefault(RowsComboBox.SelectedIndex)) Core.Page = 1;
        Core.NumberOfRows = rowValues.GetValueOrDefault(RowsComboBox.SelectedIndex);
        _isAccepted = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _isClosing = true;
        Close();
    }

/***********************************************************************************************************************/
/*                                              Back Events Handlers                                                   */
/*                                                   Black Magic                                                       */
/***********************************************************************************************************************/
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var hwndSource = System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(this).Handle);
        if (hwndSource != null) hwndSource.AddHook(HwndMessageHook);
    }

    private IntPtr HwndMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;
        if (msg == WM_SYSCOMMAND && (wParam.ToInt32() & 0xFFF0) == SC_MOVE) handled = true;
        return IntPtr.Zero;
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        if (_isClosing) return;
        if (_isAccepted) return;
        SystemSounds.Beep.Play();
        Activate();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _isClosing = true;
    }
}