using System.Windows.Media;

namespace XboxMetroLauncher.Models.Bing;

public sealed class BingSearchResult
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;     // e.g. "Netflix", "Steam", "Web"
    public string Category { get; init; } = string.Empty;   // Web/Images/Videos/News/Games/Apps/Music
    public ImageSource? Thumbnail { get; init; }
    public string LaunchUrl { get; init; } = string.Empty;  // for web/video/news
    public string LaunchTarget { get; init; } = string.Empty; // game/app id, or "web"
}
