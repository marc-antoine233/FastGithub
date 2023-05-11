using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace FastGithub.Configuration
{
    /// <summary>
    /// 监听器
    /// </summary>
    public static class GlobalListener
    {
        private static readonly IPGlobalProperties global = IPGlobalProperties.GetIPGlobalProperties();
        private static readonly HashSet<string> tcpListenPorts = GetListenPorts(global.GetActiveTcpListeners);
        private static readonly HashSet<string> udpListenPorts = GetListenPorts(global.GetActiveUdpListeners);

        /// <summary>
        /// ssh端口
        /// </summary>
        public static int GetSshPort(IPAddress ip) => GetAvailableTcpPort(ip, 22);

        /// <summary>
        /// git端口
        /// </summary>
        public static int GetGitPort(IPAddress ip) => GetAvailableTcpPort(ip, 9418);

        /// <summary>
        /// http端口
        /// </summary>
        public static int GetHttpPort(IPAddress ip) => OperatingSystem.IsWindows() ? GetAvailableTcpPort(ip, 80) : GetAvailableTcpPort(ip, 3880);

        /// <summary>
        /// https端口
        /// </summary>
        public static int GetHttpsPort(IPAddress ip) => OperatingSystem.IsWindows() ? GetAvailableTcpPort(ip, 443) : GetAvailableTcpPort(ip, 38443);

        /// <summary>
        /// 获取已监听的端口
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private static HashSet<string> GetListenPorts(Func<IPEndPoint[]> func)
        {
            var hashSet = new HashSet<string>();
            try
            {
                foreach (var endpoint in func())
                {
                    hashSet.Add($"{endpoint.Address}:{endpoint.Port}");
                }
            }
            catch (Exception)
            {
            }
            return hashSet;
        }

        /// <summary>
        /// 是可以监听TCP
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CanListenTcp(IPAddress ip, int port)
        {
            return tcpListenPorts.Contains($"{ip}:{port}") == false;
        }

        /// <summary>
        /// 是可以监听UDP
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CanListenUdp(IPAddress ip, int port)
        {
            return udpListenPorts.Contains($"{ip}:{port}") == false;
        }

        /// <summary>
        /// 是可以监听TCP和Udp
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CanListen(IPAddress ip, int port)
        {
            return CanListenTcp(ip, port) && CanListenUdp(ip, port);
        }

        /// <summary>
        /// 获取可用的随机Tcp端口
        /// </summary>
        /// <param name="minPort"></param> 
        /// <returns></returns>
        public static int GetAvailableTcpPort(IPAddress ip, int minPort)
        {
            return GetAvailablePort(CanListenTcp, ip, minPort);
        }

        /// <summary>
        /// 获取可用的随机Udp端口
        /// </summary>
        /// <param name="minPort"></param> 
        /// <returns></returns>
        public static int GetAvailableUdpPort(IPAddress ip, int minPort)
        {
            return GetAvailablePort(CanListenUdp, ip, minPort);
        }

        /// <summary>
        /// 获取可用的随机端口
        /// </summary>
        /// <param name="minPort"></param> 
        /// <returns></returns>
        public static int GetAvailablePort(IPAddress ip, int minPort)
        {
            return GetAvailablePort(CanListen, ip, minPort);
        }

        /// <summary>
        /// 获取可用端口
        /// </summary>
        /// <param name="canFunc"></param>
        /// <param name="minPort"></param>
        /// <returns></returns>
        /// <exception cref="FastGithubException"></exception>
        private static int GetAvailablePort(Func<IPAddress, int, bool> canFunc, IPAddress ip, int minPort)
        {
            for (var port = minPort; port < IPEndPoint.MaxPort; port++)
            {
                if (canFunc(ip, port) == true)
                {
                    return port;
                }
            }
            throw new FastGithubException("当前无可用的端口");
        }
    }
}
