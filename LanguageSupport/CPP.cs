using System;
using System.IO;
using System.Collections.Generic;

using IroojGradingSystem;

namespace LanguageSupport
{
    public class CPP : Base
    {
        public CPP(StreamWriter stream, long memoryLimit = 512 * 1024, long timeLimit = 1000, int testCaseCount = 2) : base(stream)
        {
            base.MemoryLimit = memoryLimit;
            TimeLimit = TimeSpan.FromMilliseconds(timeLimit);
            TestCaseCount = testCaseCount;
            CompileString = new List<string> {"/bin/g++ -o main main.cpp"};
            RunScript = "./main";
        }

        public void Test()
        {
            File.Copy("main.cpp", "grad/main.cpp");
            Start();
        }
    }
}