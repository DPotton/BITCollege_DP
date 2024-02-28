using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BITCollege_DP.Data;
using BITCollege_DP.Models;

namespace BITCollege_DP.Controllers
{
    public class NextStudentsController : Controller
    {
        private BITCollege_DPContext db = new BITCollege_DPContext();

        // GET: NextStudents
        public ActionResult Index()
        {
            return View(NextStudent.GetInstance());
        }

        // GET: NextStudents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NextStudent nextStudent = db.NextStudents.Find(id);
            if (nextStudent == null)
            {
                return HttpNotFound();
            }
            return View(nextStudent);
        }

        // GET: NextStudents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NextStudents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NextUniqueNumberId,NextAvailableNumber")] NextStudent nextStudent)
        {
            if (ModelState.IsValid)
            {
                db.NextUniqueNumbers.Add(nextStudent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nextStudent);
        }

        // GET: NextStudents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NextStudent nextStudent = db.NextStudents.Find(id);
            if (nextStudent == null)
            {
                return HttpNotFound();
            }
            return View(nextStudent);
        }

        // POST: NextStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NextUniqueNumberId,NextAvailableNumber")] NextStudent nextStudent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nextStudent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nextStudent);
        }

        // GET: NextStudents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NextStudent nextStudent = db.NextStudents.Find(id);
            if (nextStudent == null)
            {
                return HttpNotFound();
            }
            return View(nextStudent);
        }

        // POST: NextStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NextStudent nextStudent = db.NextStudents.Find(id);
            db.NextUniqueNumbers.Remove(nextStudent);
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
