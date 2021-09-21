using System;
using System.IO;
using System.Collections.Generic;

using IroojGradingSystem;

namespace LanguageSupport
{
    public class Aheui : Base
    {
        public Aheui(StreamWriter stream, long memoryLimit = 512 * 1024, long timeLimit = 1000, int testCaseCount = 2) : base(stream)
        {
            base.MemoryLimit = memoryLimit;
            TimeLimit = TimeSpan.FromMilliseconds(timeLimit);
            TestCaseCount = testCaseCount;
            CompileString = new List<string> {};
            RunScript = "/root/aheui -q main.aheui";
        }

        public void Test()
        {
            Console.WriteLine(new StreamReader("main.aheui").ReadToEnd());
            File.Copy("main.aheui", "grad/main.aheui");
            Start();
        }
    }
}