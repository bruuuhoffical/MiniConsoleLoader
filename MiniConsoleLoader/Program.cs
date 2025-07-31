using KeyAuth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Microsoft.Win32;
namespace MiniConsoleLoader
{
    internal class Program
    {
        public static api KeyAuthApp = new api(
            name: "AIMBOT",
            ownerid: "JOzxAPywrc",
            version: "1.5"
        );
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern ushort GlobalFindAtom(string lpString);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int WS_SIZEBOX = 0x00040000;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;

        public static Session Config = new Session();
        
        [STAThread]
        static void Main()
        {
            var name = "BRUUUH CHEATS ";
            var creds = LoadCredentials();
            string version = GetApplicationVersion();
            Console.Title = name + $"V{version}";
            int width = 40;
            int height = 15;
            string username, password;
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);

            // Disable resize and maximize
            IntPtr consoleHandle = GetConsoleWindow();
            int style = GetWindowLong(consoleHandle, GWL_STYLE);
            style &= ~WS_MAXIMIZEBOX;
            style &= ~WS_SIZEBOX;
            SetWindowLong(consoleHandle, GWL_STYLE, style);


            TypeColored($" Welcome to {name}V{version}", ConsoleColor.Blue, false, 30);
            TypeLine("");
            TypeLine(" =====================================", 0);
            securityChecks();
            TypeLine(" [+] Loading...", 120);
            Task.Delay(1600).Wait();
            if (creds.HasValue)
            {
                username = creds.Value.Username;
                password = creds.Value.Password;
                TypeLine(" [+] Attempting auto-login...", 30);
                KeyAuthApp.init();
                KeyAuthApp.login(username, password);
                if (KeyAuthApp.response.success)
                {
                    for (var i = 0; i < KeyAuthApp.user_data.subscriptions.Count; i++)
                    {
                        TypeColored(" [+] Subscription: " + KeyAuthApp.user_data.subscriptions[i].subscription, ConsoleColor.Blue);
                        Config.Subscription = KeyAuthApp.user_data.subscriptions[i].subscription;
                    }
                    TypeColored($" [+] Logged in as {username}", ConsoleColor.Green);
                    Config.Username = username;
                    Config.Password = password;

                    Config.IsLoggedIn = true;
                    Task.Delay(10000).Wait();
                    Console.ReadLine();
                    //Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    //ShowWindow(consoleHandle, SW_HIDE);
                    //Application.Run(new Aimbot());
                }
                else if (!KeyAuthApp.response.success)
                {
                    Console.WriteLine(" [+] Error: " + KeyAuthApp.response.message);
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                }
            }
            else
            {
                TypeLine($" [+] Connecting to {name.Remove(6, 14)} Database");
                KeyAuthApp.init();
                //TypeLine(" [+] Connection Success");

                Console.Write(" [+] Enter username: ");
                username = Console.ReadLine();
                Console.Write(" [+] Enter password: ");
                password = Console.ReadLine();
                KeyAuthApp.login(username, password);

                if (KeyAuthApp.response.success)
                {
                    for (var i = 0; i < KeyAuthApp.user_data.subscriptions.Count; i++)
                    {
                        TypeColored(" [+] Subscription: " + KeyAuthApp.user_data.subscriptions[i].subscription, ConsoleColor.Blue);
                        Config.Subscription = KeyAuthApp.user_data.subscriptions[i].subscription;
                    }
                    TypeColored($" [+] Logged in as {username}", ConsoleColor.Green);
                    string sub = KeyAuthApp.user_data.subscriptions.Count > 0
                    ? KeyAuthApp.user_data.subscriptions[0].subscription
                    : "Unknown";
                    SaveCredentials(username, password, sub);
                    Config.Username = username;
                    Config.Password = password;

                    Config.IsLoggedIn = true;
                    Task.Delay(10000).Wait();
                    Console.ReadLine();
                    //Application.EnableVisualStyles();
                    //Application.SetCompatibleTextRenderingDefault(false);
                    //ShowWindow(consoleHandle, SW_HIDE);
                    //Application.Run(new Aimbot());
                }
                else if (!KeyAuthApp.response.success)
                {
                    Console.WriteLine(" [+] Error: " + KeyAuthApp.response.message);
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                }
            }
            //Hide
        }
        static string GetApplicationVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            var parts = new List<int> { version.Major, version.Minor };

            if (version.Build > 0 || version.Revision > 0)
                parts.Add(version.Build);

            if (version.Revision > 0)
                parts.Add(version.Revision);

            return string.Join(".", parts);
        }
        static void TypeLine(string text, int delay = 30)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(delay);
            }
            Console.WriteLine();
        }
        static void TypeColored(string text, ConsoleColor color, bool newLine = true, int delay = 30)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(delay);
            }
            Console.ResetColor();
            if (newLine)
                Console.WriteLine();
        }
        static void SaveCredentials(string username, string password, string subscription)
        {
            string credpath = "BRUUUH_CHEATS";//Where your user information is stored i made this easier for bots
            //Example Company\Prdouct or Company or Product anything you love but this is an exmaple
            var key = Registry.CurrentUser.CreateSubKey($@"Software\{credpath}\MiniConsoleLoader");
            key.SetValue("Username", username);
            key.SetValue("Password", password);
            key.SetValue("Subscription", subscription);
            key.Close();
        }
        static (string Username, string Password, string Subscription)? LoadCredentials()
        {
            string credpath = "BRUUUH_CHEATS";//Where your user information is stored i made this easier for bots
            //Example Company\Prdouct or Company or Product anything you love but this is an exmaple
            var key = Registry.CurrentUser.OpenSubKey($@"Software\{credpath}\MiniConsoleLoader");
            if (key == null) return null;

            string username = key.GetValue("Username") as string;
            string password = key.GetValue("Password") as string;
            string subscription = key.GetValue("Subscription") as string;
            key.Close();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                return (username, password, subscription);

            return null;
        }

        static void checkAtom()
        {
            Thread atomCheckThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(60000); // give people 1 minute to login

                    ushort foundAtom = GlobalFindAtom(KeyAuthApp.ownerid);
                    if (foundAtom == 0)
                    {
                        TerminateProcess(GetCurrentProcess(), 1);
                    }
                }
            });

            atomCheckThread.IsBackground = true; // Ensure the thread does not block program exit
            atomCheckThread.Start();
        }
        static void securityChecks()
        {
            // check if the Loader was executed by a different program
            var frames = new StackTrace().GetFrames();
            foreach (var frame in frames)
            {
                MethodBase method = frame.GetMethod();
                if (method != null && method.DeclaringType?.Assembly != Assembly.GetExecutingAssembly())
                {
                    TerminateProcess(GetCurrentProcess(), 1);
                }
            }

            // check if HarmonyLib is attempting to poison our program
            var harmonyAssembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "0Harmony");

            if (harmonyAssembly != null)
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }

            checkAtom();
        }
    }
}
