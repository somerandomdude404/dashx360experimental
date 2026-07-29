using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XboxMetroLauncher.Models.Bing;

namespace XboxMetroLauncher.Services;

public interface IBingSearchAggregator
{
    Task<IReadOnlyList<BingSearchResult>> AggregateAsync(string query, object library);
}
