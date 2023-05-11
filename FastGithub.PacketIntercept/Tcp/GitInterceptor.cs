using FastGithub.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using System.Runtime.Versioning;

namespace FastGithub.PacketIntercept.Tcp
{
    /// <summary>
    /// git拦截器
    /// </summary>   
    [SupportedOSPlatform("windows")]
    sealed class GitInterceptor : TcpInterceptor
    {
        /// <summary>
        /// git拦截器
        /// </summary>
        /// <param name="logger"></param>
        public GitInterceptor(ILogger<HttpsInterceptor> logger, IOptions<FastGithubOptions> options)
            : base(9418, getAvailablePort(options.Value), logger)
        {
        }

        private static Dictionary<string, int> getAvailablePort(FastGithubOptions options)
        {
            Dictionary<string, int> values = new Dictionary<string, int>();
            foreach (var ip in options.IpAddressList)
            {
                values.Add(ip.ToString(), GlobalListener.GetGitPort(ip));
            }
            return values;
        }
    }
}
