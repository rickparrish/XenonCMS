using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using XenonCMS.Classes;
using XenonCMS.Models;
using XenonCMS.ViewModels.Blog;

namespace XenonCMS.Controllers
{
    public class BlogController : Controller
    {
        // GET: /Blog/
        public ActionResult Index()
        {
            return View(Caching.GetBlogPosts());
        }

        // GET: /Blog/Details/5
        public ActionResult Details(int id, int? year, int? month, int? day, string slug)
        {
            SiteBlogPost BlogPost = Caching.GetBlogPost(id);
            if (BlogPost == null)
            {
                // TODOX Should we be throwing a 404 exception here?
                return HttpNotFound();
            }
            else
            {
                if ((year == BlogPost.DatePosted.Year) && (month == BlogPost.DatePosted.Month) && (day == BlogPost.DatePosted.Day) && (slug == BlogPost.Slug))
                {
                    return View(BlogPost);
                }
                else
                {
                    return RedirectToRoutePermanent("BlogPost", BlogPost.ToRouteValues());
                }
            }
        }

        // GET: /Blog/Create
        [Authorize(Roles = "GlobalAdmin, SiteAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GlobalAdmin, SiteAdmin")]
        public ActionResult Create(Create viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var DB = new ApplicationDbContext())
                {
                    string RequestDomain = Globals.GetRequestDomain();

                    // View model to domain model
                    SiteBlogPost NewPost = ModelConverter.Convert<SiteBlogPost>(viewModel);

                    // Assign values for fields not on form
                    NewPost.DateLastUpdated = DateTime.Now;
                    NewPost.DatePosted = DateTime.Now;
                    NewPost.SiteId = DB.Sites.Single(x => x.Domain == RequestDomain).Id;

                    // Transform values
                    if (string.IsNullOrWhiteSpace(viewModel.Slug)) NewPost.Slug = NewPost.Title;
                    NewPost.Slug = Globals.GetSlug(NewPost.Slug, false); // No need to enforce uniqueness, since slug isn't actually used for lookup
                    NewPost.FullPostText = Globals.SaveImagesToDisk(NewPost.FullPostText);
                    NewPost.PreviewText = Globals.SaveImagesToDisk(NewPost.PreviewText);

                    // Save changes
                    DB.SiteBlogPosts.Add(NewPost);
                    DB.SaveChanges();

                    // Update cache
                    Caching.ResetBlogPosts();

                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(viewModel);
            }
        }

        // GET: /Blog/Edit/5
        [Authorize(Roles = "GlobalAdmin, SiteAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                using (var DB = new ApplicationDbContext())
                {
                    string RequestDomain = Globals.GetRequestDomain();

                    SiteBlogPost Post = DB.SiteBlogPosts.SingleOrDefault(x => (x.Id == id) && (x.Site.Domain == RequestDomain));
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
        }

        // POST: /Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GlobalAdmin, SiteAdmin")]
        public ActionResult Edit(Edit viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var DB = new ApplicationDbContext())
                {
                    string RequestDomain = Globals.GetRequestDomain();

                    SiteBlogPost EditedPost = DB.SiteBlogPosts.SingleOrDefault(x => (x.Id == viewModel.Id) && (x.Site.Domain == RequestDomain));
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
                        if (string.IsNullOrWhiteSpace(EditedPost.Slug)) EditedPost.Slug = EditedPost.Title;
                        EditedPost.Slug = Globals.GetSlug(EditedPost.Slug, false); // No need to enforce uniqueness, since slug isn't actually used for lookup
                        EditedPost.FullPostText = Globals.SaveImagesToDisk(EditedPost.FullPostText);
                        EditedPost.PreviewText = Globals.SaveImagesToDisk(EditedPost.PreviewText);

                        // Save changes
                        DB.Entry(EditedPost).State = EntityState.Modified;
                        DB.SaveChanges();

                        // Update cache
                        Caching.ResetBlogPosts();

                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                return View(viewModel);
            }
        }

        // GET: /Blog/Delete/5
        [Authorize(Roles = "GlobalAdmin, SiteAdmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                using (var DB = new ApplicationDbContext())
                {
                    string RequestDomain = Globals.GetRequestDomain();

                    SiteBlogPost Post = DB.SiteBlogPosts.SingleOrDefault(x => (x.Id == id) && (x.Site.Domain == RequestDomain));
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
        }

        // POST: /Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GlobalAdmin, SiteAdmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var DB = new ApplicationDbContext())
            {
                string RequestDomain = Globals.GetRequestDomain();

                SiteBlogPost Post = DB.SiteBlogPosts.SingleOrDefault(x => (x.Id == id) && (x.Site.Domain == RequestDomain));
                if (Post == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    // Update cache
                    Caching.ResetBlogPosts();

                    // Save changes
                    DB.SiteBlogPosts.Remove(Post);
                    DB.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
        }
    }
}
