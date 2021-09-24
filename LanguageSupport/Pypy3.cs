using System;
using System.Collections.Generic;
using System.IO;
using IroojGradingSystem;

namespace LanguageSupport
{
    public class Pypy3 : Base
    {
        public Pypy3(StreamWriter stream, long memoryLimit = 512 * 1024, long timeLimit = 1000,
            int testCaseCount = 2) : base(stream)
        {
            base.MemoryLimit = memoryLimit;
            TimeLimit = TimeSpan.FromMilliseconds(timeLimit);
            TestCaseCount = testCaseCount;
            CompileString = new List<string> {"pypy3 -c \"import py_compile; py_compile.compile(r'main.py')\""};
            RunScript = "pypy3 main.py";
        }

        public void Test()
        {
            File.Copy("main.py", "grad/main.py");
            Start();
        }
    }
}