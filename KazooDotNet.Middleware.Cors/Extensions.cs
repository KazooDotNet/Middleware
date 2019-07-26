using System;
using Microsoft.AspNetCore.Builder;

namespace KazooDotNet.Middleware.Cors
{
    public static class Extensions
    {
        public static IApplicationBuilder UseKazooCors(this IApplicationBuilder app, Action<CorsOptions> action)
        {
            var opts = new CorsOptions();
            action.Invoke(opts);
            return app.UseMiddleware<CorsMiddleware>(opts);
        }
    }
}