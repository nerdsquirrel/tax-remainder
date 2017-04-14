using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tax_Reminder.DbContext;
using Tax_Reminder.Models;
using PaypalAPI;

namespace Tax_Reminder.Controllers
{
    public class TaxInformationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TaxInformation
        public ActionResult Index()
        {
            return View(db.TaxInformationModels.ToList());
        }

        // GET: TaxInformation/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaxInformationModel taxInformationModel = db.TaxInformationModels.Find(id);
            if (taxInformationModel == null)
            {
                return HttpNotFound();
            }
            return View(taxInformationModel);
        }



        // GET: TaxInformation/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TaxInformation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,VehicleRegistration,VehicleMake,Email,ReferralCode,CardNumber,CvcSecurityCode,Expiry,IsAgree")] TaxInformationModel taxInformationModel)
        {
            if (ModelState.IsValid)
            {
                double Price = 1.99;
                string tax = "Tax";
                string desc = "Rent";
                string retmsg = null;
                //int parameter = Convert.ToInt32(Session["Id"].ToString());
                db.TaxInformationModels.Add(taxInformationModel);
                db.SaveChanges();
                //PaymentDetails(Convert.ToString(tax), Convert.ToString(desc), System.Convert.ToString(Price), "1");
                if (System.Web.HttpContext.Current.Session["token"] != null && System.Web.HttpContext.Current.Session["token"].ToString() != "")
                {
                    return RedirectToAction(retmsg);
                }
                else
                {
                    return RedirectToAction("Details", new RouteValueDictionary(
                        new { controller = "TaxInformation", action = "Details", Id = taxInformationModel.Id }));
                }
            }

            return View(taxInformationModel);
        }
        private void PaymentDetails(string itemName, string description, string price, string quantity)
        {
            try
            {
                string currency = "USD"; //change to "USD" for US dollars
                string token = string.Empty;
                string retMsg = string.Empty;
                NVPAPICaller PPCaller = new NVPAPICaller();

                bool ret = PPCaller.ExpressCheckout(itemName, description, price, quantity, currency, ref token, ref retMsg);
                if (ret)
                {
                    System.Web.HttpContext.Current.Session["token"] = token;
                    Response.Redirect(retMsg, false);
                }
                else
                {
                    
                    return;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: TaxInformation/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaxInformationModel taxInformationModel = db.TaxInformationModels.Find(id);
            if (taxInformationModel == null)
            {
                return HttpNotFound();
            }
            return View(taxInformationModel);
        }

        // POST: TaxInformation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,VehicleRegistration,VehicleMake,Email,ReferralCode,CardNumber,CvcSecurityCode,Expiry,IsAgree")] TaxInformationModel taxInformationModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(taxInformationModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new RouteValueDictionary(
                        new { controller = "TaxInformation", action = "Details", Id = taxInformationModel.Id }));
            }
            return View(taxInformationModel);
        }

        // GET: TaxInformation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaxInformationModel taxInformationModel = db.TaxInformationModels.Find(id);
            if (taxInformationModel == null)
            {
                return HttpNotFound();
            }
            return View(taxInformationModel);
        }

        // POST: TaxInformation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaxInformationModel taxInformationModel = db.TaxInformationModels.Find(id);
            db.TaxInformationModels.Remove(taxInformationModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
