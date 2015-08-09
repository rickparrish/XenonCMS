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
                var BlogPost = DB.SiteBlogPosts.SingleOrDefault(x => (x.Site.Domain == RequestDomain) && (x.Slug == Slug));
                if (BlogPost == null)
                {
                    // Open the file for reading
                    using (var InStream = PostsFile.Open())
                    {
                        // Read each byte and convert to xml string
                        var InBytes = new List<byte>();
                        while (true)
                        {
                            var InByte = InStream.ReadByte();
                            if (InByte == -1)
                            {
                                break;
                            }
                            else
                            {
                                InBytes.Add((byte)InByte);
                            }
                        }
                        var Xml = Encoding.UTF8.GetString(InBytes.ToArray());

                        // Parse the xml string
                        XmlDocument XmlDoc = new XmlDocument();
                        XmlDoc.LoadXml(Xml);

                        // Insert the blog posts
                        // TODO "tags" element
                        DB.SiteBlogPosts.Add(new SiteBlogPost()
                        {
                            DateLastUpdated = Convert.ToDateTime(XmlDoc.DocumentElement.SelectSingleNode("date").InnerText),
                            DatePosted = Convert.ToDateTime(XmlDoc.DocumentElement.SelectSingleNode("date").InnerText),
                            FullPostText = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("content").InnerText),
                            SiteId = SiteId,
                            Slug = Slug,
                            Title = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("title").InnerText)
                        });
                        DB.SaveChanges();
                        DatabaseCache.ResetBlogIndex(ControllerContext.RequestContext.HttpContext);
                    }
                }
                else
                {
                    // TODO Skipping blog post, slug already exists
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
                        if (InByte == -1)
                        {
                            break;
                        }
                        else
                        {
                            InBytes.Add((byte)InByte);
                        }
                    }
                    var Xml = Encoding.UTF8.GetString(InBytes.ToArray());

                    // Parse the xml string
                    XmlDocument XmlDoc = new XmlDocument();
                    XmlDoc.LoadXml(Xml);

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
                        var Page = DB.SitePages.SingleOrDefault(x => (x.Site.Domain == RequestDomain) && (x.Slug == Slug));
                        if (Page == null)
                        {
                            DB.SitePages.Add(new SitePage()
                            {
                                // TODO "meta" tag, 
                                DateAdded = Convert.ToDateTime(XmlDoc.DocumentElement.SelectSingleNode("pubDate").InnerText),
                                DateLastUpdated = Convert.ToDateTime(XmlDoc.DocumentElement.SelectSingleNode("pubDate").InnerText),
                                DisplayOrder = Convert.ToInt32(XmlDoc.DocumentElement.SelectSingleNode("menuOrder").InnerText),
                                Html = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("content").InnerText),
                                Layout = "NormalNoSidebar", // TODO there is a "template" tag
                                ParentId = 0, // TODO there is a "parent" tag
                                RequireAdmin = false, // TODO "private" tag
                                RightAlign = false,
                                ShowInMenu = (XmlDoc.DocumentElement.SelectSingleNode("menuStatus").InnerText == "Y"),
                                ShowTitleOnPage = true,
                                SiteId = SiteId,
                                Slug = Slug,
                                Text = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("menu").InnerText), // TODO Rename text to something more intuitive
                                Title = HttpUtility.HtmlDecode(XmlDoc.DocumentElement.SelectSingleNode("title").InnerText)
                            });
                            DB.SaveChanges();
                            DatabaseCache.ResetNavMenuItems(ControllerContext.RequestContext.HttpContext);
                        }
                        else
                        {
                            // TODO Skipping page, slug already exists
                        }
                    }
                }
            }
        }
        #endregion
    }
}