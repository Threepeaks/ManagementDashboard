using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ManagementDashboard.Startup))]
namespace ManagementDashboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
