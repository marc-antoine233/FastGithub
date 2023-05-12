using FastGithub.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Runtime.Versioning;
using Microsoft.Extensions.Options;

namespace FastGithub.PacketIntercept.Tcp
{
    /// <summary>
    /// ssh拦截器
    /// </summary>   
    [SupportedOSPlatform("windows")]
    sealed class SshInterceptor : TcpInterceptor
    {
        /// <summary>
        /// ssh拦截器
        /// </summary>
        /// <param name="logger"></param>
        public SshInterceptor(ILogger<HttpInterceptor> logger, IOptions<FastGithubOptions> options)
            : base(22, getAvailablePort(options.Value), logger)
        {
        }

        private static Dictionary<string, int> getAvailablePort(FastGithubOptions options)
        {
            Dictionary<string, int> values = new Dictionary<string, int>();
            foreach (var ip in options.IpAddressList)
            {
                values.Add(ip.ToString(), GlobalListener.GetSshPort(ip));
            }
            return values;
        }
    }
}
