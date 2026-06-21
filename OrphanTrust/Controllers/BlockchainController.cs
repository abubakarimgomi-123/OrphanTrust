using OrphanTrust.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OrphanTrust.Controllers
{
    public class BlockchainController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BlockchainController()
        {
            _db = new ApplicationDbContext();
        }

        // GET: /Blockchain/Index
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            var donations = _db.Donations
                .Where(d => d.TxHash != null)
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            return View(donations);
        }

        // GET: /Blockchain/Verify
        public ActionResult Verify()
        {
            return View();
        }

        // POST: /Blockchain/Verify
        [HttpPost]
        public async Task<ActionResult> Verify(string txHash)
        {
            if (string.IsNullOrEmpty(txHash))
            {
                ViewBag.Error = "Please enter a transaction hash.";
                return View();
            }

            txHash = txHash.Trim();

            // Check in our database
            var donation = _db.Donations
                .FirstOrDefault(d => d.TxHash == txHash);

            ViewBag.TxHash = txHash;
            ViewBag.ExplorerUrl =
                "https://testnet.snowtrace.io/tx/" + txHash;

            if (donation != null)
            {
                ViewBag.Found = true;
                ViewBag.DonationId = donation.DonationId;
                ViewBag.Amount = donation.Amount;
                ViewBag.Currency = donation.Currency;
                ViewBag.Date = donation.CreatedAt;
                ViewBag.Status = donation.BlockchainStatus;

                var orphanage = _db.Orphanages
                    .FirstOrDefault(
                        o => o.OrphanageId == donation.OrphanageId);
                ViewBag.OrphanageName = orphanage != null
                    ? orphanage.Name : "Unknown";
            }
            else
            {
                // Also check orphanage verifications
                var orphanage = _db.Orphanages
                    .FirstOrDefault(o => o.BlockchainAddress == txHash);

                if (orphanage != null)
                {
                    ViewBag.Found = true;
                    ViewBag.IsOrphanage = true;
                    ViewBag.OrphanageName = orphanage.Name;
                    ViewBag.Date = orphanage.VerifiedAt;
                    ViewBag.Status = "Verified";
                }
                else
                {
                    ViewBag.Found = false;
                }
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}