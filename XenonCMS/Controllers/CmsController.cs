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
        // GET: /Cms/Install/
        public ActionResult Install()
        {
            if (Caching.GetSite(ControllerContext.RequestContext.HttpContext) == null)
            {
                return View();
            }
            else
            {
                return Redirect("~");
            }
        }

        //
        // POST: /Cms/Install/
        [HttpPost]
        [Authorize(Roles = "GlobalAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult Install(Install model)
        {
            if (Caching.GetSite(ControllerContext.RequestContext.HttpContext) == null)
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

                        Caching.ResetSite(ControllerContext.RequestContext.HttpContext);
                        Caching.ResetBlogPosts(ControllerContext.RequestContext.HttpContext);
                        Caching.ResetPages(ControllerContext.RequestContext.HttpContext);
                        // TODOXXX Caching.ResetSidebars(ControllerContext.RequestContext.HttpContext);
                    }

                    return Redirect("~");
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return Redirect("~");
            }
        }

        //
        // GET: /Cms/
        public ActionResult Slug(string slug)
        {
            // Get the page
            SitePage Page = Caching.GetPage(slug, ControllerContext.RequestContext.HttpContext);
            if (Page == null)
            {
                if (Caching.GetSite(ControllerContext.RequestContext.HttpContext) == null)
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
    }
}