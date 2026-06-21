using System.Linq;
using System.Web.Mvc;
using OrphanTrust.Models;

namespace OrphanTrust.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController()
        {
            _db = new ApplicationDbContext();
        }

        // GET: Home/Index — Public landing page
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            ViewBag.TotalOrphanages = _db.Orphanages
                .Count(o => o.Status == "Active");
            ViewBag.TotalChildren = _db.Children.Count();
            ViewBag.TotalDonors = _db.Users.Count();
            ViewBag.TotalDonations =
                _db.Donations
                   .Where(d => d.Status == "Confirmed")
                   .Sum(d => (decimal?)d.Amount) ?? 0;

            var orphanages = _db.Orphanages
                .Where(o => o.Status == "Active")
                .OrderByDescending(o => o.CreatedAt)
                .Take(6)
                .ToList();

            return View(orphanages);
        }

        [AllowAnonymous]
        public ActionResult About()
            => View();

        [AllowAnonymous]
        public ActionResult Contact()
            => View();

        [AllowAnonymous]
        public ActionResult Privacy()
            => View();

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}