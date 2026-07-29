using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using XboxMetroLauncher.Models.Bing;
using XboxMetroLauncher.Services;

namespace XboxMetroLauncher.ViewModels.Tabs;

public sealed class BingTabViewModel : DashboardTabViewModel
{
    private readonly IBingSearchAggregator _aggregator;
    private BingResultCategoryViewModel? _activeCategory;
    private bool _isResultsOpen;
    private bool _isKeyboardOpen;

    public ObservableCollection<string> TrendingSearches { get; }
    public ObservableCollection<BingResultCategoryViewModel> Categories { get; }

    public BingResultCategoryViewModel? ActiveCategory
    {
        get => _activeCategory;
        set { if (SetField(ref _activeCategory, value)) OnPC(nameof(ResultCountText)); }
    }

    public bool IsResultsOpen
    {
        get => _isResultsOpen;
        set { if (SetField(ref _isResultsOpen, value)) OnPC(nameof(ResultCountText)); }
    }

    public bool IsKeyboardOpen { get => _isKeyboardOpen; set => SetField(ref _isKeyboardOpen, value); }

    public string ResultCountText =>
        (_activeCategory is null || _activeCategory.Results.Count == 0)
            ? string.Empty
            : $"{_activeCategory.Results.Count} results in {_activeCategory.DisplayName}";

    // Shell-delegated commands (preserved from the original stub)
    public ICommand SubmitSearchCommand => base.Shell.SubmitSearchCommand;
    public ICommand UseTrendingSearchCommand => base.Shell.UseTrendingSearchCommand;

    // New commands
    public ICommand OpenResultCommand { get; }
    public ICommand CloseResultsCommand { get; }
    public ICommand OpenVirtualKeyboardCommand { get; }

    public BingTabViewModel(DashboardViewModel shell, IBingSearchAggregator aggregator)
        : base(shell, "bing", "bing")
    {
        _aggregator = aggregator;

        TrendingSearches = new ObservableCollection<string>
        {
            "Halo Reach", "Forza Horizon", "Game Pass PC",
            "Local co-op games", "Xbox 360 dashboard"
        };

        Categories = new ObservableCollection<BingResultCategoryViewModel>
        {
            new("games",  "Games"),
            new("apps",   "Apps"),
            new("music",  "Music"),
            new("web",    "Web"),
            new("images", "Images"),
            new("videos", "Videos"),
            new("news",   "News"),
        };
        Categories[0].IsActive = true;
        ActiveCategory = Categories[0];

        OpenResultCommand = new RelayCommand(OpenResult);
        CloseResultsCommand = new RelayCommand(_ => CloseResults());
        OpenVirtualKeyboardCommand = new RelayCommand(_ => IsKeyboardOpen = !IsKeyboardOpen);

        // Intercept the shell submit so we can populate results before/instead of browser.
        base.Shell.SearchSubmitted += async (_, q) => await RunSearchAsync(q);
    }

    public void SetActiveCategory(BingResultCategoryViewModel cat)
    {
        foreach (var c in Categories) c.IsActive = (c == cat);
        ActiveCategory = cat;
        base.Shell.Audio?.Play("select");
    }

    public void EnsureKeyboardClosed() => IsKeyboardOpen = false;

    private async Task RunSearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;
        IsResultsOpen = true;
        foreach (var c in Categories) c.Results.Clear();

        var results = await _aggregator.AggregateAsync(query, base.Shell.Library);
        foreach (var r in results)
        {
            var cat = Categories.FirstOrDefault(c => c.Key == r.Category);
            cat?.Results.Add(r);
        }
        var firstNonEmpty = Categories.FirstOrDefault(c => c.Results.Count > 0) ?? Categories[0];
        SetActiveCategory(firstNonEmpty);
    }

    private void OpenResult(object? parameter)
    {
        if (parameter is not BingSearchResult r) return;
        base.Shell.Audio?.Play("select");
        if (r.LaunchTarget == "web" && !string.IsNullOrEmpty(r.LaunchUrl))
        {
            // Reuse the existing browser-launch path.
            base.Shell.SearchService.SearchWebAsync(r.Title, r.LaunchUrl);
        }
        else if (r.LaunchTarget == "game")
        {
            base.Shell.LaunchById(r.LaunchUrl); // helper on shell, see §10
        }
    }

    private void CloseResults()
    {
        IsResultsOpen = false;
        base.Shell.Audio?.Play("menu-out");
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    private void OnPC(string n) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(n));
    private bool SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? n = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value; OnPC(n!); return true;
    }
}
