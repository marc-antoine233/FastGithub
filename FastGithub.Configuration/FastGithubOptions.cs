﻿using System;
using System.Collections.Generic;
using System.Net;

namespace FastGithub.Configuration
{
    /// <summary>
    /// FastGithub的配置
    /// </summary>
    public class FastGithubOptions
    {
        /// <summary>
        /// 监听ip
        /// </summary>
        public IPAddress[] IpAddressList { get; set; } = Array.Empty<IPAddress>();

        /// <summary>
        /// 环回地址
        /// </summary>
        public string RedirectHost { get; set; } = "localhost";

        /// <summary>
        /// http代理端口
        /// </summary>
        public int HttpProxyPort { get; set; } = 38457;

        /// <summary>
        /// 回退的dns
        /// </summary>
        public IPEndPoint[] FallbackDns { get; set; } = Array.Empty<IPEndPoint>();

        /// <summary>
        /// 代理的域名配置
        /// </summary>
        public Dictionary<string, DomainConfig> DomainConfigs { get; set; } = new();
    }
}
