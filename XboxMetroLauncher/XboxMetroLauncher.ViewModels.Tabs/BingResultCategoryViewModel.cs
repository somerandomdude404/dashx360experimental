using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using XboxMetroLauncher.Models.Bing;

namespace XboxMetroLauncher.ViewModels.Tabs;

public sealed class BingResultCategoryViewModel : INotifyPropertyChanged
{
    private bool _isActive;
    public string Key { get; }
    public string DisplayName { get; }
    public ObservableCollection<BingSearchResult> Results { get; } = new();

    public BingResultCategoryViewModel(string key, string displayName) => (Key, DisplayName) = (key, displayName);

    public bool IsActive
    {
        get => _isActive;
        set { if (_isActive != value) { _isActive = value; OnPC(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPC([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
