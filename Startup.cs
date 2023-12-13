using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AutoyVaro.Startup))]
namespace AutoyVaro
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
