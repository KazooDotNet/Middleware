using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KazooDotNet.Middleware.Cors
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        private IEnumerable<string> _whitelistHosts;
        private CorsOptions _opts;
        
        public CorsMiddleware(CorsOptions opts, RequestDelegate next)
        {
            _whitelistHosts = opts.WhitelistRemoteHosts
                .Where( h => !string.IsNullOrWhiteSpace(h))
                .Select(h => h.ToLowerInvariant().Trim());
            _opts = opts;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            var rHeaders = context.Request.Headers;
            if (!rHeaders.ContainsKey("Origin"))
                return;
            var headers = context.Response.Headers;
            var origin = rHeaders["Origin"].First().ToLowerInvariant();
            var host = (_opts.RemoteHostHeader != null && rHeaders.ContainsKey(_opts.RemoteHostHeader) ? 
                rHeaders[_opts.RemoteHostHeader].First() : 
                context.Request.Host.Host).ToLowerInvariant();
            var baseHost = BaseHost(host.Split(':').First());
            var baseOrigin = BaseHost(new Uri(origin).Host.Split(':').First());
            var found = _whitelistHosts.Any(wHost =>
            {
                var bw = BaseHost(wHost); 
                return  bw == baseOrigin;
            });
            if (!found) found = baseOrigin == baseHost;
            if (found)
            {
                headers["Access-Control-Allow-Origin"] = rHeaders["Origin"].First();
                headers["Access-Control-Allow-Methods"] = "GET,HEAD,POST,PUT,PATCH,DELETE";
                headers["Access-Control-Allow-Headers"] = "Content-Type, *";
                headers["Access-Control-Expose-Headers"] = "*";
                _opts.OnSuccess?.Invoke(headers);
            }
            headers["Vary"] = "Origin";
        }

        private static string BaseHost(string host)
        {
            var parts = host.Split('.');
            var l = parts.Length;
            return l == 1 ? parts[0] : $"{parts[l - 2]}.{parts[l - 1]}";
        }    
            
    }
}