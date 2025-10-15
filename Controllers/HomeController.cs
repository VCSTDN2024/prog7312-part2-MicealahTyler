using Microsoft.AspNetCore.Mvc;
using MzansiFoSho_ST10070824.Models;

namespace MzansiFoSho_ST10070824.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Temporary in-memory storage of issues
        private static List<Issues> Issue = new List<Issues>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Home Page
        public ActionResult Index()
        {
            return View();
        }

        // GET: Show the Report Issue form
        [HttpGet]
        public ActionResult ReportIssue()
        {
            return View();
        }

        // POST: Handle Issue Submission (manual handling of engagement + file upload)
        [HttpPost]
        public ActionResult ReportIssue(IFormCollection form, IFormFile attachment)
        {
            var issue = new Issues
            {
                Location1 = form["location1"],
                Location2 = form["location2"],
                City = form["City"],
                Province = form["Province"],
                Category = form["category"]
            };

            // Handle engagement (description)
            string engagement = form["engagement"];

            // Handle attachment (file upload)
            if (attachment != null && attachment.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // Ensure uploads folder exists
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, attachment.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    attachment.CopyTo(stream);
                }
            }

            // Save issue in temporary list
            Issue.Add(issue);

            // Pass extra info via ViewBag
            ViewBag.Engagement = engagement;
            ViewBag.AttachmentName = attachment?.FileName;

            // Show confirmation page
            return View("ReportConfirmation", issue);
        }

        // View all submitted issues
        public ActionResult ViewAllIssues()
        {
            return View(Issue);
        }

        // Placeholder pages
        public ActionResult LocalEvents()
        {
            return View();
        }

        public ActionResult ServiceStatus()
        {
            return View();
        }
    }
}
