using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using MzansiFoSho_ST10070824.Models;

namespace MzansiFoSho_ST10070824.Services
{
    public class RecommendationService
    {
            private readonly Dictionary<string, int> _searchFreq =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            public void RecordSearch(string term)
            {
                if (string.IsNullOrWhiteSpace(term)) return;
                if (_searchFreq.ContainsKey(term)) _searchFreq[term]++;
                else _searchFreq[term] = 1;
            }

            public IReadOnlyDictionary<string, int> Snapshot() => _searchFreq;

            public IEnumerable<LocalEvents> Recommend(IEnumerable<LocalEvents> source, int max = 5)
            {
                if (_searchFreq.Count == 0)
                    return source.OrderByDescending(e => e.Popularity).Take(max);

                var top = _searchFreq.OrderByDescending(kv => kv.Value).First().Key;

                return source
                    .Where(e =>
                        (e.Title?.IndexOf(top, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                        (e.Description?.IndexOf(top, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                        (e.Category?.IndexOf(top, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                        (e.Location?.IndexOf(top, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0)
                    .OrderByDescending(e => e.Popularity)
                    .ThenBy(e => e.Date)
                    .Take(max);
            }
        }
    }


