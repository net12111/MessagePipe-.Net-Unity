﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePipe.Interprocess.Workers
{
    internal sealed class SocketUdpServer : IDisposable
    {
        const int MinBuffer = 4096;

        readonly Socket socket;
        readonly byte[] buffer;

        // SocketUdpServer(int bufferSize)
        // {
        //     socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //     socket.ReceiveBufferSize = bufferSize;
        //     buffer = new byte[Math.Max(bufferSize, MinBuffer)];
        // }

        SocketUdpServer(int bufferSize, AddressFamily addressFamily, ProtocolType protocolType)
        {
            socket = new Socket(addressFamily, SocketType.Dgram, protocolType);
            socket.ReceiveBufferSize = bufferSize;
            buffer = new byte[Math.Max(bufferSize, MinBuffer)];
        }

        public static SocketUdpServer Bind(int port, int bufferSize)
        {
            var server = new SocketUdpServer(bufferSize, AddressFamily.InterNetwork, ProtocolType.Udp);
            server.socket.Bind(new IPEndPoint(IPAddress.Any, port));
            return server;
        }
#if NET5_0_OR_GREATER
        public static SocketUdpServer BindUnixDomainSocket(string domainSocketPath, int bufferSize)
        {
            var server = new SocketUdpServer(bufferSize, AddressFamily.Unix, ProtocolType.IP);
            server.socket.Bind(new UnixDomainSocketEndPoint(domainSocketPath));
            return server;
        }
#endif

        public async ValueTask<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken cancellationToken)
        {
#if NET5_0_OR_GREATER
            var i = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken).ConfigureAwait(false);
            return buffer.AsMemory(0, i);
#else
            var tcs = new TaskCompletionSource<ReadOnlyMemory<byte>>();

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, x =>
            {
                int i;
                try
                {
                    i = socket.EndReceive(x);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                    return;
                }
                var r = buffer.AsMemory(0, i);
                tcs.TrySetResult(r);
            }, null);

            return await tcs.Task;
#endif
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }

    internal sealed class SocketUdpClient : IDisposable
    {
        const int MinBuffer = 4096;

        readonly Socket socket;
        readonly byte[] buffer;

        SocketUdpClient(int bufferSize, ProtocolType protocolType)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, protocolType);
            socket.SendBufferSize = bufferSize;
            buffer = new byte[Math.Max(bufferSize, MinBuffer)];
        }

        public static SocketUdpClient Connect(string host, int port, int bufferSize)
        {
            var client = new SocketUdpClient(bufferSize, ProtocolType.Udp);
            client.socket.Connect(new IPEndPoint(IPAddress.Parse(host), port));
            return client;
        }
#if NET5_0_OR_GREATER
        public static SocketUdpClient ConnectUnixDomainSocket(string domainSocketPath, int bufferSize)
        {
            var client = new SocketUdpClient(bufferSize, ProtocolType.IP);
            client.socket.Connect(new UnixDomainSocketEndPoint(domainSocketPath));
            return client;
        }
#endif

        public ValueTask<int> SendAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
#if NET5_0_OR_GREATER
            return socket.SendAsync(buffer, SocketFlags.None, cancellationToken);
#else
            var tcs = new TaskCompletionSource<int>();
            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, x =>
             {
                 int i;
                 try
                 {
                     i = socket.EndSend(x);
                 }
                 catch (Exception ex)
                 {
                     tcs.TrySetException(ex);
                     return;
                 }
                 tcs.TrySetResult(i);
             }, null);
#if !UNITY_2018_3_OR_NEWER
            return new ValueTask<int>(tcs.Task);
#else
            return tcs.Task;
#endif
#endif
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}