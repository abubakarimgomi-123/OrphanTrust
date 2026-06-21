using System.Linq;
using System.Web.Mvc;
using OrphanTrust.Infrastructure.Constants;
using OrphanTrust.Models;

namespace OrphanTrust.Controllers
{
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AuditLogsController()
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

            var logs = _db.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(200)
                .ToList();
            return View(logs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}