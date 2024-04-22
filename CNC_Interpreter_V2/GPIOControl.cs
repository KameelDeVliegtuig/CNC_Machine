using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.GPIO;

namespace CNC_Interpreter_V2
{
    internal class GPIOControl
    {
        public bool SetPin(int Pin, bool Value)
        {
            return true;
        }

        public bool ReadPin(int Pin)
        {
            return true;

        }

        public bool SetPWM(int Pin, int DutyCycle)
        {
            return true;
        }
        
        public bool PinSetup()
        {
            GPIOController IOControl = new();
            IOControl.OpenPin();
            return true;
        }

        public bool ControlSpindel(bool Dir, int Speed)
        {



            return true;
        }
    }
}
