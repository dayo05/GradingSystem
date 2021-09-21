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
            Console.WriteLine("Running");
            var listener = new TcpListener(IPAddress.Loopback, 8080);
            listener.Start();
            while (true)
            {
                using var client = listener.AcceptTcpClient();
                using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                using var writer = new StreamWriter(stream);

                Console.WriteLine("Connected");
                var data = reader.ReadLine();
                var xmldoc = new XmlDocument();
                xmldoc.LoadXml(data);

                var xml = xmldoc.GetElementsByTagName("root");
                
                var questionNumber = int.Parse(xml[0]["question_number"].InnerText);
                var language = xml[0]["language"].InnerText;
                var codesize = int.Parse(xml[0]["code_size"].InnerText);
                var code = "";
                var buffer = new char[1000];
                int r;
                while ((r = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
                    code += new string(buffer)[..r];
                    codesize -= r;
                    if (codesize == 0) break;
                }

                using var sourcecode = new StreamWriter(GetSourceCodeByLanguage(language));
                sourcecode.Write(code);
                
                sourcecode.Flush();
                switch (language)
                {
                    case "CPP":
                        new CPP.GradCPP(writer).Test();
                        break;
                    case "CS":
                        new CS.GradCS(writer).Test();
                        break;
                    case "Rust":
                        new Rust.GradRust(writer).Test();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}