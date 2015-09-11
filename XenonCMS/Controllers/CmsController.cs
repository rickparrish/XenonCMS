// TODO Setup a redirect system, so for example /news could be redirected to /blog
// TODO Every time a page is updated, put it in a history table so old versions can be restored
// TODO Add ability to import from GetSimple (maybe have import happen at Install time?)
// TODO Add Edit button to pages if logged in as admin.  After edit is done, return to page.  Need to handle changed slug
// TODO Maybe don't even need a catch-all for cms urls if rammfar can catch and handle 404s
// TODO Log 404s so then a dashboard page can be created showing common 404s.  Add option to ignore certain ones (ie maybe want to ignore robots.txt or favicon.ico instead of putting one in place)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XenonCMS.Classes;
using XenonCMS.Models;
using XenonCMS.ViewModels.Cms;

namespace XenonCMS.Controllers
{
    public class CmsController : Controller
    {
        //
        // GET: /Cms/
        public ActionResult Slug(string slug)
        {
            // Clean up the url parameter
            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "home";
            }
            else
            {
                slug = slug.ToLower().Trim('/');
                if (slug.EndsWith("/index")) slug = slug.Substring(0, slug.LastIndexOf("/"));
                if (string.IsNullOrWhiteSpace(slug))
                {
                    slug = "home";
                }
            }

            // Check if we cached the page
            SitePage Page = DatabaseCache.GetSitePage(ControllerContext.RequestContext.HttpContext, slug);
            if (Page == null)
            {
                string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);
                using (ApplicationDbContext DB = new ApplicationDbContext())
                {
                    Page = DB.SitePages.SingleOrDefault(x => (x.Slug == slug) && (x.Site.Domain == RequestDomain));
                }
                DatabaseCache.AddSitePage(ControllerContext.RequestContext.HttpContext, Page);
            }

            // Ensure retrieved page is valid
            if (Page == null)
            {
                if (Globals.IsNewSite(ControllerContext.RequestContext.HttpContext))
                {
                    return RedirectToAction("Install", "Cms");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                ViewBag.Layout = Page.Layout;
                ViewBag.Title = Page.Title;
                ViewBag.ShowTitleOnPage = Page.ShowTitleOnPage;
                ViewBag.Html = Page.Html;
                ViewBag.DateLastUpdated = Page.DateLastUpdated;
                return View();
            }
        }

        //
        // GET: /CmsFile/File/
        public FilePathResult File(string filename)
        {
            string AbsoluteFilename = SiteHelper.AbsoluteFilename(ControllerContext.RequestContext.HttpContext, filename);
            if (System.IO.File.Exists(AbsoluteFilename))
            {
                return File(AbsoluteFilename, MimeMapping.GetMimeMapping(AbsoluteFilename));
            }
            else
            {
                throw new HttpException(404, "Not found");
            }
        }

        //
        // GET: /Cms/Install/
        public ActionResult Install()
        {
            if (Globals.IsNewSite(ControllerContext.RequestContext.HttpContext))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Cms/Install/
        [HttpPost]
        [Authorize(Roles = "GlobalAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Install(Install model)
        {
            if (Globals.IsNewSite(ControllerContext.RequestContext.HttpContext))
            {
                if (ModelState.IsValid)
                {
                    using (ApplicationDbContext DB = new ApplicationDbContext())
                    {
                        string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);
                        Site Site = new Site();
                        Site.BlogPosts = new List<SiteBlogPost>();
                        Site.Pages = new List<SitePage>();

                        SiteBlogPost NewBlogPost = new SiteBlogPost();
                        NewBlogPost.DateLastUpdated = DateTime.Now;
                        NewBlogPost.DatePosted = DateTime.Now;
                        NewBlogPost.FullPostText = "XenonCMS has been successfully installed and is ready for use on " + RequestDomain + "!";
                        NewBlogPost.Slug = "xenoncms-installed";
                        NewBlogPost.Title = "XenonCMS Installed";
                        Site.BlogPosts.Add(NewBlogPost);

                        Site.ContactEmail = "website@" + RequestDomain;
                        Site.Domain = RequestDomain;
                        Site.NavBarInverted = false;

                        SitePage NewPageHome = new SitePage();
                        NewPageHome.DateAdded = DateTime.Now;
                        NewPageHome.DateLastUpdated = DateTime.Now;
                        NewPageHome.DisplayOrder = 1;
                        NewPageHome.Html = "XenonCMS has been successfully installed and is ready for use on " + RequestDomain + "!";
                        NewPageHome.Layout = "JumbotronNoSidebar";
                        NewPageHome.Text = "Home";
                        NewPageHome.Slug = "home";
                        NewPageHome.ParentId = 0;
                        NewPageHome.RequireAdmin = false;
                        NewPageHome.RightAlign = false;
                        NewPageHome.ShowInMenu = true;
                        NewPageHome.ShowTitleOnPage = true;
                        NewPageHome.Title = "XenonCMS Installed";
                        Site.Pages.Add(NewPageHome);

                        SitePage NewPageBlog = new SitePage();
                        NewPageBlog.DateAdded = DateTime.Now;
                        NewPageBlog.DateLastUpdated = DateTime.Now;
                        NewPageBlog.DisplayOrder = 2;
                        NewPageBlog.Html = "N/A";
                        NewPageBlog.Layout = "NormalSidebar";
                        NewPageBlog.Text = "Blog";
                        NewPageBlog.Slug = "blog";
                        NewPageBlog.ParentId = 0;
                        NewPageBlog.RequireAdmin = false;
                        NewPageBlog.RightAlign = false;
                        NewPageBlog.ShowInMenu = true;
                        NewPageBlog.ShowTitleOnPage = true;
                        NewPageBlog.Title = "Blog";
                        Site.Pages.Add(NewPageBlog);

                        SitePage NewPageContact = new SitePage();
                        NewPageContact.DateAdded = DateTime.Now;
                        NewPageContact.DateLastUpdated = DateTime.Now;
                        NewPageContact.DisplayOrder = 3;
                        NewPageContact.Html = "N/A";
                        NewPageContact.Layout = "NormalSidebar";
                        NewPageContact.Text = "Contact";
                        NewPageContact.Slug = "contact";
                        NewPageContact.ParentId = 0;
                        NewPageContact.RequireAdmin = false;
                        NewPageContact.RightAlign = false;
                        NewPageContact.ShowInMenu = true;
                        NewPageContact.ShowTitleOnPage = true;
                        NewPageContact.Title = "Contact";
                        Site.Pages.Add(NewPageContact);

                        Site.Sidebar = "<div class=\"panel panel-default\"><div class=\"panel-heading\"><h3 class=\"panel-title\">XenonCMS Installed</h3></div><div class=\"panel-body\">XenonCMS has been successfully installed and is ready for use on " + RequestDomain + "!</div></div>";
                        Site.Theme = "Cerulean";
                        Site.Title = RequestDomain;

                        DB.Sites.Add(Site);
                        DB.SaveChanges();

                        DatabaseCache.AddSite(ControllerContext.RequestContext.HttpContext, Site);
                        DatabaseCache.ResetAdminIPs(ControllerContext.RequestContext.HttpContext);
                        DatabaseCache.ResetBlogPosts(ControllerContext.RequestContext.HttpContext);
                        DatabaseCache.ResetNavMenuItems(ControllerContext.RequestContext.HttpContext);
                        DatabaseCache.ResetSidebars(ControllerContext.RequestContext.HttpContext);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
    }
}