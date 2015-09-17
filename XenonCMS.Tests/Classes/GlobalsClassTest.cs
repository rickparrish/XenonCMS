using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;

namespace XenonCMS.Tests.Classes
{
    [TestClass]
    public class GlobalsClassTest
    {
        [TestMethod]
        public void TestGetRequestDomain()
        {
            var HCB = new HttpContextWrapper(
                new HttpContext(
                    new HttpRequest(null, "http://www.RickParrish.ca", null),
                    new HttpResponse(null)
                )
            );
            string Domain = XenonCMS.Classes.Globals.GetRequestDomain(HCB);
            Assert.AreEqual("rickparrish.ca", Domain);

            HCB = new HttpContextWrapper(
                new HttpContext(
                    new HttpRequest(null, "http://RickParrish.ca", null),
                    new HttpResponse(null)
                )
            );
            Domain = XenonCMS.Classes.Globals.GetRequestDomain(HCB);
            Assert.AreEqual("rickparrish.ca", Domain);

            HCB = new HttpContextWrapper(
                new HttpContext(
                    new HttpRequest(null, "http://www.a.b.c.d.RickParrish.ca", null),
                    new HttpResponse(null)
                )
            );
            Domain = XenonCMS.Classes.Globals.GetRequestDomain(HCB);
            Assert.AreEqual("a.b.c.d.rickparrish.ca", Domain);

            HCB = new HttpContextWrapper(
                new HttpContext(
                    new HttpRequest(null, "http://a.b.c.d.RickParrish.ca", null),
                    new HttpResponse(null)
                )
            );
            Domain = XenonCMS.Classes.Globals.GetRequestDomain(HCB);
            Assert.AreEqual("a.b.c.d.rickparrish.ca", Domain);
        }
    }
}
