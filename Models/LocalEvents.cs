namespace MzansiFoSho_ST10070824.Models
{
    public class LocalEvents
    {
        public string Id { get; set; }          // key (Dictionary)
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }    // categories (HashSet)
        public DateTime Date { get; set; }      // SortedDictionary<DateTime, List<Event>>
        public int Popularity { get; set; }     // used by our priority structure
        public string Location { get; set; }
    }
}
