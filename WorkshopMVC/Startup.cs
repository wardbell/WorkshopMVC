using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorkshopMVC.Startup))]
namespace WorkshopMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
