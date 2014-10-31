using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PEETS.Startup))]
namespace PEETS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
