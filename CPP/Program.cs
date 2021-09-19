using System;
using IroojGradingSystem;

namespace CPP
{
    class Program
    {
        static void Main(string[] args)
        {
            //Process.Start("/bin/bash");
            new GradCPP().Test();
        }
    }

    public class GradCPP : Base
    {
        public GradCPP()
        {
            MemoryLimit = 512 * 1024;
            TimeLimit = TimeSpan.FromMilliseconds(1000);
            TestCaseCount = 1;
            CompileString = new() {"/bin/g++ -o main main.cpp"};
            RunScript = "./main";
        }

        protected void SendResult(Result result, string dat = "")
        {
            Console.WriteLine(result);
        }

        public void Test()
        {
            Start();
        }
    }
}