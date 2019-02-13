using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LPBugTracker.Startup))]
namespace LPBugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
