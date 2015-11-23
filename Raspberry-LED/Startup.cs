using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Raspberry_LED.Startup))]
namespace Raspberry_LED
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
