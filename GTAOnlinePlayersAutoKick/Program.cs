using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GTAOnlinePlayersAutoKick
{
    internal class Program
    {
        const string prefix = "$";
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Welcome to the utility to exclude all players from the game session in GTA Online. Type 'start'.");
            Console.ForegroundColor = ConsoleColor.White;
            CLI();
        }

        static void CLI()
        {
            Console.WriteLine();
            Console.Write(prefix);
            string msg = Console.ReadLine();
            switch (msg)
            {
                case "start":
                    Process[] p = Process.GetProcessesByName("GTA5");
                    Process proc = p[0];
                    proc.Suspend();
                    Console.WriteLine("Процесс заморожен, ожидание 12 секунд");
                    Thread.Sleep(12000);
                    Console.WriteLine("12 секунд прошло, размораживаем процесс");
                    proc.Resume();
                    Console.WriteLine("Процесс разморожен, возвращайтесь в игру");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Command not found");
                    CLI();
                    break;
            }
        }
    }

    [Flags]
    public enum ThreadAccess : int
    {
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200)
    }

    public static class ProcessExtension
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        public static void Suspend(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
            }
        }
        public static void Resume(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(pOpenThread);
            }
        }
    }
}
