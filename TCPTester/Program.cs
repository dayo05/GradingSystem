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
            //SendData("<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><question_number>123</question_number><language>CS</language></root>", "using System;\nusing System.Linq;\nvar x = Console.ReadLine().Split().Select(int.Parse).ToList();Console.WriteLine(x[0] + x[1]);");
            //SendData(123, "CS", "using System;\nusing System.Linq;\nvar x = Console.ReadLine().Split().Select(int.Parse).ToList();Console.WriteLine(x[0] + x[1]);");
            //SendData(123, "CS", "using System;\nusing System.Linq;\nvar x = Console.ReadLine().Split().Select(int.Parse).ToList();Console.WriteLine(x[0] - x[1]);");
            //SendData(12, "CPP", "#include <iostream>\nusing namespace std;\nsigned main(){int a, b;cin>>a>>b;cout<<a+b;return 0;}");
            SendData(1212, "Rust", "use std::io;fn main() {let mut s = String::new();io::stdin().read_line(&mut s).expect(\"Failed to read line\");let inputs:Vec<u32> = s.split_whitespace().map(|x| x.parse().expect(\"Input is not integar!\")).collect();println!(\"{}\", inputs[0]+inputs[1]);}");
        }
        static void SendData(int questionNumber, string language, string code)
        {
            using var tcp = new TcpClient("127.0.0.1", 8080);
            using var stream = tcp.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);
            var data =
                $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><question_number>{questionNumber}</question_number><language>{language}</language><code_size>{code.Length}</code_size></root>";
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