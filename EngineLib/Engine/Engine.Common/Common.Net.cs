using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Engine.Common
{
    /// <summary>
    /// 网络操作类
    /// </summary>
    public static partial class sCommon
    {
        /// <summary>
        /// 获取计算机IP地址列表
        /// </summary>
        /// <param name="ipType"></param>
        /// <returns></returns>
        public static List<string> GetHostAddress(IP_TYPE ipType)
        {
            IPAddress[] ipAddr = Dns.GetHostAddresses(Dns.GetHostName());

            if (ipAddr == null)
                return null;

            List<string> ipList = new List<string>();
            foreach (IPAddress ip in ipAddr)
            {
                switch (ipType)
                {
                    case IP_TYPE.IPV4:
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            ipList.Add(ip.ToString());
                        break;

                    case IP_TYPE.IPV6:
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                            ipList.Add(ip.ToString());
                        break;

                    default:
                        ipList.Add(ip.ToString());
                        break;
                }
            }
            return ipList;
        }

        /// <summary>
        /// 获取当前主机名称
        /// </summary>
        public static string HostName
        {
            get => Dns.GetHostName();
        }

        /// <summary>
        /// 侦测目标服务端口
        /// </summary>
        /// <param name="host">主机名称 or 主机IP </param>
        /// <param name="port"></param>
        /// <param name="millisecondsTimeout">超时时间 单位：ms 默认：1000ms</param>
        /// <returns></returns>
        public static string GetServerPortStatus(string host, int port, int millisecondsTimeout = 1000)
        {
            TcpClient client = new TcpClient();
            try
            {
                var ar = client.BeginConnect(host, port, null, null);
                ar.AsyncWaitHandle.WaitOne(millisecondsTimeout);
            }
            catch(SocketException ex)
            {
                client.Close();
                client.Dispose();
                if (ex.ErrorCode == 10061)
                {
                    return "端口占用";
                }
                else
                {
                    return "端口服务未开启";
                }
            }
            finally
            {
                client.Close();
                client.Dispose();
            }
            return client.Connected ? "端口侦听中..." : "";
        }
    }
}
