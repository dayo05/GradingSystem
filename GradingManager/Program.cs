using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace GradingManager
{
    class Program
    {
        static string GetSourceCodeByLanguage(string language)
        {
            switch (language)
            {
                case "CPP":
                    return "main.cpp";
                case "CS":
                    return "Main.cs";
                case "Rust":
                    return "main.rs";
                default:
                    return "Error.log";
            }
        }
        static void Main(string[] args)
        {
            var listener = new TcpListener(IPAddress.Loopback, 8080);
            listener.Start();
            while (true)
            {
                using var client = listener.AcceptTcpClient();
                using var stream = client.GetStream();
                var buffer = new byte[1024];
                int byteRead;
                var recvData = new StringBuilder();
                while ((byteRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    recvData.Append(Encoding.UTF8.GetString(buffer, 0, byteRead));
                Console.WriteLine("Received");

                new Thread(() =>
                {
                    var tcp = new TcpClient("127.0.0.1", 8081);
                    var writeStream = tcp.GetStream();
                    writeStream.Write(Encoding.UTF8.GetBytes("Received"));
                    writeStream.Flush();
                }).Start();
                
                var data = recvData.ToString();
                var xmldoc = new XmlDocument();
                xmldoc.LoadXml(data);

                var xml = xmldoc.GetElementsByTagName("root");
                
                var questionNumber = int.Parse(xml[0]["question_number"].InnerText);
                var language = xml[0]["language"].InnerText;
                var code = xml[0]["code"].InnerText;
                using var sourcecode = new StreamWriter(GetSourceCodeByLanguage(language));
                sourcecode.Write(code);
                
                Console.WriteLine(questionNumber);
                Console.WriteLine(language);
                Console.WriteLine(code);
                sourcecode.Flush();
                switch (language)
                {
                    case "CPP":
                        new CPP.GradCPP().Test();
                        break;
                    case "CS":
                        new CS.GradCS().Test();
                        break;
                    case "Rust":
                        break;
                    default:
                        break;
                }
                /*
                new Thread(() =>
                {
                    var buf = Encoding.UTF8.GetBytes("Hello, world!");
                    stream.Write(buf, 0, buf.Length);
                    stream.Flush();
                }).Start();
                */
            }
        }
    }
}