using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RalphLaurenPortal.Startup))]
namespace RalphLaurenPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
