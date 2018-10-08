using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CEO_Dashboard.Startup))]
namespace CEO_Dashboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
