using System.Linq;
using System.Web.Mvc;
using OrphanTrust.Infrastructure.Constants;
using OrphanTrust.Models;

namespace OrphanTrust.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UsersController() { _db = new ApplicationDbContext(); }

        private bool IsSuperAdmin() =>
            Session["UserRole"] != null &&
            Session["UserRole"].ToString() == RoleConstants.SuperAdmin;

        public ActionResult Index()
        {
            if (!IsSuperAdmin())
                return RedirectToAction("Login", "Account");

            var users = _db.Users.OrderByDescending(u => u.CreatedAt)
                .ToList();
            return View(users);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}