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

namespace XenonCMS.Helpers
{
    static public class Globals
    {
        static private List<string> GetAdminIPs(HttpContextBase httpContext, bool globalOnly)
        {
            List<string> Result = DatabaseCache.GetAdminIPs(httpContext, globalOnly);
            if (Result == null)
            {
                Result = new List<string>();
                string RequestDomain = Globals.GetRequestDomain(httpContext);

                try
                {
                    using (XenonCMSContext DB = new XenonCMSContext())
                    {
                        // Global admin ips
                        foreach (var Row in DB.GlobalAdminIPs)
                        {
                            Result.Add(Row.Address);
                        }

                        if (!globalOnly)
                        {
                            // Site admin ips
                            foreach (var Row in DB.SiteAdminIPs.Where(x => x.Site.Domain == RequestDomain))
                            {
                                Result.Add(Row.Address);
                            }
                        }
                    }

                    // Cache what we have so far, so if the dns lookup is necessary, and takes awhile, we won't have multiple requests doing it
                    DatabaseCache.AddAdminIPs(httpContext, Result, globalOnly);

                    // Lookup any hostnames and convert to ip address
                    for (int i = 0; i < Result.Count; i++)
                    {
                        if (!Regex.IsMatch(Result[i], @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}", RegexOptions.Compiled))
                        {
                            // Not in the format of an IP, so convert hostname to IP
                            try
                            {
                                Result[i] = Dns.GetHostAddresses(Result[i])[0].ToString();
                            }
                            catch (Exception)
                            {
                                // Lookup failed, just ignore
                            }
                        }
                    }

                    // Re-cache, now that we have dns lookups completed
                    DatabaseCache.AddAdminIPs(httpContext, Result, globalOnly);
                }
                catch (Exception)
                {
                    // Something failed, just ignore
                }
            }

            return Result;
        }

        static public string GetRequestDomain(HttpContextBase httpContext)
        {
            string Result = httpContext.Request.Url.Host.ToLower();
            if (Result == "localhost")
            {
                Result = ConfigurationManager.AppSettings["XenonCMS:DebugDomain"];
            }
            else if (Result.StartsWith("www."))
            {
                Result = Result.Substring(4);
            }
            return Result;
        }

        // From: http://predicatet.blogspot.com/2009/04/improved-c-slug-generator-or-how-to.html
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

        static public bool IsNewSite(HttpContextBase httpContext)
        {
            Site Site = DatabaseCache.GetSite(httpContext);
            if (Site == null)
            {
                string RequestDomain = GetRequestDomain(httpContext);
                using (XenonCMSContext DB = new XenonCMSContext())
                {
                    Site = DB.Sites.SingleOrDefault(x => x.Domain == RequestDomain);
                }
                DatabaseCache.AddSite(httpContext, Site);
            }

            return (Site == null);
        }

        static public bool IsUserFromAdminIP(HttpContextBase httpContext)
        {
            List<string> AdminIPs = GetAdminIPs(httpContext, false);
            return AdminIPs.Contains(httpContext.Request.UserHostAddress);
        }

        static public bool IsUserFromGlobalAdminIP(HttpContextBase httpContext)
        {
            List<string> AdminIPs = GetAdminIPs(httpContext, true);
            return AdminIPs.Contains(httpContext.Request.UserHostAddress);
        }
        
        // From: https://github.com/madskristensen/MiniBlog
        static public string SaveImagesToDisk(string html, HttpContextBase httpContext)
        {
            foreach (Match match in Regex.Matches(html, "src=\"(data:([^\"]+))\"(>.*?</a>)?"))
            {
                // Get the bytes of the image
                int StartIndex = match.Groups[2].Value.IndexOf("base64,", StringComparison.Ordinal) + "base64,".Length;
                byte[] ImageBytes = Convert.FromBase64String(match.Groups[2].Value.Substring(StartIndex));

                // Save the image to disk
                Guid Id = Guid.NewGuid();
                string Src = "~/Images/" + Id;
                string Filename = HostingEnvironment.MapPath("~/Images/" + GetRequestDomain(httpContext) + "/" + Id + ".png"); // TODO What happens if they upload a .gif or .jpg?
                Directory.CreateDirectory(Path.GetDirectoryName(Filename));
                File.WriteAllBytes(Filename, ImageBytes);

                // Update the html to include a link instead of data: uri
                html = new Regex("src=\"(data:([^\"]+))\"").Replace(html, "src=\"" + VirtualPathUtility.ToAbsolute(Src) + "\" alt=\"\"", 1);
            }

            return html;
        }
    }
}