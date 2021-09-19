using System;
using System.Collections.Generic;

using IroojGradingSystem;


namespace CS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new GradCS().Test();
        }
    }

    class GradCS : Base
    {
        public GradCS()
        {
            MemoryLimit = 512 * 1024;
            TimeLimit = TimeSpan.FromMilliseconds(1000);
            TestCaseCount = 2;
            CompileString = new List<string>
            {
                "dotnet new console --force -o ./Main",
                "rm Main/Program.cs",
                "cp ./Main.cs ./Main/Program.cs",
                "dotnet publish Main --configuration Release --self-contained true --runtime linux-x64"
            };
            RunScript = "Main/bin/Release/net5.0/linux-x64/Main";
        }

        public void Test()
            => Start();
    }
}