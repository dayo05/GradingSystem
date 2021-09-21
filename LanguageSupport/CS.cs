using System;
using System.IO;
using System.Collections.Generic;

using IroojGradingSystem;

namespace LanguageSupport
{
    public class CS : Base
    {
        public CS(StreamWriter stream, long memoryLimit = 512 * 1024, long timeLimit = 1000, int testCaseCount = 2) : base(stream)
        {
            base.MemoryLimit = memoryLimit;
            TimeLimit = TimeSpan.FromMilliseconds(timeLimit);
            TestCaseCount = testCaseCount;
            CompileString = new List<string>
            {
                "/root/.dotnet/dotnet new console --force -o ./Main",
                "rm Main/Program.cs",
                "cp ./Main.cs ./Main/Program.cs",
                "/root/.dotnet/dotnet publish Main --configuration Release --self-contained true --runtime linux-x64"
            };
            RunScript = "Main/bin/Release/net5.0/linux-x64/Main";
        }

        public void Test()
        {
            File.Copy("Main.cs", "grad/Main.cs");
            Start();
        }
    }
}