
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(YourNamespace.Startup))]

namespace YourNamespace
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure authentication using cookies
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Home/Login")
            });
        }
    }
}