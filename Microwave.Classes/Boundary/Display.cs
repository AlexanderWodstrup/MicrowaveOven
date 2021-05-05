using System.Runtime.InteropServices;
using Microwave.Classes.Interfaces;

namespace Microwave.Classes.Boundary
{
    public class Display : IDisplay
    {
        private IOutput myOutput;

        public Display(IOutput output)
        {
            myOutput = output;
        }

        public void ShowTime(int min, int sec)
        {
            if (sec < 10)
            {
                string secString = "0" + sec.ToString();
                myOutput.OutputLine($"Display shows: " + min + ":" + secString);
            }
            else
            {
                myOutput.OutputLine($"Display shows: " + min + ":" + sec);
            }
            
        }

        public void ShowPower(int power)
        {
            myOutput.OutputLine($"Display shows: {power} W");
        }

        public void Clear()
        {
            myOutput.OutputLine($"Display cleared");
        }
    }
}