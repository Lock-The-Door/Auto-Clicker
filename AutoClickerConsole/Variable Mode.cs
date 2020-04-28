using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoClickerConsole
{
    class VariableMode
    {
        public static void VariableModeMenu(double setMin = 0, double setMax = 0)
        {
            double minCps = setMin;
            double maxCps = setMax;

            if (minCps == 0)
            {
                // Constant Mode Menu
                Console.Clear();
                Console.WriteLine("Variable Mode");
                Console.WriteLine("In variable mode, the cps changes.");
                Console.Write("First, type a minimum CPS or type q to go back: ");
                string minInput = Console.ReadLine();

                // Parse CPS
                if (!double.TryParse(minInput, out minCps) || minCps <= 0)
                {
                    if (minInput.ToLower() == "q")
                    {
                        Program.MainMenu();
                        return;
                    }
                    Console.WriteLine("That is not a positive non-zero number!");
                    Thread.Sleep(1000);
                    VariableModeMenu();
                    return;
                }
            }

            if (maxCps == 0)
            {
                Console.Write("Now, type a max CPS or type b to go back: ");
                string maxInput = Console.ReadLine();

                // Parse Max CPS
                if (!double.TryParse(maxInput, out maxCps))
                {
                    if (maxInput.ToLower() == "b")
                    {
                        VariableModeMenu();
                        return;
                    }
                    Console.WriteLine("That is not a positive non-zero number!");
                    Thread.Sleep(1000);
                    VariableModeMenu();
                    return;
                }
                else if (minCps > maxCps)
                {
                    Console.WriteLine("The minimum CPS is bigger than the maximum CPS! Type a new max CPS or type b to go back!");
                    Thread.Sleep(1000);
                    VariableModeMenu(minCps);
                    return;
                }
            }

            //Calculate seconds per click
            double minSpc = 1 / minCps;
            double maxSpc = 1 / maxCps;

            // Hand Off to click controller
            Console.WriteLine("The clicker will start when you press [ and stop when you press ] and you can go back by pressing \\");
            ClickController(minSpc, maxSpc);
        }

        public static bool stop = false;

        private static void ClickController(double minSpc, double maxSpc)
        {
            // Wait for Keypress to start
            char s = ' ';
            while (s != '[' && s != '\\')
                s = Console.ReadKey(false).KeyChar;
            if (s == '\\')
            {
                VariableModeMenu();
                return;
            }
            s = ' ';
            Console.WriteLine("Starting... Press ] to stop");
            Thread clickThread = new Thread(new ParameterizedThreadStart(Click));
            stop = false;
            clickThread.Start(new double[]{ minSpc, maxSpc});
            Thread.Sleep(1000);
            while (s != ']')
                s = Console.ReadKey(false).KeyChar;
            stop = true;
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            Console.WriteLine("Stopping... Press [ to start again and \\ to go back");
            ClickController(minSpc, maxSpc);
        }

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        [DllImport("user32")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons,
    int dwExtraInfo);
        private static void Click(object spcObject)
        {
            double[] spcValues = (double[])spcObject;
            double minSpc = float.Parse(spcValues[0].ToString()); // Convert back to string
            double maxSpc = double.Parse(spcValues[1].ToString());

            Random random = new Random();

            System.Timers.Timer timer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = (random.NextDouble() * (maxSpc - minSpc) + minSpc) * 1000,
                Enabled = true,
            };
            timer.Elapsed += (object source, System.Timers.ElapsedEventArgs e) =>
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                timer.Interval = (random.NextDouble() * (maxSpc - minSpc) + minSpc) * 1000;
                if (stop)
                    timer.Close();
            };
        }
    }
}
