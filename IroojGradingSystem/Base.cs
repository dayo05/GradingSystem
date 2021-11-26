using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace IroojGradingSystem
{
    public class Base
    {
        public long QuestionNumber { get; init; }
        protected List<string> CompileString { get; init; }
        protected string RunScript { get; init; }
        public long MemoryLimit { get; init; }
        public TimeSpan TimeLimit { get; init; }
        public int TestCaseCount { get; init; }
        public long JudgeNumber { get; init; }
        private StreamWriter OutputStream { get; init; }
        protected Base(StreamWriter stream)
        {
            OutputStream = stream;
        }

        /// <summary>
        /// Make RTE more prettier
        /// </summary>
        /// <param name="error">Runtime error string</param>
        /// <returns></returns>
        protected virtual string CheckRteString(string error)
        {
            return error;
        }

        /// <summary>
        /// Compile source
        /// </summary>
        private bool Compile()
        {
            if (CompileString == null) return true;
            foreach (var process in CompileString.Select(Run))
            {
                if (process == null)
                {
                    SendResult(Result.Error, XmlMessage("Cannot found compile program"));
                    return false;
                }
                process.WaitForExit();
                if (process.ExitCode == 0) continue;
                SendResult(Result.CE, XmlMessage(process.StandardError.ReadToEnd()));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Launch source and Check is right soruce
        /// </summary>
        private void Launch()
        {
            var maxTime = TimeSpan.Zero;
            long maxMemory = 0;
            
            for (var i = 1; i <= TestCaseCount; i++)
            {
                SendResult(Result.Running, XmlMessage("Running: " + i + "/" + TestCaseCount));
                using(var execSh = new StreamWriter("exec.sh"))
                {
                    execSh.WriteLine($"{RunScript} < grad_input/{i}.in > output");
                }
                
                var process = Run("/bin/bash exec.sh");
                if (process == null)
                {
                    SendResult(Result.Error, XmlMessage("Cannot run program"));
                    return;
                }

                var stopwatch = new Stopwatch();
                var killed = false;
                new Thread(() =>
                {
                    stopwatch.Start();
                    while (true)
                    {
                        try
                        {
                            maxMemory = Math.Max(process.WorkingSet64, maxMemory);
                            if (process.WorkingSet64 > MemoryLimit * 1024)
                            {
                                killed = true;
                                process.Kill();
                                SendResult(Result.MLE);
                                break;
                            }

                            if (stopwatch.Elapsed > TimeLimit)
                            {
                                killed = true;
                                process.Kill();
                                SendResult(Result.TLE);
                                break;
                            }

                            Thread.Sleep(1);
                            process.Refresh();
                        }
                        catch (InvalidOperationException)
                        {
                            break;
                        }
                    }
                }).Start();
                process.WaitForExit();
                stopwatch.Stop();

                if (killed) return;
                if (process.ExitCode != 0)
                {
                    Console.WriteLine(process.ExitCode);
                    SendResult(Result.RTE, XmlMessage("NZEC"));
                    //return;
                }
                
                var error = process.StandardError;
                var e = error.ReadToEnd().Trim();
                if (e != "")
                {
                    SendResult(Result.RTE, XmlMessage(CheckRteString(e)));
                    return;
                }
                using var ans = new StreamReader($"grad_output/{i}.out");
                var output = new StreamReader("output");
                
                while (true)
                {
                    var ansString = "";
                    var optString = "";
                    while (ansString == "")
                    {
                        if (ans.EndOfStream) break;
                        ansString = ans.ReadLine()?.Trim();
                    }

                    while (optString == "")
                    {
                        if (output.EndOfStream) break;
                        optString = output.ReadLine()?.Trim();
                    }

                    if (ansString == "" && optString == "") break;
                    if (ansString != optString)
                    {
                        SendResult(Result.WA, new XElement("root",
                            new XElement("user_ans", new StreamReader("output").ReadToEnd().Trim()),
                            new XElement("tc_num", i)));
                        return;
                    }
                }

                maxTime = maxTime > stopwatch.Elapsed ? maxTime : stopwatch.Elapsed;
            }

            SendResult(Result.AC, new XElement("root",
                new XElement("time", maxTime.Milliseconds),
                new XElement("memory", maxMemory / 1024)));
        }

        public void Start()
        {
            SendResult(Result.Running, XmlMessage("Start initialize"));
            // Copy Testcase
            DirectoryCopy($"grad_input/{QuestionNumber}/", $"grad/grad_input/");
            DirectoryCopy($"grad_output/{QuestionNumber}/", $"grad/grad_output/");
            
            Directory.SetCurrentDirectory("grad");
            SendResult(Result.Running, XmlMessage("Start Compile"));
            if (Compile())
            {
                SendResult(Result.Running, XmlMessage("Launch"));
                Launch(); 
            }
            Directory.SetCurrentDirectory("..");
            Directory.Delete("grad", true);
            Directory.CreateDirectory("grad");
        }

        private XElement XmlMessage(string message)
            => new("root",
                new XElement("message", message));

        /// <summary>
        /// Send Result to Main server
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="xml">Message</param>
        protected virtual void SendResult(Result result, XElement xml = null)
        {
            if (xml == null)
            {
                var o = new XElement("root",
                    new XElement("Result", result.ToString()), new XElement("judge_number", JudgeNumber));
                OutputStream.WriteLine(o.ToString().Length);
                OutputStream.Write(o.ToString());
            }
            else
            {
                xml.Add(new XElement("Result", result.ToString()));
                xml.Add(new XElement("judge_number", JudgeNumber));
                OutputStream.WriteLine(xml.ToString().Length);
                OutputStream.Write(xml.ToString());
            }
            OutputStream.Flush();
        }

        private static Process Run(string exec) =>
            Process.Start(new ProcessStartInfo()
            {
                FileName = exec.Split()[0],
                Arguments = string.Join(' ', exec.Split().Length != 1 ? exec.Split()[1..] : new[]{""}),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Directory.GetCurrentDirectory()
            });
        
        /// <summary>
        /// Copy Directory
        /// </summary>
        /// <param name="sourceDirName">From</param>
        /// <param name="destDirName">To</param>
        /// <param name="copySubDirs">true</param>
        /// <exception cref="DirectoryNotFoundException">Source directory not found</exception>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
        
            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);        

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);
                //file.CopyTo(tempPath, false);
                
                File.Copy(file.FullName, tempPath);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!copySubDirs) return;
            {
                foreach (var subdir in dirs)
                {
                    var tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath);
                }
            }
        }
    }
}