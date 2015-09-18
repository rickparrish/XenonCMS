using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Collections.Generic;
using System.Web.Hosting;

namespace XenonCMS.Tests.Classes
{
    [TestClass]
    public class GlobalsClassTest
    {
        [TestMethod]
        public void TestGetRequestDomain()
        {
            var Tests = new Dictionary<string, string>() {
                { "RandM.ca", "randm.ca" },
                { "Www.RandM.ca", "randm.ca" },
                { "Sub.RandM.ca", "sub.randm.ca" },
                { "Www.Sub.RandM.ca", "sub.randm.ca" },
                { "Sub.Www.RandM.ca", "sub.www.randm.ca" }
            };
            foreach (var Test in Tests)
            {
                HttpContext.Current = new HttpContext(
                    new HttpRequest(null, $"http://{Test.Key}", null),
                    new HttpResponse(null)
                );
                string Domain = XenonCMS.Classes.Globals.GetRequestDomain();
                Assert.AreEqual(Test.Value, Domain);
            }
        }
    }
}
