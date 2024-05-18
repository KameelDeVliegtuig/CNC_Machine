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
            if (Value == true)
            {
                IOControl.Write(IoPin, PinValue.High);
            }
            else
            {
                IOControl.Write(IoPin, PinValue.Low);
            }
            return true;
        }

        public bool ReadPin(int Pin)
        {
            return true;

        }

        public bool SetPWM(bool State, int Channel, int Chip, double DutyCycle)
        {
            var pwm = PwmChannel.Create(Chip, Channel, 500, DutyCycle);

            if (State == false)
            {
                pwm.Stop();
                return true;
            }
            else if (State == true)
            {

                pwm.Start();
                return true;
            }
            return false;
        }

        public GPIOControl()
        {
            
            // Set up the pins for the spindle speed and direction control
            IOControl.OpenPin(16, PinMode.Output);
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
            SetPWM(true, 1, 0, DutyCycle);
            SetPin(6, Dir);
            return true;

        }


        public bool StepControl(int Step,bool Dir)
        {
            SetPin(13, Dir);
            SetPin(19, true);
            for (int i = 0; i < Step; i++)
            {
                SetPin(19, false);
                SetPin(19, true);
            }
            return true;
        }   

    }
}
