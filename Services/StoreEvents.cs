using Microsoft.Extensions.Logging;
using MzansiFoSho_ST10070824.Models;

namespace MzansiFoSho_ST10070824.Services
{
    public class StoreEvents
    {
        // Hash table / dictionary
        private readonly Dictionary<string, LocalEvents> _events = new Dictionary<string, LocalEvents>(StringComparer.OrdinalIgnoreCase);

        // Sorted by date
        private readonly SortedDictionary<DateTime, List<LocalEvents>> _byDate = new SortedDictionary<DateTime, List<LocalEvents>>();

        // Unique categories
        private readonly HashSet<string> _categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Recently viewed stack (store ids to avoid duplication of objects)
        private readonly Stack<string> _recent = new Stack<string>();

        // Upcoming notifications queue (ids)
        private readonly Queue<string> _upcoming = new Queue<string>();

        // Popularity priority “queue”: key = popularity, value = FIFO of ids
        // Use descending order for max-first behaviour.
        private readonly SortedDictionary<int, Queue<string>> _popularityBuckets =
            new SortedDictionary<int, Queue<string>>(Comparer<int>.Create((a, b) => b.CompareTo(a)));

        public StoreEvents()
        {
            Seed(); // small demo dataset
        }

        public IEnumerable<LocalEvents> AllEvents() => _events.Values;

        public IEnumerable<string> Categories() => _categories;

        public IEnumerable<LocalEvents> Chronological()
        {
            foreach (var kv in _byDate)
                foreach (var e in kv.Value.OrderBy(x => x.Date))
                    yield return e;
        }

        public LocalEvents Find(string id) => id != null && _events.TryGetValue(id, out var e) ? e : null;

        public void View(string id)
        {
            if (id != null && _events.ContainsKey(id))
                _recent.Push(id); // Stack (LIFO)
        }

        public IEnumerable<LocalEvents> RecentlyViewed(int max = 5)
            => _recent.Take(max).Select(id => _events[id]);

        public void EnqueueUpcoming(string id)
        {
            if (id != null && _events.ContainsKey(id))
                _upcoming.Enqueue(id); // Queue (FIFO)
        }

        public IEnumerable<LocalEvents> Upcoming(int max = 5)
            => _upcoming.Take(max).Select(id => _events[id]);

        public void Add(LocalEvents e)
        {
            _events[e.Id] = e;

            // Sorted by date
            if (!_byDate.ContainsKey(e.Date.Date))
                _byDate[e.Date.Date] = new List<LocalEvents>();
            _byDate[e.Date.Date].Add(e);

            // Unique categories
            _categories.Add(e.Category);

            // Popularity bucket
            if (!_popularityBuckets.ContainsKey(e.Popularity))
                _popularityBuckets[e.Popularity] = new Queue<string>();
            _popularityBuckets[e.Popularity].Enqueue(e.Id);

            // Default upcoming queue
            _upcoming.Enqueue(e.Id);
        }
        public LocalEvents DequeueMostPopular()
        {
            foreach (var kv in _popularityBuckets)
            {
                if (kv.Value.Count > 0)
                {
                    var id = kv.Value.Dequeue();
                    return _events[id];
                }
            }
            return null;
        }

        private void Seed()
        {
            var demo = new[]
            {
                new LocalEvents{ Id="EVT001", Title="Soweto Marathon Expo", Category="Fitness",  Description="Run through Soweto's historic streets, passing iconic landmarks like Vilakazi Street and Regina Mundi Church.", Date=DateTime.Today.AddDays(2),  Popularity=90, Location="FNB Stadium,Johannesburg"},
                new LocalEvents{ Id="EVT002", Title="Cape Town Jazz Evening", Category="Culture", Description="Calling upon all Jazz lovers! Enjoy a vibrant jazz evening session at the V&A WaterFront.",    Date=DateTime.Today.AddDays(7),  Popularity=85, Location="V&A WaterFront,Cape Town"},
                new LocalEvents{ Id="EVT003", Title="HIV Rapid Test Drive", Category="Health",  Description="Knowing one’s HIV status is important not only to protect oneself but to also protect others. Results are available within 30 minutes.", Date=DateTime.Today.AddDays(1),  Popularity=70, Location="Umhlanga,Durban"},
                new LocalEvents{ Id="EVT004", Title="Recycling Awareness",  Category="Community",Description="Reduce, Reuse, Recycle: Learn recycling tips", Date=DateTime.Today.AddDays(5),  Popularity=60, Location="Pretoria"},
                new LocalEvents{ Id="EVT005", Title="Youth Tech Hackathon", Category="Technology",Description="A 48h hackathon aimed to empower the youth and equip future hackers.", Date=DateTime.Today.AddDays(10), Popularity=95, Location="Sandton,Johannesburg"},
                new LocalEvents{ Id="EVT006", Title="Caramel Sundae Festival", Category="Culture", Description="A music and food festival celebrating summer vibes, live performances, and delicious food.", Date=new DateTime(DateTime.Today.Year, 12, 14), Popularity=88, Location="Muldersdrift,Johannesburg"},
                new LocalEvents{ Id="EVT007", Title="HYROX Fitness Challenge", Category="Fitness", Description="A competitive fitness race combining endurance, running, and strength exercises — open to all levels.", Date=DateTime.Today.AddDays(21), Popularity=92, Location="Sandton Convention Centre, Johannesburg"}
            };

            foreach (var e in demo) Add(e);
        }
    }
}
