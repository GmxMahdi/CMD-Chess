using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace Chess
{
    class ScreenSettings
    {
        #region DLL_IMPORTS
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        #endregion

        const int SWP_NOZORDER = 0x4;
        const int SWP_NOACTIVATE = 0x10;

        static public void SetWindowSize(int x, int y)
        {
            Console.BufferWidth = Console.WindowWidth = x;//130;
            Console.BufferHeight = Console.WindowHeight = y;// 62;
        }
        static public void DisableWindowButtons()
        {
            const int MF_BYCOMMAND = 0x00000000;
            const int SC_MINIMIZE = 0xF020;
            const int SC_MAXIMIZE = 0xF030;
            const int SC_SIZE = 0xF000;

            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }
        }
        public static void SetWindowPosition(int x, int y)
        {
            var screen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;
            SetWindowPos(Handle, IntPtr.Zero, x, y, width*8, height*8, SWP_NOZORDER | SWP_NOACTIVATE);
        }

        private static IntPtr Handle
        {
            get
            {
                //Initialize();
                return GetConsoleWindow();
            }
        }


        public static void AdjustWindowForChess()
        {
            ScreenSettings.SetWindowPosition(10, 10);
            Console.Clear();
            try
            {
                ScreenSettings.SetWindowSize(130, 62);
            }
            catch
            {
                // Si la console ne rentre pas dans l'ecran, change le font.
                try
                {
                    ConsoleFont.SetConsoleFont();
                    ScreenSettings.SetWindowSize(130, 62);
                }
                // Si ca ne vraiment rentre pas... arrete le.
                catch { CloseWithErrorMessage(); }
            }
        }

        private static void CloseWithErrorMessage()
        {
            Console.Clear();
            Console.CursorVisible = false;
            ScreenSettings.SetWindowSize(83, 20);

            var screen = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            int width = screen.Width;
            int height = screen.Height;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n Désoler, mais la résolution de votre écran (");

            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(width + "x" + height);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(") est trop petite...\n");
            Console.Write(" J'ai faits mes calculs, puis vous devez avoir une résolution minimum de ");

            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.Write("1040x900");

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("\n pour éviter que les graphiques explosent.\n\n Voici des exemples de résolutions acceptés:\n ");

            string[] legalResolutions = { "1920x1080", "1768x992", "1680x1050", "1600x1200" };
            foreach (string res in legalResolutions)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(res);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(" ");
            }

            Console.WriteLine("\n\n J'aurai pu adapter les graphiques selon la résolution de l'écran,\n mais là je n'avais pas trop envi d'exploser davantage ma tête avec cmd et les res.");
            Console.Write(" La page de résolution est déjà ouverte pour vous :) \n Vérifier si vous pouvez augmenter la résolution.\n Merci\n\n ");

            OpenResolutionPage();
            Thread.Sleep(10000);
            Environment.Exit(0);
        }

        private static void OpenResolutionPage()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("desk.cpl");
            cmd.StandardInput.Close();
            cmd.WaitForExit();
        }
    }
}
