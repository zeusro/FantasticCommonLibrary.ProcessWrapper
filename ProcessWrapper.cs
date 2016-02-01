using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FantasticCommonLibrary.ProcessWrapper
{
    public static class ProcessWrapper
    {
#if NET40
               /// <summary>
        /// 执行单条命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="isOutput">是否输出</param>
        public static void RunSingleCommand(this string command, bool isOutput = true)
        {
            AutoResetEvent auto = new AutoResetEvent(false);
            Task task = new Task((Action)(() =>
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe ";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;//是否显示DOS窗口，true代表隐藏;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    if (isOutput)
                        process.OutputDataReceived += (DataReceivedEventHandler)((sender, e) => Console.WriteLine(e.Data));
                    process.ErrorDataReceived += (DataReceivedEventHandler)((sender, e) => Console.WriteLine(e.Data));
                    process.EnableRaisingEvents = true;//配合事件委托使用
                    process.Exited += (EventHandler)((sender, e) => auto.Set());
                    process.Start();
                    using (StreamWriter standardInput = process.StandardInput)
                    {
                        standardInput.AutoFlush = true;
                        process.BeginOutputReadLine();
                        standardInput.WriteLine(command);
                    }
                    process.WaitForExit();
                }
            }), TaskCreationOptions.LongRunning);
            task.Start();
            task.Wait();
            //无限等待
            auto.WaitOne();
        }

        /// <summary>
        /// 执行多条命令
        /// </summary>
        /// <param name="commands">多条命令</param>
        /// <param name="isOutput">是否输出</param>
        public static void RunMultiCommand(this IEnumerable<string> commands, bool isOutput = true)
        {
            if (commands == null || !Enumerable.Any<string>(commands))
                throw new ArgumentException("去屎", "commands");
            Action<string> runCommand = (Action<string>)(command =>
            {
                AutoResetEvent auto = new AutoResetEvent(false);
                Task task = new Task((Action)(() =>
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "cmd.exe ";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;//是否显示DOS窗口，true代表隐藏;
                        process.StartInfo.RedirectStandardInput = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        if (isOutput)
                            process.OutputDataReceived += (DataReceivedEventHandler)((sender, e) => Console.WriteLine(e.Data));
                        process.ErrorDataReceived += (DataReceivedEventHandler)((sender, e) => Console.WriteLine(e.Data));
                        process.EnableRaisingEvents = true;//配合事件委托使用
                        process.Exited += (EventHandler)((sender, e) => auto.Set());
                        process.Start();
                        using (StreamWriter standardInput = process.StandardInput)
                        {
                            standardInput.AutoFlush = true;
                            process.BeginOutputReadLine();
                            standardInput.WriteLine(command);
                        }
                        process.WaitForExit();
                    }
                }), TaskCreationOptions.LongRunning);
                task.Start();
                task.Wait();
                auto.WaitOne();
            });
            CountdownEvent countEvent = new CountdownEvent(Enumerable.Count<string>(commands));
            for (int index = 0; index < Enumerable.Count<string>(commands); ++index)
            {
                /* i variable refers to the same memory location throughout the loop’s lifetime.
       Therefore, each thread calls Console.Write on a variable whose value may change as it is running!*/
                //see http://www.albahari.com/threading/
                int temp = index;
                new Thread((ThreadStart)(() =>
                {
                    runCommand(Enumerable.ElementAt<string>(commands, temp));
                    countEvent.Signal();
                })).Start();
            }
            countEvent.Wait(-1);
        }
#endif

#if NET45
        /// <summary>
        /// 执行单条命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="isOutput">是否输出</param>
        public static void RunSingleCommand(this string command, bool isOutput = true)
        {
            AutoResetEvent auto = new AutoResetEvent(false);
            Task task = new Task((async () =>
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe ";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;//是否显示DOS窗口，true代表隐藏;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    if (isOutput)
                        process.OutputDataReceived += (DataReceivedEventHandler)((sender, e) => Console.WriteLine(e.Data));
                    process.ErrorDataReceived += (DataReceivedEventHandler)((sender, e) => Console.WriteLine(e.Data));
                    process.EnableRaisingEvents = true;//配合事件委托使用
                    process.Exited += (EventHandler)((sender, e) => auto.Set());
                    process.Start();
                    using (StreamWriter standardInput = process.StandardInput)
                    {
                        standardInput.AutoFlush = true;
                        process.BeginOutputReadLine();
                        await standardInput.WriteLineAsync(command);
                    }
                    process.WaitForExit();
                }
            }), TaskCreationOptions.LongRunning);
            task.Start();
            task.Wait();
            //无限等待
            auto.WaitOne();
        }

        /// <summary>
        /// 执行多条命令
        /// </summary>
        /// <param name="commands">多条命令</param>
        /// <param name="isOutput">是否输出</param>
        public static void RunMultiCommand(this IEnumerable<string> commands, bool isOutput = true)
        {
            if (commands == null || !Enumerable.Any<string>(commands))
                throw new ArgumentException("去屎", "commands");
            Action<string> runCommand = (command =>
            {
                AutoResetEvent auto = new AutoResetEvent(false);
                Task task = new Task((async () =>
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "cmd.exe ";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;//是否显示DOS窗口，true代表隐藏;
                        process.StartInfo.RedirectStandardInput = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;
                        if (isOutput)
                            process.OutputDataReceived += (DataReceivedEventHandler)((sender, e) => Console.WriteLine(e.Data));
                        process.ErrorDataReceived += (DataReceivedEventHandler)((sender, e) => Console.WriteLine(e.Data));
                        process.EnableRaisingEvents = true;//配合事件委托使用
                        process.Exited += (EventHandler)((sender, e) => auto.Set());
                        process.Start();
                        using (StreamWriter standardInput = process.StandardInput)
                        {
                            standardInput.AutoFlush = true;
                            process.BeginOutputReadLine();
                            await standardInput.WriteLineAsync(command);
                        }
                        process.WaitForExit();
                    }
                }), TaskCreationOptions.LongRunning);
                task.Start();
                task.Wait();
                auto.WaitOne();
            });
            CountdownEvent countEvent = new CountdownEvent(Enumerable.Count<string>(commands));
            for (int index = 0; index < Enumerable.Count<string>(commands); ++index)
            {
                /* i variable refers to the same memory location throughout the loop’s lifetime.
       Therefore, each thread calls Console.Write on a variable whose value may change as it is running!*/
                //see http://www.albahari.com/threading/
                int temp = index;
                new Thread((ThreadStart)(() =>
                {
                    runCommand(Enumerable.ElementAt<string>(commands, temp));
                    countEvent.Signal();
                })).Start();
            }
            countEvent.Wait(-1);
        }
#endif

    }
}
