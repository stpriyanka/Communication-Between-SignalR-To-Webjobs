using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebJobAsSingalRClient.Startup))]
namespace WebJobAsSingalRClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
			app.MapSignalR();
        }
    }
}
