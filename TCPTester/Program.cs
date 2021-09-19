using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPTester
{
    class Program
    {
        static void Main(string[] args)
        {
            SendData("<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><question_number>123</question_number><language>CS</language><code>using System;\nusing System.Linq;\nvar x = Console.ReadLine().Split().Select(int.Parse).ToList();Console.WriteLine(x[0] + x[1]);</code></root>");
        }

        static void SendData(string data)
        {
            var tcp = new TcpClient("127.0.0.1", 8080);
            var stream = tcp.GetStream();
            var buffer = Encoding.UTF8.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            
            new Thread(() =>
            {
                var readTcp = new TcpListener(IPAddress.Loopback, 8081);
                readTcp.Start();
                var client = readTcp.AcceptTcpClient();
                var readStream = client.GetStream();
                while (client.Connected)
                {
                    if (!readStream.CanRead) continue;
                    var byteRead = 0;
                    var recvData = new StringBuilder();
                    while ((byteRead = readStream.Read(buffer, 0, buffer.Length)) > 0)
                        recvData.Append(Encoding.UTF8.GetString(buffer, 0, byteRead));
                    Console.WriteLine(recvData);
                }
            }).Start();
            Console.ReadKey();
        }
    }
}