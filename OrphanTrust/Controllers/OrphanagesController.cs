using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using OrphanTrust.Infrastructure.Blockchain;
using OrphanTrust.Infrastructure.Constants;
using OrphanTrust.Models;
using OrphanTrust.Models.DomainModels;

namespace OrphanTrust.Controllers
{
    public class OrphanagesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrphanagesController()
        {
            _db = new ApplicationDbContext();
        }

        private bool IsLoggedIn()
            => Session["UserId"] != null;

        private bool IsSuperAdmin()
            => Session["UserRole"] != null &&
               Session["UserRole"].ToString() == RoleConstants.SuperAdmin;

        public ActionResult Index()
        {
            var orphanages = _db.Orphanages
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
            return View(orphanages);
        }

        public ActionResult Details(int id)
        {
            var orphanage = _db.Orphanages
                .FirstOrDefault(o => o.OrphanageId == id);
            if (orphanage == null)
                return HttpNotFound();
            return View(orphanage);
        }

        public ActionResult Create()
        {
            if (!IsLoggedIn() || !IsSuperAdmin())
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Orphanage model)
        {
            if (!IsLoggedIn() || !IsSuperAdmin())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.UtcNow;
                model.Status = OrphanageStatus.Active;
                _db.Orphanages.Add(model);
                _db.SaveChanges();
                TempData["Success"] = "Orphanage created successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!IsLoggedIn() || !IsSuperAdmin())
                return RedirectToAction("Login", "Account");

            var orphanage = _db.Orphanages
                .FirstOrDefault(o => o.OrphanageId == id);
            if (orphanage == null)
                return HttpNotFound();
            return View(orphanage);
        }

        [HttpPost]
        public ActionResult Edit(Orphanage model)
        {
            if (!IsLoggedIn() || !IsSuperAdmin())
                return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var orphanage = _db.Orphanages
                    .FirstOrDefault(
                        o => o.OrphanageId == model.OrphanageId);
                if (orphanage == null)
                    return HttpNotFound();

                orphanage.Name = model.Name;
                orphanage.Region = model.Region;
                orphanage.Address = model.Address;
                orphanage.RegistrationNumber = model.RegistrationNumber;
                orphanage.Description = model.Description;
                orphanage.ContactPhone = model.ContactPhone;
                orphanage.ContactEmail = model.ContactEmail;
                orphanage.Status = model.Status;
                orphanage.UpdatedAt = DateTime.UtcNow;

                _db.SaveChanges();
                TempData["Success"] = "Orphanage updated successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!IsLoggedIn() || !IsSuperAdmin())
                return RedirectToAction("Login", "Account");

            var orphanage = _db.Orphanages
                .FirstOrDefault(o => o.OrphanageId == id);
            if (orphanage != null)
            {
                _db.Orphanages.Remove(orphanage);
                _db.SaveChanges();
                TempData["Success"] = "Orphanage deleted.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Verify(int id)
        {
            if (!IsLoggedIn() || !IsSuperAdmin())
                return RedirectToAction("Login", "Account");

            var orphanage = _db.Orphanages
                .FirstOrDefault(o => o.OrphanageId == id);

            if (orphanage != null)
            {
                // Save to DB first
                orphanage.Status = OrphanageStatus.Active;
                orphanage.IsVerifiedOnChain = true;
                orphanage.VerifiedAt = DateTime.UtcNow;
                orphanage.VerifiedByUserId = (int?)Session["UserId"];
                orphanage.UpdatedAt = DateTime.UtcNow;
                _db.SaveChanges();

                // Then blockchain
                string txHash = null;
                try
                {
                    var blockchain = new BlockchainService();
                    txHash = await blockchain.VerifyOrphanageAsync(
                        orphanage.OrphanageId,
                        orphanage.Name,
                        orphanage.RegistrationNumber,
                        orphanage.Region);

                    if (txHash != null)
                    {
                        orphanage.BlockchainAddress = txHash;
                        _db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Blockchain error: " + ex.Message);
                }

                TempData["Success"] = txHash != null
                    ? "Verified on Avalanche! TX: " + txHash
                    : "Orphanage verified!";
            }
            return RedirectToAction("Index");
        }

        public ActionResult MyOrphanage()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var orphanage = _db.Orphanages
                .FirstOrDefault(o => o.AdminUserId == userId);

            if (orphanage == null)
                return View("NoOrphanageAssigned");

            return View(orphanage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}