using log4net.Repository.Hierarchy;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tax_Reminder.App_Start;
using Tax_Reminder.DbContext;
using Tax_Reminder.Models;


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
               
                string retmsg = null;
                //int parameter = Convert.ToInt32(Session["Id"].ToString());
                db.TaxInformationModels.Add(taxInformationModel);
                db.SaveChanges();
                // payment section
                PaymentDetails(taxInformationModel);

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


        private void PaymentDetails(TaxInformationModel taxInformationModely)
        {
            //create and item for which you are taking payment
            //if you need to add more items in the list
            //Then you will need to create multiple item objects or use some loop to instantiate object
            Item item = new Item();
            item.name = "Tax";
            item.currency = "USD";
            item.price = "1.99";
            item.quantity = "1";
            item.sku = "sku";

            //Now make a List of Item and add the above item to it
            //you can create as many items as you want and add to this list
            List<Item> itms = new List<Item>();
            itms.Add(item);
            ItemList itemList = new ItemList();
            itemList.items = itms;

            //Address for the payment
            Address billingAddress = new Address();
            billingAddress.city = "NewYork";
            billingAddress.country_code = "US";
            billingAddress.line1 = "23rd street kew gardens";
            billingAddress.postal_code = "43210";
            billingAddress.state = "NY";

            //Now Create an object of credit card and add above details to it
            CreditCard crdtCard = new CreditCard();
            crdtCard.billing_address = billingAddress;
            crdtCard.cvv2 = "874";
            crdtCard.expire_month = 1;
            crdtCard.expire_year = 2020;
            crdtCard.first_name = "Aman";
            crdtCard.last_name = "Thakur";
            crdtCard.number = taxInformationModely.CardNumber;
            crdtCard.type = "discover";

            // Specify details of your payment amount.
            Details details = new Details();
            details.shipping = "1";
            details.subtotal = "5";
            details.tax = "1";

            // Specify your total payment amount and assign the details object
            Amount amnt = new Amount();
            amnt.currency = "USD";
            // Total = shipping tax + subtotal.
            amnt.total = "7";
            amnt.details = details;

            // Now make a trasaction object and assign the Amount object
            Transaction tran = new Transaction();
            tran.amount = amnt;
            tran.description = "Description about the payment amount.";
            tran.item_list = itemList;
            tran.invoice_number = "your invoice number which you are generating";

            // Now, we have to make a list of trasaction and add the trasactions object
            // to this list. You can create one or more object as per your requirements

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(tran);

            // Now we need to specify the FundingInstrument of the Payer
            // for credit card payments, set the CreditCard which we made above

            FundingInstrument fundInstrument = new FundingInstrument();
            fundInstrument.credit_card = crdtCard;

            // The Payment creation API requires a list of FundingIntrument

            List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            // Now create Payer object and assign the fundinginstrument list to the object
            Payer payr = new Payer();
            payr.funding_instruments = fundingInstrumentList;
            payr.payment_method = "credit_card";

            // finally create the payment object and assign the payer object & transaction list to it
            Payment pymnt = new Payment();
            pymnt.intent = "sale";
            pymnt.payer = payr;
            pymnt.transactions = transactions;


            try
            {
                //getting context from the paypal
                //basically we are sending the clientID and clientSecret key in this function
                //to the get the context from the paypal API to make the payment
                //for which we have created the object above.

                //Basically, apiContext object has a accesstoken which is sent by the paypal
                //to authenticate the payment to facilitator account.
                //An access token could be an alphanumeric string

                APIContext apiContext = PaypalConfiguration.GetAPIContext();

                //Create is a Payment class function which actually sends the payment details
                //to the paypal API for the payment. The function is passed with the ApiContext
                //which we received above.

                Payment createdPayment = pymnt.Create(apiContext);

                //if the createdPayment.state is "approved" it means the payment was successful else not

                if (createdPayment.state.ToLower() != "approved")
                {
                    //return View("FailureView");
                }
            }
            catch (PayPal.PayPalException ex)
            {
                //Logger.Log("Error: " + ex.Message);
                //return View("FailureView");
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
