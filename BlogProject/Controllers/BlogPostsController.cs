using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogProject.Models;
using BlogProject.Helpers;
using Microsoft.AspNet.Identity;
using PagedList;
using System.IO;

namespace BlogProject.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page, string sortCat, string currentFilter, string searchString, string currentSearch)
        {
            if(sortCat != null)
            {
                page = 1;
            }
            else
            {
                sortCat = currentFilter;
            }

            ViewBag.CurrentFilter = sortCat;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentSearch = searchString;

            var blogs = from b in db.Posts select b;

            blogs = blogs.Where(b => b.published == true);

            if (!String.IsNullOrEmpty(sortCat))
            {
                blogs = blogs.Where(b => b.category.categoryName == sortCat);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                blogs = blogs.Where(b => b.title.Contains(searchString) || b.body.Contains(searchString));
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(blogs.OrderByDescending(b => b.created).ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Unfinished(int? page, string sortCat, string currentFilter)
        {
            if (sortCat != null)
            {
                page = 1;
            }
            else
            {
                sortCat = currentFilter;
            }

            ViewBag.CurrentFilter = sortCat;

            var blogs = from b in db.Posts select b;

            blogs = blogs.Where(b => b.published == false);

            if (!String.IsNullOrEmpty(sortCat))
            {
                blogs = blogs.Where(b => b.category.categoryName == sortCat);
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(blogs.OrderByDescending(b => b.created).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Sidebar()
        {
            var blogs = from b in db.Posts select b;

            blogs = blogs.Where(b => b.published == true);

            return View(blogs.OrderByDescending(b => b.created).ToList());
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.FirstOrDefault(p => p.slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
           return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,slug,body,mediaURL,published,categoryID")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blogPost.title);

                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if (db.Posts.Any(p => p.slug == slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }

                if (ImageUploadValidator.IsWebpageFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.mediaURL = "/Uploads/" + fileName;
                }
                else
                {
                    blogPost.mediaURL = "/images/blog/post-1.jpg";
                }

                string abtractBodyText = blogPost.body;

                blogPost.abstractBody = abtractBodyText;
                blogPost.authorID = User.Identity.GetUserId();
                Convert.ToInt32(blogPost.categoryID);
                blogPost.slug = slug;
                blogPost.created = DateTimeOffset.Now;
                db.Posts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            string currentTitle = blogPost.title;
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,categoryID,created,slug,body,mediaURL,published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blogPost.title);

                if (String.IsNullOrWhiteSpace(slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if (slug != blogPost.slug && db.Posts.Any(p => p.slug == slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }

                if (ImageUploadValidator.IsWebpageFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.mediaURL = "/Uploads/" + fileName;
                }
                else
                {
                    blogPost.mediaURL = blogPost.mediaURL;
                }

                blogPost.updated = DateTimeOffset.Now;
                blogPost.authorID = User.Identity.GetUserId();
                blogPost.slug = slug;

                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.Posts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.Posts.Find(id);
            db.Posts.Remove(blogPost);
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
