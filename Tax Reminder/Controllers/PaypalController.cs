using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tax_Reminder.Controllers
{
    public class PaypalController : Controller
    {
        // GET: Paypal
        public ActionResult PaymentWithCreditCard()
        {
            return View();
        }
    }
}