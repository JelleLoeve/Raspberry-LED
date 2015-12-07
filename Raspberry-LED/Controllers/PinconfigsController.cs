using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Raspberry_LED.Models;

namespace Raspberry_LED.Controllers
{
    public class PinconfigsController : Controller
    {
        private PinConfigDBContext db = new PinConfigDBContext();

        // GET: Pinconfigs
        public ActionResult Index()
        {
            return View(db.PinConfigs.ToList());
        }

        // GET: Pinconfigs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pinconfig pinconfig = db.PinConfigs.Find(id);
            if (pinconfig == null)
            {
                return HttpNotFound();
            }
            return View(pinconfig);
        }

        // GET: Pinconfigs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pinconfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,pinnumber,color,isSet,isOn")] Pinconfig pinconfig)
        {
            if (ModelState.IsValid)
            {
                db.PinConfigs.Add(pinconfig);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pinconfig);
        }

        // GET: Pinconfigs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pinconfig pinconfig = db.PinConfigs.Find(id);
            if (pinconfig == null)
            {
                return HttpNotFound();
            }
            return View(pinconfig);
        }

        // POST: Pinconfigs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,pinnumber,color,isSet,isOn")] Pinconfig pinconfig)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pinconfig).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pinconfig);
        }

        // GET: Pinconfigs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pinconfig pinconfig = db.PinConfigs.Find(id);
            if (pinconfig == null)
            {
                return HttpNotFound();
            }
            return View(pinconfig);
        }

        // POST: Pinconfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pinconfig pinconfig = db.PinConfigs.Find(id);
            db.PinConfigs.Remove(pinconfig);
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
