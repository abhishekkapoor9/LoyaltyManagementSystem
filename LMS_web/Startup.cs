using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LMS_web.Startup))]
namespace LMS_web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
