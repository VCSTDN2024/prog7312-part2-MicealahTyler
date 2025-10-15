using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MzansiFoSho_ST10070824.Models
{
    public class EventsIndexViewModel
    {
        public IEnumerable<LocalEvents> Events { get; set; }
        public IEnumerable<LocalEvents> Recommendations { get; set; }
        public IEnumerable<LocalEvents> RecentlyViewed { get; set; }
        public IEnumerable<LocalEvents> UpcomingQueue { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public string Search { get; set; }
        public string Category { get; set; }
        public string InfoMessage { get; set; }
    }
}
