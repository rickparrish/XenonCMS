// TODO Any User.IsInRole checks also need to check if they're a SiteAdmin for the CURRENT domain!
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using XenonCMS.Models;

namespace XenonCMS.Classes
{
    static public class Globals
    {
        static public string GetRequestDomain(HttpContextBase httpContext)
        {
            string Result = httpContext.Request.Url.Host.ToLower();
            if (Result.StartsWith("www.")) Result = Result.Substring(4);
            return Result;
        }

        // From: http://predicatet.blogspot.com/2009/04/improved-c-slug-generator-or-how-to.html
        // TODO Add ignored word filter
        static public string GetSlug(string text, bool allowSlash)
        {
            // remove accent characters, lowercase, and trim
            text = Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(text)).ToLower().Trim();
            
            // invalid chars to hyphen
            if (allowSlash)
            {
                text = Regex.Replace(text, @"[^a-z0-9/-]", "-");
            
                // convert multiple slashes into one slash
                text = Regex.Replace(text, @"/+", "/");
            }
            else
            {
                text = Regex.Replace(text, @"[^a-z0-9-]", "-");
            }

            // convert multiple hyphens into one hyphen, making sure we don't start/end with a hyphen
            text = Regex.Replace(text, @"-+", "-").Trim(new char[] { '-', '/' });

            return text;
        }

        // From: https://github.com/madskristensen/MiniBlog
        static public string SaveImagesToDisk(string html, HttpContextBase httpContext)
        {
            foreach (Match M in Regex.Matches(html, "src=\"(data:([^\"]+))\"(>.*?</a>)?"))
            {
                // Get a new guid to use when naming this image
                Guid ImageId = Guid.NewGuid();

                // Get the bytes of the image
                int StartIndex = M.Groups[2].Value.IndexOf("base64,", StringComparison.Ordinal) + "base64,".Length;
                byte[] ImageBytes = Convert.FromBase64String(M.Groups[2].Value.Substring(StartIndex));

                // Save the image to disk
                string Extension = "." + Regex.Match(M.Value, "data:([^/]+)/([a-z]+);base64").Groups[2].Value;
                if (string.IsNullOrWhiteSpace(Extension)) Extension = ".png"; // Default to .png if no extension was found
                if (Extension == ".jpeg") Extension = ".jpg";
                string AbsoluteFilename = SiteHelper.AbsoluteFilename(httpContext, ImageId + Extension, "Images");
                Directory.CreateDirectory(Path.GetDirectoryName(AbsoluteFilename));
                File.WriteAllBytes(AbsoluteFilename, ImageBytes);

                // Update the html to include a link instead of data: uri
                html = new Regex("src=\"(data:([^\"]+))\"").Replace(html, "src=\"" + VirtualPathUtility.ToAbsolute("~/Images/" + ImageId + Extension) + "\" alt=\"\"", 1);
            }

            return html;
        }
    }
}