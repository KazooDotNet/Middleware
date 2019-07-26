using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace KazooDotNet.Middleware.Cors
{
    public class CorsOptions
    {
        public IEnumerable<string> WhitelistRemoteHosts { get; set; }
        public string RemoteHostHeader { get; set; }
        public Action<IHeaderDictionary> OnSuccess { get; set; }
    }
}