using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Net.Sockets;
using IroojGradingSystem;
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
                while (true)
                {
                    var dataSize = int.Parse(reader.ReadLine());
                    var buffer = new char[dataSize + 10];
                    reader.Read(buffer, 0, dataSize);
                    var data = new string(buffer);
                    var xmldoc = new XmlDocument();
                    xmldoc.LoadXml(data ?? string.Empty);

                    var xml = xmldoc.GetElementsByTagName("root");

                    var timeLimit = long.Parse(xml[0]?["time_limit"]?.InnerText);
                    var memoryLimit = long.Parse(xml[0]?["memory_limit"]?.InnerText);
                    var testCaseCount = int.Parse(xml[0]?["test_case_count"]?.InnerText);
                    var judgeNumber = long.Parse(xml[0]?["judge_number"]?.InnerText);
                    var language = xml[0]["language"]?.InnerText;
                    var code = xml[0]["code"]?.InnerText;

                    new Grad(writer, language, code)
                    {
                        MemoryLimit = memoryLimit,
                        TimeLimit = TimeSpan.FromMilliseconds(timeLimit),
                        TestCaseCount = testCaseCount,
                        JudgeNumber = judgeNumber
                    }.Start();
                }
            }
        }
    }
}