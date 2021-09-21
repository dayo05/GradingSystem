using System;
using System.IO;
using System.Collections.Generic;

using IroojGradingSystem;

namespace LanguageSupport
{
    public class BrainFuck : Base
    {
        public BrainFuck(StreamWriter stream, long memoryLimit = 512 * 1024, long timeLimit = 1000, int testCaseCount = 2) : base(stream)
        {
            base.MemoryLimit = memoryLimit;
            TimeLimit = TimeSpan.FromMilliseconds(timeLimit);
            TestCaseCount = testCaseCount;
            CompileString = new List<string> {};
            RunScript = "/root/brainfuck main.bf";
        }

        public void Test()
        {
            File.Copy("main.bf", "grad/main.bf");
            Start();
        }
    }
}