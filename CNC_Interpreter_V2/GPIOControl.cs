using System;
using CNC_Interpreter_V2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Gpio;
using System.Device.Pwm;
using System.ComponentModel.Design;

namespace CNC_Interpreter_V2
{
    internal class GPIOControl
    {
        private GpioController IOControl = new();

        public bool SetPin(int IoPin, bool Value)
        {
            IOControl.Write(IoPin, Value);
        }

        public bool ReadPin(int Pin)
        {
            return true;

        }

        public bool SetPWM(bool State, int Channel, int Chip, double DutyCycle)
        {
            if (State == false)
            {
                pwm.Stop();
                return true;
            }
            else if (State == true)
            {

                var pwm = PwmChannel.Create(Channel, Chip, 500, DutyCycle);
                pwm.Start();
                return true;
            }
            return false;
        }

        public GPIOControl()
        {
            
            // Set up the pins for the spindle speed and direction control

            IOControl.OpenPin(6, PinMode.Output);
            IOControl.OpenPin(13, PinMode.Output);

        }

        public bool ControlSpindel(int Speed, bool Dir)
        {
            if (Speed > 100)
            {
                Speed = 100;
            }
            else if (Speed < 0)
            {
                Speed = 0;
            }
            double DutyCycle = Speed / 100;
            SetPWM(true, 6, 0, DutyCycle);
            SetPin(6, Dir);
            return true;

        }
    }
}
