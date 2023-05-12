using FastGithub.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Runtime.Versioning;
using Microsoft.Extensions.Options;

namespace FastGithub.PacketIntercept.Tcp
{
    /// <summary>
    /// https拦截器
    /// </summary>   
    [SupportedOSPlatform("windows")]
    sealed class HttpsInterceptor : TcpInterceptor
    {
        /// <summary>
        /// https拦截器
        /// </summary>
        /// <param name="logger"></param>
        public HttpsInterceptor(ILogger<HttpsInterceptor> logger, IOptions<FastGithubOptions> options)
            : base(443, getAvailablePort(options.Value), logger)
        {
        }

        private static Dictionary<string, int> getAvailablePort(FastGithubOptions options)
        {
            Dictionary<string, int> values = new Dictionary<string, int>();
            foreach (var ip in options.IpAddressList)
            {
                values.Add(ip.ToString(), GlobalListener.GetHttpsPort(ip));
            }
            return values;
        }
    }
}
