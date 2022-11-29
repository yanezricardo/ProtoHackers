using System.Net;
using System.Net.Sockets;

namespace SmokeTest
{
    internal class TcpEchoServer
    {
        private readonly TcpListener _tcpListener;
        private bool _running;

        public TcpEchoServer(string ipAddress, int port)
        {
            var localAddress = IPAddress.Parse(ipAddress);
            _tcpListener = new TcpListener(localAddress, port);
        }

        public async Task Start()
        {
            try
            {
                _tcpListener.Start();
                _running = true;
                Console.WriteLine($"Echo Server On.");

                while (_running)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    ThreadPool.QueueUserWorkItem(OnClientConnected, tcpClient);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
                throw;
            }
        }

        public Task Stop()
        {
            _tcpListener.Stop();
            _running = false;

            return Task.CompletedTask;
        }

        private static async void OnClientConnected(object? obj)
        {
            using var client = obj as TcpClient;
            if (client is null)
            {
                return;
            }

            Console.WriteLine("Connected!");

            Byte[] buffer = new Byte[1024];
            NetworkStream stream = client!.GetStream();
            while (await stream.ReadAsync(buffer) != 0)
            {
                var data = System.Text.Encoding.ASCII.GetString(buffer);
                Console.WriteLine("Received: {0}", data);

                await stream.WriteAsync(buffer);
                Console.WriteLine("Sent: {0}", data);
            }
        }
    }
}
