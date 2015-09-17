using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XenonCMS.Classes
{
    public class NavMenuItem
    {
        public List<NavMenuItem> Children;
        public string Text;
        public string Url;

        public NavMenuItem(string text, string url)
        {
            Text = text;
            Url = url;
        }
    }
}