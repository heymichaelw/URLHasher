using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(URLHasher.Startup))]
namespace URLHasher
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
