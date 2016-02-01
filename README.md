# FantasticCommonLibrary.ProcessWrapper
cmd 的封装,支持.net 4.0,net4.5以及多命令的同时多线程进行.并且在运行效率方便,运用了线程同步,确保命令执行完才会退出

# Usage

            using FantasticCommonLibrary.ProcessWrapper;
            "tasklist".RunSingleCommand();
            string[] commands = new string[]
            {
                "tasklist",
                "systeminfo",
                "qprocess",
            };
            commands.RunMultiCommand(false);
