using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XenonCMS.Startup))]
namespace XenonCMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
