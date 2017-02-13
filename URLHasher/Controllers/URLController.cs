using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using URLHasher.Models;

namespace URLHasher.Controllers
{
    public class URLController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private string GetShort()
        {
            string longurl = Request.Form["Long"];

            byte[] byteData = Encoding.UTF8.GetBytes(longurl);
            Stream inputStream = new MemoryStream(byteData);

            using (SHA256 shaM = new SHA256Managed())
            {
                var result = shaM.ComputeHash(inputStream);
                string output1 = BitConverter.ToString(result);
                string output = output1.Replace("-", "").Substring(0, 5);

                return output;
            }
        }

        // GET: URL
        public ActionResult Index()
        {
            var uRLs = db.URLs.Include(u => u.Owner);
            return View(uRLs.ToList());
        }

        // GET: URL/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            URL uRL = db.URLs.Find(id);
            if (uRL == null)
            {
                return HttpNotFound();
            }
            return View(uRL);
        }

        // GET: URL/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: URL/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Long,Short,Created,OwnerId")] URL uRL)
        {
            if (ModelState.IsValid)
            {
                uRL.OwnerId = User.Identity.GetUserId();
                uRL.Created = DateTime.Now;
                uRL.Short = GetShort();

                db.URLs.Add(uRL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(uRL);
        }

        // GET: URL/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            URL uRL = db.URLs.Find(id);
            if (uRL == null)
            {
                return HttpNotFound();
            }
            return View(uRL);
        }

        // POST: URL/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Long,Short,Created,OwnerId")] URL uRL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uRL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(uRL);
        }

        // GET: URL/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            URL uRL = db.URLs.Find(id);
            if (uRL == null)
            {
                return HttpNotFound();
            }
            return View(uRL);
        }

        // POST: URL/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            URL uRL = db.URLs.Find(id);
            db.URLs.Remove(uRL);
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
