// TODO Every time a page is updated, put it in a history table so old versions can be restored
// TODO Add ability to import from GetSimple (maybe have import happen at Install time?)
// TODO Add Edit button to pages if logged in as admin.  After edit is done, return to page.  Need to handle changed slug
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
            if (Caching.GetSite() == null)
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
            if (Caching.GetSite() == null)
            {
                if (ModelState.IsValid)
                {
                    using (ApplicationDbContext DB = new ApplicationDbContext())
                    {
                        string RequestDomain = Globals.GetRequestDomain();
                        Site Site = new Site();

                        Site.ContactEmail = "website@" + RequestDomain;
                        Site.Domain = RequestDomain;
                        Site.NavBarInverted = false;
                        Site.Sidebar = "<div class=\"panel panel-default\"><div class=\"panel-heading\"><h3 class=\"panel-title\">XenonCMS Installed</h3></div><div class=\"panel-body\">XenonCMS has been successfully installed and is ready for use on " + RequestDomain + "!</div></div>";
                        Site.Theme = "Cerulean";
                        Site.Title = RequestDomain;

                        Site.BlogPosts.Add(new SiteBlogPost() {
                            DateLastUpdated = DateTime.Now,
                            DatePosted = DateTime.Now,
                            FullPostText = "XenonCMS has been successfully installed and is ready for use on " + RequestDomain + "!",
                            Slug = "xenoncms-installed",
                            Title = "XenonCMS Installed"
                        
                        });

                        Site.Pages.Add(new SitePage() {
                            DateAdded = DateTime.Now,
                            DateLastUpdated = DateTime.Now,
                            DisplayOrder = 1,
                            Html = "<div class=\"jumbotron\"><h2>XenonCMS Installed</h2><p>XenonCMS has been successfully installed and is ready for use on " + RequestDomain + "!</p></div>",
                            LinkText = "Home",
                            ParentId = null,
                            RequireAdmin = false,
                            RightAlign = false,
                            ShowInMenu = true,
                            ShowTitleOnPage = false,
                            Slug = "home",
                            Title = "XenonCMS Installed"
                        });

                        Site.Pages.Add(new SitePage()
                        {
                            DateAdded = DateTime.Now,
                            DateLastUpdated = DateTime.Now,
                            DisplayOrder = 2,
                            Html = null,
                            LinkText = "Blog",
                            ParentId = null,
                            RequireAdmin = false,
                            RightAlign = false,
                            ShowInMenu = true,
                            ShowTitleOnPage = true,
                            Slug = "blog",
                            Title = RequestDomain + " Blog"                            
                        });

                        Site.Pages.Add(new SitePage()
                        {
                            DateAdded = DateTime.Now,
                            DateLastUpdated = DateTime.Now,
                            DisplayOrder = 3,
                            Html = null,
                            LinkText = "Contact",
                            ParentId = null,
                            RequireAdmin = false,
                            RightAlign = false,
                            ShowInMenu = true,
                            ShowTitleOnPage = true,
                            Slug = "contact",
                            Title = "Contact Form"
                        });

                        DB.Sites.Add(Site);
                        DB.SaveChanges();

                        Caching.ResetSite();
                        Caching.ResetBlogPosts();
                        Caching.ResetPages();
                        // TODOXXX Caching.ResetSidebars();
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
            SitePage Page = Caching.GetPage(slug);
            if (Page == null)
            {
                if (Caching.GetSite() == null)
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
                ViewBag.Title = Page.Title;
                ViewBag.ShowTitleOnPage = Page.ShowTitleOnPage;
                ViewBag.Html = Page.Html;
                ViewBag.DateLastUpdated = Page.DateLastUpdated;
                return View();
            }
        }
    }
}