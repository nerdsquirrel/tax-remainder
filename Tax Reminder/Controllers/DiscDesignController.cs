using Rotativa;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Tax_Reminder.App_Start;
using Tax_Reminder.DbContext;
using Tax_Reminder.Models;

namespace Tax_Reminder.Controllers
{
    [Authorize]
    public class DiscDesignController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: DiscDesign
        public ActionResult Index()
        {
            return View(db.DiscDesignModels.ToList());
        }

        public ActionResult PdfFront()
        {
            return View();
        }

        public ActionResult BackView()
        {
            return View();
        }

        public ActionResult GeneratePDF()
        {
            return new ViewAsPdf("BackView");
        }

        /*public ActionResult ExportPDF()
        {
            return new ActionAsPdf("HTMLtoPDF")
            {
                FileName = Server.MapPath("~/Content/Disc.pdf")
            };
        }*/

        // GET: DiscDesign/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscDesignModel discDesignModel = db.DiscDesignModels.Find(id);
            if (discDesignModel == null)
            {
                return HttpNotFound();
            }
            return View(discDesignModel);
        }

        // GET: DiscDesign/Create
        [Admin]
        public ActionResult Create()
        {
            return View();
        }

        // GET: DiscDesign/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscDesignModel discDesignModel = db.DiscDesignModels.Find(id);
            if (discDesignModel == null)
            {
                return HttpNotFound();
            }
            DiscDesignViewModel dvm = new DiscDesignViewModel
            {
                Id = discDesignModel.Id,
                VehicleName = discDesignModel.VehicleName,
                TaxDay = discDesignModel.TaxDateTime.Day,
                TaxMonth = discDesignModel.TaxDateTime.Month,
                TaxYear = discDesignModel.TaxDateTime.Year,
                MotDay = discDesignModel.MotDateTime.Day,
                MotMonth = discDesignModel.MotDateTime.Month,
                MotYear = discDesignModel.MotDateTime.Year
            };
            return View(dvm);
        }

        // POST: DiscDesign/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DiscDesignViewModel discDesignView)
        {
            if (ModelState.IsValid)
            {
                var discDesignModel = new DiscDesignModel()
                {
                    Id = discDesignView.Id,
                    VehicleName = discDesignView.VehicleName,
                    TaxDateTime = DateTime.Parse(string.Format("{0} {1} {2} ", discDesignView.TaxDay, discDesignView.TaxMonth, discDesignView.TaxYear)),
                    MotDateTime = DateTime.Parse(string.Format("{0} {1} {2} ", discDesignView.MotDay, discDesignView.MotMonth, discDesignView.MotYear))
                };

                db.Entry(discDesignModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(discDesignView);
        }

        // GET: DiscDesign/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscDesignModel discDesignModel = db.DiscDesignModels.Find(id);
            if (discDesignModel == null)
            {
                return HttpNotFound();
            }
            return View(discDesignModel);
        }

        // POST: DiscDesign/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DiscDesignModel discDesignModel = db.DiscDesignModels.Find(id);
            db.DiscDesignModels.Remove(discDesignModel);
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
