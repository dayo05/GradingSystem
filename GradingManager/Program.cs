using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Net.Sockets;

using LanguageSupport;

namespace GradingManager
{
    class Program
    {
        static string GetSourceCodeByLanguage(string language)
        {
            return language switch
            {
                "CPP" => "main.cpp",
                "CS" => "Main.cs",
                "Rust" => "main.rs",
                "Python3" => "main.py",
                "Aheui" => "main.aheui",
                "Brainfuck" => "main.bf",
                "Pypy3" => "main.py",
                _ => "Error.log"
            };
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
                xmldoc.LoadXml(data ?? string.Empty);

                var xml = xmldoc.GetElementsByTagName("root");
                
                var timeLimit = long.Parse(xml[0]?["time_limit"]?.InnerText);
                var memoryLimit = long.Parse(xml[0]?["memory_limit"]?.InnerText);
                var testCaseCount = int.Parse(xml[0]?["test_case_count"]?.InnerText);
                var language = xml[0]["language"]?.InnerText;
                var codesize = int.Parse(xml[0]["code_size"]?.InnerText);
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
                Console.WriteLine(language);
                switch (language)
                {
                    case "CPP":
                        new CPP(writer, memoryLimit, timeLimit, testCaseCount).Test();
                        break;
                    case "CS":
                        new CS(writer, memoryLimit, timeLimit, testCaseCount).Test();
                        break;
                    case "Rust":
                        new Rust(writer, memoryLimit, timeLimit, testCaseCount).Test();
                        break;
                    case "Python3":
                        new Python3(writer, memoryLimit, timeLimit, testCaseCount).Test();
                        break;
                    case "Aheui":
                        new Aheui(writer, memoryLimit, timeLimit, testCaseCount).Test();
                        break;
                    case "Brainfuck":
                        new BrainFuck(writer, memoryLimit, timeLimit, testCaseCount).Test();
                        break;
                    case "Pypy3":
                        new Pypy3(writer, memoryLimit, timeLimit, testCaseCount).Test();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}