using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using WindivertDotnet;

namespace FastGithub.PacketIntercept.Tcp
{
    /// <summary>
    /// tcp拦截器
    /// </summary>   
    [SupportedOSPlatform("windows")]
    abstract class TcpInterceptor : ITcpInterceptor
    {
        private readonly string filter_string;
        private readonly ushort oldServerPort;
        private readonly Dictionary<string, int> newServer;
        private readonly ILogger logger;

        /// <summary>
        /// tcp拦截器
        /// </summary>
        /// <param name="oldServerPort">修改前的服务器端口</param>
        /// <param name="newServer">修改后的服务器ip与端口</param>
        /// <param name="logger"></param>
        public TcpInterceptor(int oldServerPort, Dictionary<string, int> newServer, ILogger logger)
        {
            this.filter_string=$"(loopback and (tcp.DstPort == {(ushort)oldServerPort}) or (false ";
            foreach (var kvp in newServer)
            {
                switch (IPAddress.Parse(kvp.Key).AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        this.filter_string+=$"or (ip.SrcAddr == {kvp.Key} and tcp.SrcPort == {kvp.Value})";
                        break;
                    case AddressFamily.InterNetworkV6:
                        this.filter_string+=$"or (ipv6.SrcAddr == {kvp.Key} and tcp.SrcPort == {kvp.Value})";
                        break;
                }
            }
            this.filter_string+="))";

            this.oldServerPort = (ushort)oldServerPort;
            this.newServer = newServer;
            this.logger = logger;
        }

        /// <summary>
        /// 拦截指定端口的数据包
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="Win32Exception"></exception>
        public async Task InterceptAsync(CancellationToken cancellationToken)
        {
            if (this.newServer.Count == 0)
            {
                return;
            }

            using var divert = new WinDivert(this.filter_string, WinDivertLayer.Network);
            using var packet = new WinDivertPacket();
            using var addr = new WinDivertAddress();

            foreach (var kvp in newServer)
            {
                this.logger.LogInformation($"{kvp.Key}:{this.oldServerPort} <=> {kvp.Key}:{kvp.Value}");
            }

            while (cancellationToken.IsCancellationRequested == false)
            {
                await divert.RecvAsync(packet, addr, cancellationToken);
                try
                {
                    this.ModifyTcpPacket(packet, addr);
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex.Message);
                }
                finally
                {
                    await divert.SendAsync(packet, addr, cancellationToken);
                }
            }
        }

        /// <summary>
        /// 修改tcp数据端口的端口
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="addr"></param>
        unsafe private void ModifyTcpPacket(WinDivertPacket packet, WinDivertAddress addr)
        {
            var result = packet.GetParseResult();
            if (result.TcpHeader->DstPort == oldServerPort)
            {
                if (result.IPV4Header!=null)
                {
                    int DstPortNew;
                    this.newServer.TryGetValue(result.IPV4Header->DstAddr.ToString(), out DstPortNew);
                    if (DstPortNew>0)
                    {
                        result.TcpHeader->DstPort = (ushort)DstPortNew;
                    }
                }
                if (result.IPV6Header!=null)
                {
                    int DstPortNew;
                    this.newServer.TryGetValue(result.IPV6Header->DstAddr.ToString(), out DstPortNew);
                    if (DstPortNew>0)
                    {
                        result.TcpHeader->DstPort = (ushort)DstPortNew;
                    }
                }
            }
            else
            {
                result.TcpHeader->SrcPort = oldServerPort;
            }
            addr.Flags |= WinDivertAddressFlag.Impostor;
            packet.CalcChecksums(addr);
        }
    }
}
