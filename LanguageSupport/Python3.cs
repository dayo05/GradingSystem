using System;
using System.IO;
using System.Collections.Generic;

using IroojGradingSystem;

namespace LanguageSupport
{
    public class Python3 : Base
    {
        public Python3(StreamWriter stream, long memoryLimit = 512 * 1024, long timeLimit = 1000,
            int testCaseCount = 2) : base(stream)
        {
            base.MemoryLimit = memoryLimit;
            TimeLimit = TimeSpan.FromMilliseconds(timeLimit);
            TestCaseCount = testCaseCount;
            CompileString = new List<string> {"python3 -c \"import py_compile; py_compile.compile(r'main.py')\""};
            RunScript = "python3 main.py";
        }

        public void Test()
        {
            File.Copy("main.py", "grad/main.py");
            Start();
        }
    }
}