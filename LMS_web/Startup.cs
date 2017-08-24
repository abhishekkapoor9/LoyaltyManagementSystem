using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LMS_Web.Startup))]
namespace LMS_Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
