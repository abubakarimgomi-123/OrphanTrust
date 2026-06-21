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
    public class DonationsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DonationsController()
        {
            _db = new ApplicationDbContext();
        }

        private bool IsLoggedIn() => Session["UserId"] != null;

        // GET: /Donations
        public ActionResult Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var donations = _db.Donations
                .OrderByDescending(d => d.CreatedAt)
                .ToList();
            return View(donations);
        }

        // GET: /Donations/Create
        public ActionResult Create(int? orphanageId)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.Orphanages = _db.Orphanages
                .Where(o => o.Status == "Active")
                .OrderBy(o => o.Name)
                .ToList();

            ViewBag.SelectedOrphanageId = orphanageId;
            return View();
        }

        // POST: /Donations/Create
        [HttpPost]
        public async Task<ActionResult> Create(
            int OrphanageId, decimal Amount,
            string Currency, string PaymentMethod,
            string PaymentReference, string DonorMessage,
            bool IsAnonymous)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];

            var donation = new Donation
            {
                DonorUserId = userId,
                OrphanageId = OrphanageId,
                Amount = Amount,
                Currency = Currency ?? "TZS",
                PaymentMethod = PaymentMethod,
                PaymentReference = PaymentReference,
                DonorMessage = DonorMessage,
                IsAnonymous = IsAnonymous,
                Status = DonationStatus.Confirmed,
                BlockchainStatus = BlockchainTxStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ConfirmedAt = DateTime.UtcNow
            };

            _db.Donations.Add(donation);
            _db.SaveChanges();

            // Record on Avalanche blockchain
            string txHash = null;
            try
            {
                var blockchain = new BlockchainService();
                txHash = await blockchain.RecordDonationAsync(
                    donation.DonationId,
                    Session["Email"]?.ToString() ?? string.Empty,
                    OrphanageId,
                    Amount,
                    Currency ?? "TZS",
                    PaymentReference ?? string.Empty);

                if (txHash != null)
                {
                    donation.TxHash = txHash;
                    donation.BlockchainStatus = BlockchainTxStatus.Confirmed;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "Blockchain error: " + ex.Message);
            }

            TempData["Success"] = txHash != null
                ? "Donation recorded on Avalanche blockchain! TX: " + txHash
                : "Donation confirmed successfully!";

            TempData["TxHash"] = txHash;
            TempData["DonationId"] = donation.DonationId;

            return RedirectToAction("Receipt",
                new { id = donation.DonationId });
        }

        // GET: /Donations/Receipt/5
        public ActionResult Receipt(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var donation = _db.Donations
                .FirstOrDefault(d => d.DonationId == id);

            if (donation == null)
                return HttpNotFound();

            return View(donation);
        }

        // GET: /Donations/MyDonations
        public ActionResult MyDonations()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var donations = _db.Donations
                .Where(d => d.DonorUserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToList();

            return View(donations);
        }

        // GET: /Donations/MyImpact
        public ActionResult MyImpact()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            var donations = _db.Donations
                .Where(d => d.DonorUserId == userId
                         && d.Status == "Confirmed")
                .ToList();

            ViewBag.TotalAmount = donations.Sum(d => d.Amount);
            ViewBag.TotalCount = donations.Count;
            ViewBag.Orphanages =
                donations.Select(d => d.OrphanageId).Distinct().Count();

            return View(donations);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}