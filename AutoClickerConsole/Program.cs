using System;
using System.Threading;

namespace AutoClickerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
        }

        public static void MainMenu()
        {
            // Display Main Menu
            Console.Clear();
            Console.WriteLine("The Multi-Setting Auto-Clicker!");
            Console.WriteLine("There are currently two modes:");
            Console.WriteLine("1. Constant Click Rate");
            Console.WriteLine("2. Variable Click Rate");
            Console.Write("Enter a mode number: ");
            string choice = Console.ReadLine();

            // Parse choice as number
            if (!int.TryParse(choice, out int choiceNumber))
            {
                Console.WriteLine("That is not a number!");
                Thread.Sleep(1000);
                MainMenu();
                return;
            }

            // Read Choice
            switch (choiceNumber)
            {
                case 1:
                    ConstantMode.ConstantModeMenu();
                    break;
                case 2:
                    VariableMode.VariableModeMenu();
                    break;
                default:
                    Console.WriteLine("That is not a valid mode!");
                    break;
            }
        }

        

        
    }
}
