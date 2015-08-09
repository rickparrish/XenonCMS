using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using XenonCMS.Helpers;
using XenonCMS.Models;

namespace XenonCMS.Areas.Admin.Controllers
{
    public class ImportController : Controller
    {
        // GET: Admin/Import/GetSimple
        public ActionResult GetSimple()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetSimple(HttpPostedFileBase file)
        {
            if ((file == null) || (file.ContentLength <= 0))
            {
                // TODO Display an error
            }
            else
            {
                try
                {
                    using (var DB = new XenonCMSContext())
                    {
                        using (var Zip = new ZipArchive(file.InputStream))
                        {
                            HandleGetSimpleNewsManagerPosts(Zip, DB);
                            HandleGetSimplePages(Zip, DB);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO Archive is likely invalid?
                }
            }

            return View();
        }

        #region GetSimple Helper Methods
        private void HandleGetSimpleNewsManagerPosts(ZipArchive zip, XenonCMSContext DB)
        {
            string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);
            int SiteId = DB.Sites.Single(x => x.Domain == RequestDomain).Id;

            // Handle the NewsManager posts files
            var PostsFiles = zip.Entries.Where(x => (x.FullName.StartsWith("data/posts/") || x.FullName.StartsWith("posts/")) && (Path.GetExtension(x.Name) == ".xml"));
            foreach (var PostsFile in PostsFiles)
            {
                string Slug = Path.GetFileNameWithoutExtension(PostsFile.Name);

                // Open the file for reading
                using (var InStream = PostsFile.Open())
                {
                    // Read each byte and convert to xml string
                    var InBytes = new List<byte>();
                    while (true)
                    {
                        var InByte = InStream.ReadByte();
                        if (InByte == -1) break;
                        InBytes.Add((byte)InByte);
                    }

                    // Parse the xml string
                    XmlDocument XmlDoc = new XmlDocument();
                    XmlDoc.LoadXml(Encoding.UTF8.GetString(InBytes.ToArray()));

                    // Update existing or add new post
                    var SBP = DB.SiteBlogPosts.SingleOrDefault(x => (x.Site.Domain == RequestDomain) && (x.Slug == Slug)) ?? new SiteBlogPost();
                    SBP.DateLastUpdated = Convert.ToDateTime(XmlDoc.DocumentElement.SelectSingleNode("date").InnerText);
                    SBP.DatePosted = Convert.ToDateTime(XmlDoc.DocumentElement.SelectSingleNode("date").InnerText);
                    SBP.FullPostText = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("content").InnerText);
                    SBP.SiteId = SiteId;
                    SBP.Slug = Slug;
                    SBP.Title = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("title").InnerText);
                    if (SBP.Id <= 0) DB.SiteBlogPosts.Add(SBP);
                    DB.SaveChanges();
                    DatabaseCache.ResetBlogIndex(ControllerContext.RequestContext.HttpContext);
                }
            }
        }

        private void HandleGetSimplePages(ZipArchive zip, XenonCMSContext DB)
        {
            string RequestDomain = Globals.GetRequestDomain(ControllerContext.RequestContext.HttpContext);
            int SiteId = DB.Sites.Single(x => x.Domain == RequestDomain).Id;

            // Handle the pages files
            var PagesFiles = zip.Entries.Where(x => (x.FullName.StartsWith("data/pages/") || x.FullName.StartsWith("pages/")) && (Path.GetExtension(x.Name) == ".xml"));
            foreach (var PagesFile in PagesFiles)
            {
                // Open the file for reading
                using (var InStream = PagesFile.Open())
                {
                    // Read each byte and convert to xml string
                    var InBytes = new List<byte>();
                    while (true)
                    {
                        var InByte = InStream.ReadByte();
                        if (InByte == -1) break;
                        InBytes.Add((byte)InByte);
                    }

                    // Parse the xml string
                    XmlDocument XmlDoc = new XmlDocument();
                    XmlDoc.LoadXml(Encoding.UTF8.GetString(InBytes.ToArray()));

                    // Check if the page exists TODO Test with a parent
                    string Parent = XmlDoc.DocumentElement.SelectSingleNode("parent").InnerText;
                    string Url = XmlDoc.DocumentElement.SelectSingleNode("url").InnerText;
                    string Slug = string.IsNullOrWhiteSpace(Parent) ? Url : Parent + "/" + Url;
                    if ((Slug == "blog") || (Slug == "contact") || (Slug == "news"))
                    {
                        // TODO Skipping these built-ins
                    }
                    else
                    {
                        if (Slug == "index") Slug = "home";

                        // Update existing or add new page
                        var SP = DB.SitePages.SingleOrDefault(x => (x.Site.Domain == RequestDomain) && (x.Slug == Slug)) ?? new SitePage();
                        // TODO "meta" tag, 
                        SP.DateAdded = Convert.ToDateTime(XmlDoc.DocumentElement.SelectSingleNode("pubDate").InnerText);
                        SP.DateLastUpdated = Convert.ToDateTime(XmlDoc.DocumentElement.SelectSingleNode("pubDate").InnerText);
                        SP.DisplayOrder = Convert.ToInt32(XmlDoc.DocumentElement.SelectSingleNode("menuOrder").InnerText);
                        SP.Html = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("content").InnerText);
                        SP.Layout = "NormalNoSidebar"; // TODO there is a "template" tag
                        SP.ParentId = 0; // TODO there is a "parent" tag
                        SP.RequireAdmin = false; // TODO "private" tag
                        SP.RightAlign = false;
                        SP.ShowInMenu = (XmlDoc.DocumentElement.SelectSingleNode("menuStatus").InnerText == "Y");
                        SP.ShowTitleOnPage = true;
                        SP.SiteId = SiteId;
                        SP.Slug = Slug;
                        SP.Text = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("menu").InnerText); // TODO Rename text to something more intuitive
                        SP.Title = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("title").InnerText);
                        if (SP.Id <= 0) DB.SitePages.Add(SP);
                        DB.SaveChanges();
                        DatabaseCache.ResetNavMenuItems(ControllerContext.RequestContext.HttpContext);
                        DatabaseCache.RemoveSitePage(ControllerContext.RequestContext.HttpContext, Slug);
                    }
                }
            }
        }
        #endregion
    }
}