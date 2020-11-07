using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PostUploader.Startup))]
namespace PostUploader
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
