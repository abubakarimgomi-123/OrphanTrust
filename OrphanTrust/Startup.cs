using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OrphanTrust.Startup))]

namespace OrphanTrust
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}