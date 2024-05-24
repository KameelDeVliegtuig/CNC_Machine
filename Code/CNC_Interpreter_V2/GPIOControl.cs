﻿using System;
using CNC_Interpreter_V2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Gpio;
using System.Device.Pwm;
using System.ComponentModel.Design;
using MCPController;
using UnitsNet;
using System.Diagnostics;

namespace CNC_Interpreter_V2
{
    internal class GPIOControl
    {
        public enum StepperAxis
        {
            X = 8,
            Y = 9,
            Z = 10
        }

        public enum LimitSwitch
        {
            X = 7,
            Y = 6,
            Z = 5
        }

        private GpioController _ioControl = new();
        private MCP23017Controller _ioExtender = new();

        // Initialize int for current spindle speed
        private int _currentSpindelSpeed;

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
            _ioControl.OpenPin(_spindleDir, PinMode.Output);

            // Set up the pins for the soft stop
            // Set up pin for the LED transistor
            _ioControl.OpenPin(_softStopLED, PinMode.Output);
            // Set up pin for soft stop button
            _ioControl.OpenPin(_softStopButton, PinMode.Input);

            _setPin(_stepReset, true);

        }

        private bool _setPin(int IoPin, bool Value)
        {
            _ioControl.Write(IoPin, Value ? PinValue.High : PinValue.Low);
            return true;
        }

        // Read a pin
        public bool ReadPin(int Pin, bool IsOnExtender)
        {
            if (IsOnExtender)
            {
                return _ioExtender.ReadPin(Pin);
            }
            if (!IsOnExtender)
            {
                return _ioControl.Read(Pin) == PinValue.High;
            }
            return false;
        }

        // Set a PWM signal to specific channel
        private bool _setPWM(bool State, int Channel, int Chip, double DutyCycle)
        {
            var pwm = PwmChannel.Create(Chip, Channel, 5000, DutyCycle);
            pwm.Start();

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

        // Control the spindle speed, direction and stop mode with PWM 
        public bool ControlSpindel(int Speed, bool Dir)
        {
            // Constrain speed to 100
            // If speed is lessthan 0, stop the spindle in a hard manner

            if (Speed > 100)
            {
                Speed = 100;
            }

            if (Speed == 0)
            {
                Console.WriteLine("Spindel controlled stop");

                while (_currentSpindelSpeed > 10)
                {
                    _currentSpindelSpeed = _currentSpindelSpeed / 2;
                    _setPWM(true, 1, 0, (double)_currentSpindelSpeed / 100);
                    _currentSpindelSpeed = Speed;
                    Thread.Sleep(2000);
                }

                _setPWM(false, 1, 0, 0);
                return true;
            }
            else if (Speed < 0)
            {
                Console.WriteLine("Emergency stop");
                _setPWM(false, 1, 0, 0);
                _currentSpindelSpeed = Speed;
            }
            else if (Speed > 0 && Speed <= 100)
            {
                double DutyCycle = (double)Speed / 100;
                _setPWM(true, 1, 0, DutyCycle);
                _setPin(6, Dir);
                _currentSpindelSpeed = Speed;
                Console.WriteLine("Set spindel to: " + Speed);

            }
            return true;
        }

        // 200 microsecond delay needs to be implemented
        // Controls the stepper motors with a specific amount of steps and direction
        public bool ControlStep(bool dir, StepperAxis steppers)
        {
            _setPin(_stepEnable, false);
            _ioExtender.WritePin(((int)steppers + 3), dir);

            _ioExtender.WritePin((int)steppers, true);
            UsDelay(300, Stopwatch.GetTimestamp());
            _ioExtender.WritePin((int)steppers, false);
            UsDelay(300, Stopwatch.GetTimestamp());


            return true;
        }

        // Read the limit switches
        public bool ReadLimitSwitch(LimitSwitch limitSwitch)
        {
            return _ioExtender.ReadPin((int)limitSwitch);
        }

        public bool DisableSteppers()
        {
            _setPin(_stepEnable, true);
            return true;
        }

        public bool SetPin(int pin, bool state)
        {
            if (pin < 5)
            {
                _ioExtender.WritePin(pin, state);
                return true;
            }
            else { return false; }
        }

        // Emergency stop function
        public bool EmergencyStop()
        {
            _setPin(_stepEnable, true);
            ControlSpindel(-1, false);
            return true;
        }

        // Delay for microseconds
        public long UsDelay(int microseconds, long StartTick)
        {
            var targetTicks = ((microseconds) * Stopwatch.Frequency) / 1000000;
            while (Stopwatch.GetTimestamp() - StartTick < targetTicks) continue;
            return StartTick;
        }
    }
}
