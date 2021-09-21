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
            new GradRust(new StreamWriter(Console.OpenStandardOutput())).Test();
        }
    }

    public class GradRust : Base
    {
        public GradRust(StreamWriter stream) : base(stream)
        {
            MemoryLimit = 512 * 1024;
            TimeLimit = TimeSpan.FromMilliseconds(1000);
            TestCaseCount = 2;
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