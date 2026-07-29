using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XboxMetroLauncher.Models.Bing;

namespace XboxMetroLauncher.Services;

public sealed class BingSearchAggregatorService : IBingSearchAggregator
{
    // base URLs that mirror the 2009 Bing category endpoints
    private const string WebBase    = "https://www.bing.com/search?q=";
    private const string ImageBase  = "https://www.bing.com/images/search?q=";
    private const string VideoBase  = "https://www.bing.com/videos/search?q=";
    private const string NewsBase   = "https://www.bing.com/news/search?q=";

    public Task<IReadOnlyList<BingSearchResult>> AggregateAsync(string query, object library)
    {
        var q = query?.Trim() ?? string.Empty;
        if (q.Length == 0) return Task.FromResult<IReadOnlyList<BingSearchResult>>(Array.Empty<BingSearchResult>());

        var results = new List<BingSearchResult>();

        // ---- Local library (the real "search your Xbox" experience) ----
        foreach (var tile in EnumerateLibrary(library))
        {
            if (tile.Title.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0) continue;
            results.Add(new BingSearchResult
            {
                Title      = tile.Title,
                Subtitle   = tile.Kind,
                Source     = tile.Source,
                Category   = tile.Kind.Equals("App", StringComparison.OrdinalIgnoreCase) ? "apps" : "games",
                Thumbnail  = tile.Thumbnail,
                LaunchTarget = tile.Kind.Equals("App", StringComparison.OrdinalIgnoreCase) ? "web" : "game",
                LaunchUrl  = tile.Id
            });
        }

        // ---- Music (templated; opens browser) ----
        results.Add(new BingSearchResult
        {
            Title = $"{q} — artist radio",
            Subtitle = "Open in your music player",
            Source = "Bing Music",
            Category = "music",
            LaunchTarget = "web",
            LaunchUrl = $"https://www.bing.com/music/search?q={Uri.EscapeDataString(q)}"
        });

        // ---- Web / Images / Videos / News fallbacks (2009 Explore-pane set) ----
        results.Add(new BingSearchResult
        {
            Title = $"Search the web for “{q}”",
            Subtitle = "Open Bing in browser",
            Source = "bing.com",
            Category = "web",
            LaunchTarget = "web",
            LaunchUrl = WebBase + Uri.EscapeDataString(q)
        });
        results.Add(new BingSearchResult
        {
            Title = $"Images for “{q}”",
            Source = "bing.com/images",
            Category = "images",
            LaunchTarget = "web",
            LaunchUrl = ImageBase + Uri.EscapeDataString(q)
        });
        results.Add(new BingSearchResult
        {
            Title = $"Videos for “{q}”",
            Source = "bing.com/videos",
            Category = "videos",
            LaunchTarget = "web",
            LaunchUrl = VideoBase + Uri.EscapeDataString(q)
        });
        results.Add(new BingSearchResult
        {
            Title = $"News for “{q}”",
            Source = "bing.com/news",
            Category = "news",
            LaunchTarget = "web",
            LaunchUrl = NewsBase + Uri.EscapeDataString(q)
        });

        return Task.FromResult<IReadOnlyList<BingSearchResult>>(results);
    }

    // Duck-typed enumeration so we don't hard-couple to GameLibrary internals.
    private static IEnumerable<LibraryTile> EnumerateLibrary(object? library)
    {
        if (library is null) yield break;
        var gamesProp = library.GetType().GetProperty("Games");
        var games = gamesProp?.GetValue(library) as System.Collections.IEnumerable;
        if (games is not null)
        {
            foreach (var g in games)
            {
                var t = g.GetType();
                yield return new LibraryTile
                {
                    Title     = (string?)t.GetProperty("Title")?.GetValue(g) ?? "Untitled",
                    Kind      = "Game",
                    Source    = "Local",
                    Id        = (string?)t.GetProperty("Id")?.GetValue(g) ?? string.Empty,
                    Thumbnail = t.GetProperty("CoverImage")?.GetValue(g) as System.Windows.Media.ImageSource
                };
            }
        }
        var appsProp = library.GetType().GetProperty("Apps");
        var apps = appsProp?.GetValue(library) as System.Collections.IEnumerable;
        if (apps is not null)
        {
            foreach (var a in apps)
            {
                var t = a.GetType();
                yield return new LibraryTile
                {
                    Title     = (string?)t.GetProperty("Title")?.GetValue(a) ?? "App",
                    Kind      = "App",
                    Source    = "Local",
                    Id        = (string?)t.GetProperty("Path")?.GetValue(a) ?? string.Empty,
                    Thumbnail = t.GetProperty("Icon")?.GetValue(a) as System.Windows.Media.ImageSource
                };
            }
        }
    }

    private sealed class LibraryTile
    {
        public string Title = string.Empty;
        public string Kind = string.Empty;
        public string Source = string.Empty;
        public string Id = string.Empty;
        public System.Windows.Media.ImageSource? Thumbnail;
    }
}
