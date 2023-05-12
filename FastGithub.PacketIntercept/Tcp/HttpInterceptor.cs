using FastGithub.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Versioning;

namespace FastGithub.PacketIntercept.Tcp
{
    /// <summary>
    /// http拦截器
    /// </summary>   
    [SupportedOSPlatform("windows")]
    sealed class HttpInterceptor : TcpInterceptor
    {
        private int newServerPort2;

        /// <summary>
        /// http拦截器
        /// </summary>
        /// <param name="logger"></param>
        public HttpInterceptor(ILogger<HttpInterceptor> logger, IOptions<FastGithubOptions> options)
            : base(80, getAvailablePort(options.Value), logger)
        {
        }

        private static Dictionary<string, int> getAvailablePort(FastGithubOptions options)
        {
            Dictionary<string, int> values = new Dictionary<string, int>();
            foreach (var ip in options.IpAddressList)
            {
                values.Add(ip.ToString(), GlobalListener.GetHttpPort(ip));
            }
            return values;
        }
    }
}
