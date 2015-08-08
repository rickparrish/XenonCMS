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
        #endregion
    }
}