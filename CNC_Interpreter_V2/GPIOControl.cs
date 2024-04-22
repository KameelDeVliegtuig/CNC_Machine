using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            //GiHub test
        }

        public bool SetPWM(int Pin, int DutyCycle)
        {
            return true;
        }
    }
}
