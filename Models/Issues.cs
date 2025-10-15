namespace MzansiFoSho_ST10070824.Models
{
    public class Issues
    {
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public string Engagement { get; set; }
        public IFormFile Attachment { get; set; }
    }

}
