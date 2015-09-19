using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XenonCMS.Classes
{
    public class NavMenuItem
    {
        public int Id;
        public List<NavMenuItem> Children;
        public string Text;
        public string Url;

        public NavMenuItem(int id, string text, string url)
        {
            Id = id;
            Text = text;
            Url = url;
        }
    }
}