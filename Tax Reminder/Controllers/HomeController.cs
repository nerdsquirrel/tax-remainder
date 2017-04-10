using System.Web.Mvc;
using Tax_Reminder.DbContext;

namespace Tax_Reminder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        public HomeController()
        {
            db = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
             return View();
        }

        // POST: Home/TaxInformation
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult ThankYou()
        {
            return View();
        }

     
        public ActionResult HowItWorks()
        {
            return View();
        }


    }
}