using System;
using CNC_Interpreter_V2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Gpio;
using System.Device.Pwm;
using System.ComponentModel.Design;
using MCPController;

namespace CNC_Interpreter_V2
{
    internal class GPIOControl
    {
     
        private GpioController _ioControl = new();
        private MCP23017Controller _ioExtender = new();

    // Define the pins for the stepper motor control
        // Pin definitions on main board
        private const int _stepEnable = 22;
        private const int _stepReset = 18;

        // Pin definitions on the IO extender
        private const int _stepX = 8;
        private const int _stepY = 9;
        private const int _stepZ = 10;
        private const int _dirX = 11;
        private const int _dirY = 12;
        private const int _dirZ = 13;

    // Define the pins for the limit switches (IO extender)
        private const int _limitX = 7;
        private const int _limitY = 6;
        private const int _limitZ = 5;

    // Define the pins for the spindle speed and direction control
    // Pin definitions on main board
        private const int _spindlePWM = 13;
        private const int _spindleDir = 6;
        
    // Define the pins for the soft stop
    // Pin definitions on main board
        private const int _softStopLED = 16;
        private const int _softStopButton = 20;

        public GPIOControl()
        {



            // Set up the pins for the stepper motor control
            _ioControl.OpenPin(_stepEnable, PinMode.Output);
            _ioControl.OpenPin(_stepReset, PinMode.Output);

            // Set up the pins for the stepper step control
            _ioExtender.SetPinDirection(_stepX, true);
            _ioExtender.SetPinDirection(_stepY, true);
            _ioExtender.SetPinDirection(_stepZ, true);
            // Set up the pins for stepper direction control
            _ioExtender.SetPinDirection(_dirX, true);
            _ioExtender.SetPinDirection(_dirY, true);
            _ioExtender.SetPinDirection(_dirZ, true);


            // Set up the pins for limit switches
            _ioExtender.SetPinDirection(_limitX, false);
            _ioExtender.SetPinDirection(_limitY, false);
            _ioExtender.SetPinDirection(_limitZ, false);

            // Set up the pins for the spindle speed and direction control
            _ioControl.OpenPin(_spindlePWM, PinMode.Output);
            _ioControl.OpenPin(_spindleDir, PinMode.Output);
            
            // Set up the pins for the soft stop
            // Set up pin for the LED transistor
            _ioControl.OpenPin(_softStopLED, PinMode.Output);
            // Set up pin for soft stop button
            _ioControl.OpenPin(_softStopButton, PinMode.Input);

        }


        private bool _setPin(int IoPin, bool Value)
        {
             _ioControl.Write(IoPin, Value ? PinValue.High : PinValue.Low);
            return true;
        }

        public bool ReadPin(int Pin)
        {
            return _ioControl.Read(Pin) == PinValue.High;
        }

        private bool _setPWM(bool State, int Channel, int Chip, double DutyCycle)
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
            _setPWM(true, 1, 0, DutyCycle);
            _setPin(6, Dir);
            return true;

        }


        public bool StepControl(int Step,bool Dir)
        {
            _ioExtender.WritePin(_dirX, Dir);

            for (int i = 0; i < Step; i++)
            {
                _ioExtender.WritePin(_stepX, true);
                _ioExtender.WritePin(_stepX, false);
            }
            return true;
        }   

    }
}
