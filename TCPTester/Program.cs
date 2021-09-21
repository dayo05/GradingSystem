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
            SendData(1000, 512*1024, 2, "CS", "using System;\nusing System.Linq;\nvar x = Console.ReadLine().Split().Select(int.Parse).ToList();Console.WriteLine(x[0] + x[1]);");
            SendData(1000, 512*1024, 2, "CS", "using System;\nusing System.Linq;\nvar x = Console.ReadLine().Split().Select(int.Parse).ToList();Console.WriteLine(x[0] - x[1]);");
            SendData(1000, 512*1024, 2, "CPP", "#include <iostream>\nusing namespace std;\nsigned main(){int a, b;cin>>a>>b;cout<<a+b;return 0;}");
            SendData(1000, 512*1024, 2, "Rust", "use std::io;fn main() {let mut s = String::new();io::stdin().read_line(&mut s).expect(\"Failed to read line\");let inputs:Vec<u32> = s.split_whitespace().map(|x| x.parse().expect(\"Input is not integar!\")).collect();println!(\"{}\", inputs[0]+inputs[1]);}");
            SendData(1, 512*1024, 2, "CS", "using System;\nusing System.Linq;\nvar x = Console.ReadLine().Split().Select(int.Parse).ToList();Console.WriteLine(x[0] + x[1]);");
            SendData(1000, 1, 2, "CS", "using System;\nusing System.Linq;\nvar x = Console.ReadLine().Split().Select(int.Parse).ToList();Console.WriteLine(x[0] + x[1]);");
        }
        static void SendData(long timeLimit, long memoryLimit, int testCaseCount, string language, string code)
        {
            using var tcp = new TcpClient("127.0.0.1", 8080);
            using var stream = tcp.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            var data =
                $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><time_limit>{timeLimit}</time_limit><memory_limit>{memoryLimit}</memory_limit><test_case_count>{testCaseCount}</test_case_count><language>{language}</language><code_size>{code.Length}</code_size></root>";
            writer.WriteLine(data.Trim());
            writer.Flush();
            writer.Write(code.Trim());
            writer.Flush();
            while(!reader.EndOfStream)
                Console.WriteLine(reader.ReadLine());
            writer.Close();
            reader.Close();
        }
    }
}