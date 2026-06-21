using System.Linq;
using System.Web.Mvc;
using OrphanTrust.Infrastructure.Constants;
using OrphanTrust.Models;

namespace OrphanTrust.Controllers
{
    public class DistributionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DistributionsController()
        {
            _db = new ApplicationDbContext();
        }

        private bool IsSuperAdmin() =>
            Session["UserRole"] != null &&
            Session["UserRole"].ToString() == RoleConstants.SuperAdmin;

        public ActionResult Index()
        {
            if (!IsSuperAdmin())
                return RedirectToAction("Login", "Account");

            var distributions = _db.Distributions
                .OrderByDescending(d => d.CreatedAt)
                .ToList();
            return View(distributions);
        }

        public ActionResult MyDistributions()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var orphanage = _db.Orphanages
                .FirstOrDefault(o => o.AdminUserId == userId);

            if (orphanage == null)
                return View("NoOrphanageAssigned");

            var distributions = _db.Distributions
                .Where(d => d.OrphanageId == orphanage.OrphanageId)
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            return View("Index", distributions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}