using System;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;

namespace Microwave.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Button startCancelButton = new Button();
            Button powerButton = new Button();
            Button timeButton = new Button();

            Door door = new Door();

            Output output = new Output();

            Display display = new Display(output);

            PowerTube powerTube = new PowerTube(output);

            Light light = new Light(output);

            Microwave.Classes.Boundary.Timer timer = new Timer();

            CookController cooker = new CookController(timer, display, powerTube);

            UserInterface ui = new UserInterface(powerButton, timeButton, startCancelButton, door, display, light, cooker);

            // Finish the double association
            cooker.UI = ui;

            door.Open();
            Console.WriteLine("Inserting food");
            door.Close();
            // Simulate a simple sequence
            for (int i = 0; i < 14; i++)
            {
                powerButton.Press();
            }
            

            for (int i = 0; i < 1; i++)
            {
                timeButton.Press();
            }
            

            startCancelButton.Press();
            
            Console.WriteLine();
            //door.Open();
            Console.WriteLine();

            // The simple sequence should now run

            System.Console.WriteLine("When you press enter, the program will stop");
            // Wait for input

            System.Console.ReadLine();
            //HEJ PALLE!
            //Hej ALEX!!
        }
    }
}
