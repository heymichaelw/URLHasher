﻿using Microsoft.AspNet.Identity;
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

        [Route("s/{Shorturl}")]
        public ActionResult Send(string Shorturl)
        {
            Click newclick = new Click()
            {
                Short = Shorturl,
                Accessed = DateTime.Now
            };
            db.Clicks.Add(newclick);
            db.SaveChanges();


            URL myurl = db.URLs.Where(u => u.Short == Shorturl).FirstOrDefault();
            string longurl = myurl.Long;
            return new RedirectResult(longurl);
        }

        // GET: URL
        public ActionResult Index()
        {
            var uRLs = db.URLs.Include(u => u.Owner);
            return View(uRLs.ToList().OrderByDescending(b => b.Created));
        }

        public ActionResult OtherIndex(string username)
        {
            var uRLs = db.URLs.Where(u => u.Owner.UserName == username);
            return View(uRLs.ToList().OrderByDescending(b => b.Created));
        }

        // GET: URL/Details/5
        public ActionResult Details(int? id)
        {
            URL uRL = db.URLs.Find(id);
            var me = User.Identity.GetUserId();
            bool isUpvoted = db.Upvotes.Where(u => (u.VotedURLId == id) && (u.VoterId == me)).Any();
            ViewBag.Owner = me;
            ViewBag.isUpvoted = isUpvoted;

            var clicks = db.Clicks.Where(c => c.Short == uRL.Short).ToList().Count();
            ViewBag.Clicks = clicks;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            if (uRL == null)
            {
                return HttpNotFound();
            }
            return View(uRL);
        }

        [HttpPost]
        public ActionResult Details (int id)
        {
            URL url = db.URLs.Find(id);

            Upvote upvote = new Upvote
            {
                VoterId = User.Identity.GetUserId(),
                VotedURLId = url.Id
            };

            db.Upvotes.Add(upvote);
            db.SaveChanges();
            return RedirectToAction("Details");
        }

        // GET: URL/Create
        [Authorize]
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
                return RedirectToAction("ShortView", uRL );
            }

            return View(uRL);
        }

        // GET: URL/Edit/5
        public ActionResult Edit(int? id)
        {
            URL url = db.URLs.Find(id);
            var userId = User.Identity.GetUserId();
            var bookmark = db.URLs.SingleOrDefault(m => m.Id == url.Id && m.OwnerId == userId);
            if (bookmark == null)
            {
                return new HttpNotFoundResult();
            }

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

        public ActionResult DeleteUpvote(int id)
        {
            Upvote upvote = db.Upvotes.Where(u => u.Id == id).FirstOrDefault();
            db.Upvotes.Remove(upvote);
            db.SaveChanges();
            return RedirectToAction("MyBookmarks");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        [Route("u/{Username}")]
        public ActionResult MyBookmarks(string Username)
        {
            

            var user = User.Identity.GetUserName();
            
            if (user == Username)
            {
                var bookmarks = db.URLs.Include(b => b.Owner).Where(b => b.Owner.UserName == Username);
                return View(bookmarks.ToList().OrderByDescending(b => b.Created));
            }
            else
            {
                var bookmarks = db.URLs.Include(b => b.Owner).Where(b => b.Owner.UserName == Username);
                return RedirectToAction("OtherIndex", new { Username });
            }

            
        }

        public ActionResult MyBookmarks()
        {
            var userid = User.Identity.GetUserId();

            ViewBag.Likes = db.Upvotes.Include(u => u.VotedURL).Where(u => u.VoterId == userid).ToList();

            var user = User.Identity.GetUserName();
            var bookmarks = db.URLs.Where(b => b.Owner.UserName == user);
            return View(bookmarks);
        }

        public ActionResult ShortView(URL uRL)
        {
            var urlshort = uRL.Short;
            ViewBag.Short = "http://localhost:57714/s/" + $"{uRL.Short}";
            return View(uRL);
        }
    }
}
