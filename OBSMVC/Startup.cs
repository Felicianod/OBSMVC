using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OBSMVC.Startup))]
namespace OBSMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
