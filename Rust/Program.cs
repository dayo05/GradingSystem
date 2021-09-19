using System;
using System.Collections.Generic;
using System.IO;
using IroojGradingSystem;

namespace Rust
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new GradRust().Test();
        }
    }

    class GradRust : Base
    {
        public GradRust()
        {
            MemoryLimit = 512 * 1024;
            TimeLimit = TimeSpan.FromMilliseconds(1000);
            TestCaseCount = 2;
            CompileString = new List<string> {"/home/dayo/.rustup/toolchains/stable-x86_64-unknown-linux-gnu/bin/rustc main.rs"};
            RunScript = "./main";
            File.Copy("main.rs", "grad/main.rs");
        }

        public void Test()
            => Start();
    }
}

// Install: curl https://sh.rustup.rs/ -sSf | sh