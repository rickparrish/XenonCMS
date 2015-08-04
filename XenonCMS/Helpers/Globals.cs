using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using XenonCMS.Models;

namespace XenonCMS.Helpers
{
    static public class Globals
    {
        static private List<string> GetAdminIPs(HttpContextBase httpContext)
        {
            List<string> Result = DatabaseCache.GetAdminIPs(httpContext);
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

                        // Site admin ips
                        foreach (var Row in DB.SiteAdminIPs.Where(x => x.Site.Domain == RequestDomain))
                        {
                            Result.Add(Row.Address);
                        }
                    }

                    // Cache what we have so far, so if the dns lookup is necessary, and takes awhile, we won't have multiple requests doing it
                    DatabaseCache.AddAdminIPs(httpContext, Result);

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
                    DatabaseCache.AddAdminIPs(httpContext, Result);
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
            List<string> AdminIPs = GetAdminIPs(httpContext);
            return AdminIPs.Contains(httpContext.Request.UserHostAddress);
        }
    }
}