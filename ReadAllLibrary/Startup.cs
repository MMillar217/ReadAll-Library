using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ReadAllLibrary.Startup))]
namespace ReadAllLibrary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
