using System.Windows.Controls;
using System.Windows.Input;
using XboxMetroLauncher.ViewModels.Tabs;

namespace XboxMetroLauncher.Views.Tabs;

public partial class BingTabView : UserControl
{
    public BingTabView() => InitializeComponent();

    private void SearchBoxFrame_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        BingSearchBox.Focus();
        if (DataContext is BingTabViewModel vm) vm.EnsureKeyboardClosed();
    }

    private void BingSearchBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not BingTabViewModel vm) return;
        if (e.Key == Key.Enter)
        {
            if (vm.SubmitSearchCommand.CanExecute(null)) vm.SubmitSearchCommand.Execute(null);
            e.Handled = true;
        }
        else if (e.Key == Key.Y) { vm.OpenVirtualKeyboardCommand.Execute(null); e.Handled = true; }
    }

    private void Category_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && rb.DataContext is BingResultCategoryViewModel cat
            && DataContext is BingTabViewModel vm)
        {
            vm.SetActiveCategory(cat);
        }
    }
}
