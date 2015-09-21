using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using XenonCMS.Classes;
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
                    using (var DB = new ApplicationDbContext())
                    {
                        using (var Zip = new ZipArchive(file.InputStream))
                        {
                            HandleGetSimpleBootstrap3SettingsXml(Zip, DB);
                            HandleGetSimpleNewsManagerPosts(Zip, DB);
                            HandleGetSimplePages(Zip, DB);
                            HandleGetSimpleWebsiteXml(Zip, DB);
                            // TODO Handle uploads (put in /SiteFiles)
                            // TODO Handle archived versions of pages?
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
        private Dictionary<string, XmlDocument> GetSimpleGetXmlDocuments(ZipArchive zip, string startsWith)
        {
            var Result = new Dictionary<string, XmlDocument>();

            var ZAEs = zip.Entries.Where(x => (x.FullName.StartsWith("data/" + startsWith) || x.FullName.StartsWith(startsWith)) && (Path.GetExtension(x.Name) == ".xml"));
            foreach (var ZAE in ZAEs)
            {
                // Open the file for reading
                using (var InStream = ZAE.Open())
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
                    Result.Add(ZAE.FullName, XmlDoc);
                }
            }

            return Result;
        }

        private void HandleGetSimpleBootstrap3SettingsXml(ZipArchive zip, ApplicationDbContext DB)
        {
            string RequestDomain = Globals.GetRequestDomain();

            // Handle the Bootstrap3Settings.xml file
            var Bootstrap3SettingsXmlFile = GetSimpleGetXmlDocuments(zip, "other/Bootstrap3Settings.xml");
            foreach (var KVP in Bootstrap3SettingsXmlFile)
            {
                var Site = DB.Sites.Single(x => x.Domain == RequestDomain);
                Site.ContactEmail = KVP.Value.DocumentElement.SelectSingleNode("ContactEmail").InnerText;
                // TODO GetSimple has DisplayOtherThemes
                Site.NavBarInverted = (KVP.Value.DocumentElement.SelectSingleNode("InvertNavigationBar").InnerText == "true");
                Site.Theme = KVP.Value.DocumentElement.SelectSingleNode("SelectedTheme").InnerText;
                // TODO GetSimple has TrackingId
                DB.SaveChanges();
                Caching.ResetSite();
            }
        }

        private void HandleGetSimpleNewsManagerPosts(ZipArchive zip, ApplicationDbContext DB)
        {
            string RequestDomain = Globals.GetRequestDomain();
            int SiteId = DB.Sites.Single(x => x.Domain == RequestDomain).Id;

            // Handle the NewsManager posts files
            var PostsFiles = GetSimpleGetXmlDocuments(zip, "posts/");
            foreach (var KVP in PostsFiles)
            {
                // Update existing or add new post
                // TODO GetSimple has a "tags" tag
                string Slug = Path.GetFileNameWithoutExtension(KVP.Key);
                var SBP = DB.SiteBlogPosts.SingleOrDefault(x => (x.Site.Domain == RequestDomain) && (x.Slug == Slug)) ?? new SiteBlogPost();
                SBP.DateLastUpdated = Convert.ToDateTime(KVP.Value.DocumentElement.SelectSingleNode("date").InnerText);
                SBP.DatePosted = Convert.ToDateTime(KVP.Value.DocumentElement.SelectSingleNode("date").InnerText);
                SBP.FullPostText = HttpUtility.HtmlDecode(KVP.Value.DocumentElement.SelectSingleNode("content").InnerText);
                SBP.SiteId = SiteId;
                SBP.Slug = Slug;
                SBP.Title = HttpUtility.HtmlDecode(KVP.Value.DocumentElement.SelectSingleNode("title").InnerText);
                if (SBP.Id <= 0) DB.SiteBlogPosts.Add(SBP);
                DB.SaveChanges();
                Caching.ResetBlogPosts();
            }
        }

        private void HandleGetSimplePages(ZipArchive zip, ApplicationDbContext DB)
        {
            string RequestDomain = Globals.GetRequestDomain();
            int SiteId = DB.Sites.Single(x => x.Domain == RequestDomain).Id;

            // Handle the pages files
            var TopLevelPages = new Dictionary<int, string>();
            var PagesFiles = GetSimpleGetXmlDocuments(zip, "pages/");
            foreach (var KVP in PagesFiles.OrderBy(x => x.Value.DocumentElement.SelectSingleNode("parent").InnerText))
            {
                // Get slug so we can skip built-in pages
                string Parent = KVP.Value.DocumentElement.SelectSingleNode("parent").InnerText;
                string Url = KVP.Value.DocumentElement.SelectSingleNode("url").InnerText;
                string Slug = string.IsNullOrWhiteSpace(Parent) ? Url : Parent + "/" + Url;
                if (Slug == "index") Slug = "home";
                if (Slug == "news") Slug = "blog";
                if ((Slug == "blog") || (Slug == "contact"))
                {
                    // Only update the display order for these built-ins
                    var SP = DB.SitePages.SingleOrDefault(x => (x.Site.Domain == RequestDomain) && (x.Slug == Slug));
                    if (SP != null)
                    {
                        SP.DisplayOrder = Convert.ToInt32(KVP.Value.DocumentElement.SelectSingleNode("menuOrder").InnerText);
                        // TODO Can this be used on the blog/contact header? SP.Title = HttpUtility.HtmlDecode(KVP.Value.DocumentElement.SelectSingleNode("title").InnerText);
                        DB.SaveChanges();

                        Caching.ResetPages();
                    }
                }
                else
                {
                    // Update existing or add new page
                    // TODO GetSimple has a "meta" tag, 
                    var SP = DB.SitePages.SingleOrDefault(x => (x.Site.Domain == RequestDomain) && (x.Slug == Slug)) ?? new SitePage();
                    SP.DateAdded = Convert.ToDateTime(KVP.Value.DocumentElement.SelectSingleNode("pubDate").InnerText);
                    SP.DateLastUpdated = Convert.ToDateTime(KVP.Value.DocumentElement.SelectSingleNode("pubDate").InnerText);
                    SP.DisplayOrder = Convert.ToInt32(KVP.Value.DocumentElement.SelectSingleNode("menuOrder").InnerText);
                    SP.Html = HttpUtility.HtmlDecode(KVP.Value.DocumentElement.SelectSingleNode("content").InnerText);
                    SP.ParentId = (TopLevelPages.ContainsValue(Parent)) ? TopLevelPages.Single(x => x.Value == Parent).Key : 0;
                    SP.RequireAdmin = false; // TODO "private" tag that could map to requireadmin
                    SP.RightAlign = false;
                    SP.ShowInMenu = (KVP.Value.DocumentElement.SelectSingleNode("menuStatus").InnerText == "Y");
                    SP.ShowTitleOnPage = true;
                    SP.SiteId = SiteId;
                    SP.Slug = Slug;
                    SP.LinkText = HttpUtility.HtmlDecode(KVP.Value.DocumentElement.SelectSingleNode("menu").InnerText);
                    SP.Title = HttpUtility.HtmlDecode(KVP.Value.DocumentElement.SelectSingleNode("title").InnerText);

                    if (SP.Id <= 0) DB.SitePages.Add(SP);
                    DB.SaveChanges();

                    Caching.ResetPages();

                    if (string.IsNullOrWhiteSpace(Parent)) TopLevelPages.Add(SP.Id, SP.Slug);
                }
            }
        }

        private void HandleGetSimpleWebsiteXml(ZipArchive zip, ApplicationDbContext DB)
        {
            string RequestDomain = Globals.GetRequestDomain();

            // Handle the Bootstrap3Settings.xml file
            var Bootstrap3SettingsXmlFile = GetSimpleGetXmlDocuments(zip, "other/website.xml");
            foreach (var KVP in Bootstrap3SettingsXmlFile)
            {
                var Site = DB.Sites.Single(x => x.Domain == RequestDomain);
                Site.Title = HttpUtility.HtmlDecode(KVP.Value.DocumentElement.SelectSingleNode("SITENAME").InnerText);
                DB.SaveChanges();
                Caching.ResetSite();
            }
        }
        #endregion
    }
}