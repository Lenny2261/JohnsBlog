using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogProject.Models;
using Microsoft.AspNet.Identity;

namespace BlogProject.Controllers
{
    [RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        [Authorize(Roles = "Admin, Mod")]
        public ActionResult Index()
        {
            
            var comments = db.Comments.Include(c => c.author).Include(c => c.post);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.authorID = new SelectList(db.Users, "Id", "firstName");
            ViewBag.postID = new SelectList(db.Posts, "id", "title");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,postID,body,created")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var slug = db.Posts.Find(comment.id).slug;

                comment.created = DateTimeOffset.Now;
                comment.authorID = User.Identity.GetUserId();
                comment.postID = comment.id;

                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "BlogPosts", new { slug = slug });
            }

            ViewBag.authorID = new SelectList(db.Users, "Id", "firstName", comment.authorID);
            ViewBag.postID = new SelectList(db.Posts, "id", "title", comment.postID);
            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles = "Admin, Mod")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.authorID = new SelectList(db.Users, "Id", "firstName", comment.authorID);
            ViewBag.postID = new SelectList(db.Posts, "id", "title", comment.postID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Mod")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,postID,authorID,body,created,updated,updateReason")] Comment comment)
        {
            if (ModelState.IsValid)
            {

                comment.updated = DateTimeOffset.Now;

                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.authorID = new SelectList(db.Users, "Id", "firstName", comment.authorID);
            ViewBag.postID = new SelectList(db.Posts, "id", "title", comment.postID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Admin, Mod")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [Authorize(Roles = "Admin, Mod")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
