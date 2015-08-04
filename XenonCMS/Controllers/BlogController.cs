using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using XenonCMS.Helpers;
using XenonCMS.Models;
using XenonCMS.ViewModels.Blog;

namespace XenonCMS.Controllers
{
    public class BlogController : Controller
    {
        private XenonCMSContext db = new XenonCMSContext();

        // GET: /Blog/
        public ActionResult Index()
        {
            return View(DatabaseCache.GetBlogIndex(db, ControllerContext.RequestContext.HttpContext));
        }

        // GET: /Blog/Details/5
        public ActionResult Details(int id, int? year, int? month, int? day, string slug)
        {
            Details ViewModel = DatabaseCache.GetBlogDetails(id, db, ControllerContext.RequestContext.HttpContext);
            if (ViewModel == null)
            {
                return HttpNotFound();
            }
            else
            {
                if ((year == ViewModel.DatePosted.Year) && (month == ViewModel.DatePosted.Month) && (day == ViewModel.DatePosted.Day) && (slug == ViewModel.Slug))
                {
                    return View(ViewModel);
                }
                else
                {
                    return RedirectToRoutePermanent("BlogPost", ViewModel.ToRouteValues());
                }
            }
        }

        // GET: /Blog/Create
        [IPAuthorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [IPAuthorize]
        public ActionResult Create(Create viewModel)
        {
            if (ModelState.IsValid)
            {
                string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);

                // View model to domain model
                SiteBlogPost NewPost = ModelConverter.Convert<SiteBlogPost>(viewModel);

                // Assign values for fields not on form
                NewPost.DateLastUpdated = DateTime.Now;
                NewPost.DatePosted = DateTime.Now;
                NewPost.SiteId = db.Sites.Single(x => x.Domain == RequestDomain).Id;
                
                // Transform values
                if (string.IsNullOrEmpty(viewModel.Slug)) NewPost.Slug = NewPost.Title;
                NewPost.Slug = Globals.GetSlug(NewPost.Slug, false); // No need to enforce uniqueness, since slug isn't actually used for lookup
                NewPost.FullPostText = Globals.SaveImagesToDisk(NewPost.FullPostText, ControllerContext.HttpContext);
                NewPost.PreviewText = Globals.SaveImagesToDisk(NewPost.PreviewText, ControllerContext.HttpContext);

                // Save changes
                db.SiteBlogPosts.Add(NewPost);
                db.SaveChanges();

                // Update cache
                DatabaseCache.ResetBlogIndex(ControllerContext.RequestContext.HttpContext);

                return RedirectToAction("Index");
            }
            else
            {
                return View(viewModel);
            }
        }

        // GET: /Blog/Edit/5
        [IPAuthorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);
                
                SiteBlogPost Post = db.SiteBlogPosts.SingleOrDefault(x => (x.Id == id) && (x.Site.Domain == RequestDomain));
                if (Post == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(ModelConverter.Convert<Edit>(Post));
                }
            }
        }

        // POST: /Blog/Edit/5
        [HttpPost]
        [IPAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Edit viewModel)
        {
            if (ModelState.IsValid)
            {
                string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);

                SiteBlogPost EditedPost = db.SiteBlogPosts.SingleOrDefault(x => (x.Id == viewModel.Id) && (x.Site.Domain == RequestDomain));
                if (EditedPost == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    // View model to domain model
                    ModelConverter.Convert(viewModel, EditedPost);

                    // Assign values for fields not on form
                    EditedPost.DateLastUpdated = DateTime.Now;
                    
                    // Transform values
                    if (string.IsNullOrEmpty(EditedPost.Slug)) EditedPost.Slug = EditedPost.Title;
                    EditedPost.Slug = Globals.GetSlug(EditedPost.Slug, false); // No need to enforce uniqueness, since slug isn't actually used for lookup
                    EditedPost.FullPostText = Globals.SaveImagesToDisk(EditedPost.FullPostText, ControllerContext.HttpContext);
                    EditedPost.PreviewText = Globals.SaveImagesToDisk(EditedPost.PreviewText, ControllerContext.HttpContext);

                    // Save changes
                    db.Entry(EditedPost).State = EntityState.Modified;
                    db.SaveChanges();

                    // Update cache
                    DatabaseCache.RemoveBlogDetails(viewModel.Id, ControllerContext.RequestContext.HttpContext);
                    DatabaseCache.ResetBlogIndex(ControllerContext.RequestContext.HttpContext);

                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(viewModel);
            }
        }

        // GET: /Blog/Delete/5
        [IPAuthorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);

                SiteBlogPost Post = db.SiteBlogPosts.SingleOrDefault(x => (x.Id == id) && (x.Site.Domain == RequestDomain));
                if (Post == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    return View(ModelConverter.Convert<Delete>(Post));
                }
            }
        }

        // POST: /Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [IPAuthorize]
        public ActionResult DeleteConfirmed(int id)
        {
            string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);

            SiteBlogPost Post = db.SiteBlogPosts.SingleOrDefault(x => (x.Id == id) && (x.Site.Domain == RequestDomain));
            if (Post == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Update cache
                DatabaseCache.RemoveBlogDetails(id, ControllerContext.RequestContext.HttpContext);
                DatabaseCache.ResetBlogIndex(ControllerContext.RequestContext.HttpContext);

                // Save changes
                db.SiteBlogPosts.Remove(Post);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
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
