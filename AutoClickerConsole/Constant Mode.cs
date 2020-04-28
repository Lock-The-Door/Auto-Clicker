using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoClickerConsole
{
    static class ConstantMode
    {
        public static void ConstantModeMenu()
        {
            // Constant Mode Menu
            Console.Clear();
            Console.WriteLine("Constant Mode");
            Console.WriteLine("In constant mode, the cps remains constant.");
            Console.Write("Set a CPS or type q to go back: ");
            string input = Console.ReadLine();

            // Parse CPS
            if (!double.TryParse(input, out double cps) || cps <= 0)
            {
                if (input.ToLower() == "q")
                {
                    Program.MainMenu();
                    return;
                }
                Console.WriteLine("That is not a positive non-zero number!");
                Thread.Sleep(1000);
                ConstantModeMenu();
                return;
            }

            //Calculate seconds per click
            double spc = 1 / cps;

            // Hand Off to click controller
            Console.WriteLine("The clicker will start when you press [ and stop when you press ] and you can go back by pressing \\");
            ClickController(spc);
        }

        public static bool stop = false;

        private static void ClickController(double spc)
        {
            // Wait for Keypress to start
            char s = ' ';
            while (s != '[' && s != '\\')
                s = Console.ReadKey(true).KeyChar;
            if (s == '\\')
            {
                ConstantModeMenu();
                return;
            }    
            s = ' ';
            Console.WriteLine("Starting... Press ] to stop");
            Thread clickThread = new Thread(new ParameterizedThreadStart(Click));
            stop = false;
            clickThread.Start(spc);
            Thread.Sleep(1000);
            while (s != ']')
                s = Console.ReadKey(true).KeyChar;
            stop = true;
            Console.WriteLine("Stopping... Press [ to start again and \\ to go back");
            ClickController(spc);
        }

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        [DllImport("user32")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons,
    int dwExtraInfo);
        private static void Click(object spcObject)
        {
            double spc = (double)spcObject; // Convert back to string

            System.Timers.Timer timer = new System.Timers.Timer{
                AutoReset = true,
                Interval = spc * 1000,
                Enabled = true,
            };
            timer.Elapsed += (object source, System.Timers.ElapsedEventArgs e) =>
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                if (stop)
                    timer.Close();
            };
        }
    }
}
