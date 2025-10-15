using Microsoft.AspNetCore.Mvc;
using MzansiFoSho_ST10070824.Models;
using MzansiFoSho_ST10070824.Services;

namespace MzansiFoSho_ST10070824.Controllers
{
    public class EventsController : Controller
    {   
            
            private static readonly StoreEvents _store = new StoreEvents();
            private static readonly RecommendationService _recs = new RecommendationService();

            [HttpGet]
            public IActionResult Index(string search = null, string category = null)
            {
                var eventsQ = _store.AllEvents();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    _recs.RecordSearch(search);
                    var s = search.Trim();
                    eventsQ = eventsQ.Where(e =>
                        (e.Title?.IndexOf(s, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                        (e.Description?.IndexOf(s, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                        (e.Location?.IndexOf(s, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0 ||
                        (e.Category?.IndexOf(s, StringComparison.OrdinalIgnoreCase) ?? -1) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(category))
                    eventsQ = eventsQ.Where(e => string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase));

                var vm = new EventsIndexViewModel
                {
                    Search = search,
                    Category = category,
                    Events = eventsQ.OrderBy(e => e.Date).ToList(),
                    Recommendations = _recs.Recommend(_store.AllEvents()).ToList(),
                    RecentlyViewed = _store.RecentlyViewed().ToList(),
                    UpcomingQueue = _store.Upcoming().ToList(),
                    Categories = _store.Categories().OrderBy(c => c).ToList(),
                    InfoMessage = "Stacks (recent), Queues (upcoming), SortedDictionary (by date), Dictionary (by id), HashSet (unique categories), Priority via popularity buckets."
                };
                return View(vm);
            }

            [HttpGet]
            public IActionResult EventDetails(string id)
            {
                var ev = _store.Find(id);
                if (ev == null) return HttpNotFound();
                _store.View(id);              // push to Stack
                _store.EnqueueUpcoming(id);   // (re)queue in Upcoming FIFO
                return View("EventDetails", ev);
            }

        private IActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
            public IActionResult MostPopular() // pops one from priority queue
            {
                var ev = _store.DequeueMostPopular();
                if (ev == null) return Json(new { ok = false });
                return Json(new { ok = true, id = ev.Id, title = ev.Title, date = ev.Date.ToShortDateString() });
            }
       
    }
   
}

