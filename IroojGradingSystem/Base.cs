﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace IroojGradingSystem
{
    public class Base
    {
        protected List<string> CompileString { get; init; }
        protected string RunScript { get; init; }
        protected long MemoryLimit { get; set; }
        protected TimeSpan TimeLimit { get; set; }
        protected int TestCaseCount { get; set; }

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
                    SendResult(Result.Error, "Cannot found compile program");
                    return false;
                }
                process.WaitForExit();
                if (process.ExitCode == 0) continue;
                SendResult(Result.CE, process.StandardError.ReadToEnd());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Launch source and Check is right soruce
        /// </summary>
        private void Launch()
        {
            TimeSpan maxTime = TimeSpan.Zero;
            long maxMemory = 0;
            
            for (var i = 1; i <= TestCaseCount; i++)
            {
                using(var execSh = new StreamWriter("exec.sh"))
                {
                    execSh.WriteLine($"{RunScript} < grad_input/{i}");
                }
                
                var process = Run("/bin/bash exec.sh");
                if (process == null)
                {
                    SendResult(Result.Error, "Cannot run program");
                    return;
                }

                var stopwatch = new Stopwatch();
                var killed = false;
                new Thread(() =>
                {
                    stopwatch.Start();
                    while (!process.HasExited)
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
                }).Start();
                process.WaitForExit();
                stopwatch.Stop();

                if (killed) return;
                if (process.ExitCode != 0)
                {
                    SendResult(Result.RTE, "NZEC");
                    return;
                }
                
                var error = process.StandardError;
                var e = error.ReadToEnd().Trim();
                if (e != "")
                {
                    SendResult(Result.RTE, CheckRteString(e));
                    return;
                }
                using var ans = new StreamReader("grad_output/" + i);
                var output = process.StandardOutput;
                
                while (true)
                {
                    var ansString = "";
                    var optString = "";
                    while (ansString == "")
                    {
                        if (ans.EndOfStream) break;
                        ansString = ans.ReadLine().Trim();
                    }

                    while (optString == "")
                    {
                        if (output.EndOfStream) break;
                        optString = output.ReadLine().Trim();
                    }

                    if (ansString == "" && optString == "") break;
                    if (ansString != optString)
                    {
                        SendResult(Result.WA);
                        return;
                    }
                }

                maxTime = maxTime > stopwatch.Elapsed ? maxTime : stopwatch.Elapsed;
                SendResult(Result.Running, "Checked: " + i);
            }
            SendResult(Result.AC, $"{maxMemory / 1024}KiB {maxTime.Milliseconds}ms");
        }

        public void Start()
        {
            SendResult(Result.Running, "Start");
            if (Compile())
            {
                SendResult(Result.Running, "Compiled");
                Launch();
            }
        }
        /// <summary>
        /// Send Result to Main server
        /// </summary>
        /// <param name="result">Result</param>
        /// <param name="message">To check RTE, CE</param>
        public virtual void SendResult(Result result, string message = "")
        {
            //throw new NotImplementedException();
            Console.WriteLine(result + " " + message);
        }

        private static Process Run(string exec) =>
            Process.Start(new ProcessStartInfo()
            {
                FileName = exec.Split()[0],
                Arguments = string.Join(' ', exec.Split().Length != 1 ? exec.Split()[1..] : new string[]{""}),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = Directory.GetCurrentDirectory()
            });
    }
}