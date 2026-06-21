using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using OrphanTrust.Infrastructure.Constants;
using OrphanTrust.Models;

namespace OrphanTrust.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController()
        {
            _db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            if (Session["UserRole"] == null)
                return RedirectToAction("Login", "Account");

            string role = Session["UserRole"].ToString();

            if (role == RoleConstants.SuperAdmin)
                return RedirectToAction("SuperAdminDashboard");
            if (role == RoleConstants.OrphanageAdmin)
                return RedirectToAction("OrphanageAdminDashboard");
            if (role == RoleConstants.Donor)
                return RedirectToAction("DonorDashboard");

            return RedirectToAction("Login", "Account");
        }

        public ActionResult SuperAdminDashboard()
        {
            if (Session["UserRole"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["UserRole"].ToString() != RoleConstants.SuperAdmin)
                return RedirectToAction("Login", "Account");

            ViewBag.TotalOrphanages = _db.Orphanages.Count();
            ViewBag.TotalChildren = _db.Children.Count();
            ViewBag.TotalDonations = _db.Donations.Count();
            ViewBag.TotalUsers = _db.Users.Count();
            ViewBag.PendingOrphanages =
                _db.Orphanages.Count(o => o.Status == "Pending");
            ViewBag.TotalAmountRaised =
                _db.Donations
                   .Where(d => d.Status == "Confirmed")
                   .Sum(d => (decimal?)d.Amount) ?? 0;
            return View();
        }

        public ActionResult OrphanageAdminDashboard()
        {
            if (Session["UserRole"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["UserRole"].ToString() != RoleConstants.OrphanageAdmin)
                return RedirectToAction("Login", "Account");
            return View();
        }

        public ActionResult DonorDashboard()
        {
            if (Session["UserRole"] == null)
                return RedirectToAction("Login", "Account");
            if (Session["UserRole"].ToString() != RoleConstants.Donor)
                return RedirectToAction("Login", "Account");

            int userId = (int)Session["UserId"];
            ViewBag.TotalDonations =
                _db.Donations.Count(d => d.DonorUserId == userId);
            ViewBag.TotalAmount =
                _db.Donations
                   .Where(d => d.DonorUserId == userId
                            && d.Status == "Confirmed")
                   .Sum(d => (decimal?)d.Amount) ?? 0;
            ViewBag.OrphanagesSupported =
                _db.Donations
                   .Where(d => d.DonorUserId == userId)
                   .Select(d => d.OrphanageId)
                   .Distinct()
                   .Count();
            return View();
        }

        public async Task<ActionResult> TestBlockchain()
        {
            try
            {
                string rpcUrl = System.Configuration.ConfigurationManager
                    .AppSettings["AvalancheFujiRpcUrl"];
                string privateKey = System.Configuration.ConfigurationManager
                    .AppSettings["BlockchainAdminPrivateKey"];

                var account = new Nethereum.Web3.Accounts.Account(
                    privateKey, 43113);
                var web3 = new Nethereum.Web3.Web3(account, rpcUrl);

                var balance = await web3.Eth.GetBalance
                    .SendRequestAsync(account.Address);

                var balanceInEth = Nethereum.Web3.Web3
                    .Convert.FromWei(balance.Value);

                var chainId = await web3.Eth.ChainId
                    .SendRequestAsync();

                return Content(
                    "Address: " + account.Address +
                    " | Balance: " + balanceInEth + " AVAX" +
                    " | ChainId: " + chainId +
                    " | RPC: " + rpcUrl);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.Message +
                    " | " + ex.InnerException?.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}