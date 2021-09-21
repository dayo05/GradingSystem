﻿using System;
using System.IO;
using System.Collections.Generic;

using IroojGradingSystem;

namespace Rust
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new GradRust(new StreamWriter(Console.OpenStandardOutput())).Test();
        }
    }

    public class GradRust : Base
    {
        public GradRust(StreamWriter stream, long memoryLimit = 512 * 1024, long timeLimit = 1000, int testCaseCount = 2) : base(stream)
        {
            base.MemoryLimit = memoryLimit;
            TimeLimit = TimeSpan.FromMilliseconds(timeLimit);
            TestCaseCount = testCaseCount;
            CompileString = new List<string> {"/root/.rustup/toolchains/stable-x86_64-unknown-linux-gnu/bin/rustc main.rs"};
            RunScript = "./main";
        }

        public void Test()
        {
            File.Copy("main.rs", "grad/main.rs");
            Start();
        }
    }
}

// Install: curl https://sh.rustup.rs/ -sSf | sh