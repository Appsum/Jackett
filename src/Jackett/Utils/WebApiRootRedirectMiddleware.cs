﻿using Microsoft.Owin;
using System.Threading.Tasks;

namespace Jackett.Utils
{
    public class WebApiRootRedirectMiddleware : OwinMiddleware
    {
        public WebApiRootRedirectMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            var url = context.Request.Uri;
            if(context.Request.Path != null && context.Request.Path.HasValue && context.Request.Path.Value.StartsWith(Startup.BasePath))
            {
                context.Request.Path = new PathString(context.Request.Path.Value.Substring(Startup.BasePath.Length-1));
            }

            if (string.IsNullOrWhiteSpace(url.AbsolutePath) || url.AbsolutePath == "/")
            {
                // 301 is the status code of permanent redirect
                context.Response.StatusCode = 302;
                var redir = Startup.BasePath + "Admin/Dashboard";
                Engine.Logger.Info("redirecting to " + redir);
                context.Response.Headers.Set("Location", redir);
            }
            else
            {


                await Next.Invoke(context);
            }
        }
    }
}