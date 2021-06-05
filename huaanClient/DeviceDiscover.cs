using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace huaanClient
{
    public static class DeviceDiscover
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static List<(string mac, string ip, string mask, string platform, string system)> Search(int timeoutSec)
        {
            List<(string mac, string ip, string mask, string platform, string system)> ret = new List<(string mac, string ip, string mask, string platform, string system)>();

            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name).Where(ipa => ipa.AddressFamily == AddressFamily.InterNetwork).ToArray();
            Task[] tasks = new Task[ipadrlist.Length];
            for (int i = 0; i < ipadrlist.Length; ++i)
            {
                IPAddress ipa = ipadrlist[i];
                UdpClient client = new UdpClient();
                client.JoinMulticastGroup(IPAddress.Parse("224.0.1.1"));

                IPEndPoint multicast = new IPEndPoint(IPAddress.Parse("224.0.1.1"), 6100);
                client.Send(new byte[] { 0xbb, 0x0b, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0xbb, 0x0b, 0x00, 0x00 }, 12, multicast);

                tasks[i] = Task.Factory.StartNew(() =>
                {
                    //Configuration
                    var interfaceIp = ipa;
                    var interfaceEndPoint = new IPEndPoint(interfaceIp, 6100);
                    var multicastIp = IPAddress.Parse("224.0.1.1");
                    var multicastEndPoint = new IPEndPoint(multicastIp, 6100);

                    //initialize the socket
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.ExclusiveAddressUse = false;
                    socket.MulticastLoopback = false;
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    MulticastOption option = new MulticastOption(multicastEndPoint.Address, interfaceIp);
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, option);

                    //bind on a network interface
                    socket.Bind(interfaceEndPoint);

                    socket.ReceiveTimeout = timeoutSec * 1000;
                    byte[] buffer = new byte[1500];

                    DateTime startTime = DateTime.Now;
                    int len = 0;
                    try
                    {
                        while ((DateTime.Now - startTime).TotalSeconds <= timeoutSec && (len = socket.Receive(buffer)) > 0)
                        {
                            int type = BitConverter.ToInt32(buffer, 0);
                            if (type == 3004)
                            {
                                if (len > 120)
                                {
                                    lock (ret)
                                    {
                                        if (ret.Count(vt => vt.mac == Encoding.UTF8.GetString(buffer, 8, 20).TrimEnd('\0')) != 0) continue;
                                        ret.Add((Encoding.UTF8.GetString(buffer, 8, 20).TrimEnd('\0')
                                            , Encoding.UTF8.GetString(buffer, 28, 20).TrimEnd('\0')
                                            , Encoding.UTF8.GetString(buffer, 48, 20).TrimEnd('\0')
                                            //, Encoding.UTF8.GetString(buffer, 68, 16).TrimEnd('\0')
                                            , Encoding.UTF8.GetString(buffer, 84, 32).TrimEnd('\0')
                                            , Encoding.UTF8.GetString(buffer, 116, 32).TrimEnd('\0')));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                });
            }
            foreach (Task task in tasks)
                try
                {
                    task.Wait();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            return ret;
        }
    
        public static void SetIpByMac(string mac, string ip, string mask, string gateway)
        {
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name).Where(ipa => ipa.AddressFamily == AddressFamily.InterNetwork).ToArray();
            Task[] tasks = new Task[ipadrlist.Length];
            byte[] data = new byte[88];
            data[0] = 0xbd;
            data[1] = 0x0b;
            data[4] = 0x50;
            byte[] macBytes = Encoding.UTF8.GetBytes(mac);
            Array.Copy(macBytes, 0, data, 8, macBytes.Length);
            byte[] ipBytes = Encoding.UTF8.GetBytes(ip);
            Array.Copy(ipBytes, 0, data, 28, ipBytes.Length);
            byte[] maskBytes = Encoding.UTF8.GetBytes(mask);
            Array.Copy(maskBytes, 0, data, 48, maskBytes.Length);
            byte[] gatewayBytes = Encoding.UTF8.GetBytes(gateway);
            Array.Copy(gatewayBytes, 0, data, 68, gatewayBytes.Length);
            for (int i = 0; i < ipadrlist.Length; ++i)
            {
                IPAddress ipa = ipadrlist[i];
                UdpClient client = new UdpClient(new IPEndPoint(ipa, 0));
                client.JoinMulticastGroup(IPAddress.Parse("224.0.1.1"));

                IPEndPoint multicast = new IPEndPoint(IPAddress.Parse("224.0.1.1"), 6100);

                client.Send(data, 88, multicast);
            }
        }
    }
}
