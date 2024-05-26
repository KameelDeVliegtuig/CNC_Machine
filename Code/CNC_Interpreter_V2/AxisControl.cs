using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnitsNet;
using static CNC_Interpreter_V2.GPIOControl;

namespace CNC_Interpreter_V2
{
    internal class AxisControl
    {
        // Positive and negative movement of axis
        private const bool NEGATIVE = false;
        private const bool POSITIVE = true;

        private GPIOControl gpioControl = new GPIOControl();
        private GPIOControl gpioControlX = new GPIOControl();
        private GPIOControl gpioControlY = new GPIOControl();
        private GPIOControl gpioControlZ = new GPIOControl();

        private double speed; // millimeter per second
        private int[] steps = { 80, 80, 400 }; // Steps per mm (default 80, 80, 400)
        private double[] stepPerSecond = new double[3];
        private bool done = false;

        private int[] stepsToDo = new int[3];
        private int[] stepsDone = new int[] { 0, 0, 0 };
        private bool[] dir = new bool[] { POSITIVE, POSITIVE, POSITIVE}; // All increasing distance compared to 0

        private double[] ratio = new double[3];

        System.Timers.Timer TimerX = new System.Timers.Timer(1);
        System.Timers.Timer TimerY = new System.Timers.Timer(1);
        System.Timers.Timer TimerZ = new System.Timers.Timer(1);

        // Speed: mm/s, Steps: steps/mm
        public AxisControl(double Speed, int[]? Steps)
        {
            this.speed = Speed;
            if (Steps != null && Steps.Length == 3)
            {
                this.steps = Steps;

            }
            // mm/s and steps/mm to steps/s
            // 1/2 *    3/1 =       3/2
            for (int i = 0; i < steps.Length; i++)
            {
                if (speed * steps[i] <= 1000)
                {
                    this.stepPerSecond[i] = speed * steps[i];
                }
                else
                {
                    this.stepPerSecond[i] = 1000;
                }
            }

        }

        public bool Move(Coordinate coordinate)
        {
            double[] moveLocation = new double[3];
            moveLocation[0] = coordinate.X;
            moveLocation[1] = coordinate.Y;
            moveLocation[2] = coordinate.Z;

            this.done = false;
            stepsDone = new int[] { 0, 0, 0 };
            for (int i = 0; i < moveLocation.Length; i++)
            {
                if (moveLocation[i] < 0)
                {
                    dir[i] = NEGATIVE;
                    stepsToDo[i] = (int)(moveLocation[i] * steps[i] * -1);
                } else
                {
                    stepsToDo[i] = (int)(moveLocation[i] * steps[i]);
                }
            }
            Console.WriteLine("Steps to do: " + stepsToDo[0] + " " + stepsToDo[1] + " " + stepsToDo[2]);

            try
            {
                getRatio(new double[] { coordinate.X, coordinate.Y, coordinate.Z });
                Console.WriteLine("Ratio: " + ratio[0] + " " + ratio[1] + " " + ratio[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            try
            {
                double[] isrTimes = isrTime();
                if (coordinate.X != 0)
                {
                    TimerX = new System.Timers.Timer(isrTimes[0]);
                    TimerX.Enabled = true;
                    TimerX.Elapsed += TimerX_Elapsed;
                }
                if (coordinate.Y != 0)
                {
                    TimerY = new System.Timers.Timer(isrTimes[1]);
                    TimerY.Enabled = true;
                    TimerY.Elapsed += TimerY_Elapsed;
                }
                if (coordinate.Z != 0)
                {
                    TimerZ = new System.Timers.Timer(isrTimes[2]);
                    TimerZ.Enabled = true;
                    TimerZ.Elapsed += TimerZ_Elapsed;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }


            while (!done)
            {
                if (!(TimerX.Enabled || TimerY.Enabled || TimerZ.Enabled))
                {
                    done = true;
                }
            }
            return true;
        }

        private void TimerZ_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Z"); // Debug
            gpioControlZ.ControlStep(dir[2], GPIOControl.StepperAxis.Z);
            stepsDone[2]++;
            if (stepsDone[2] >= stepsToDo[2])
            {
                TimerZ.Stop();
                TimerZ.Dispose();
            }
        }

        private void TimerY_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Y");
            gpioControlY.ControlStep(dir[1], GPIOControl.StepperAxis.Y);
            stepsDone[1]++;
            if (stepsDone[1] >= stepsToDo[1])
            {
                TimerY.Stop();
                TimerY.Dispose();
            }
        }

        private void TimerX_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Console.WriteLine("X");
            gpioControlX.ControlStep(dir[0], GPIOControl.StepperAxis.X);
            stepsDone[0]++;
            if (stepsDone[0] >= stepsToDo[0])
            {
                TimerX.Stop();
                TimerX.Dispose();
            }
        }

        private double[] getRatio(double[] Coordinates)
        {
            switch (max(Coordinates))
            {
                case 0: // X is biggest, all other ratios <= 1
                    ratio[0] = 1;
                    ratio[1] = Coordinates[1] / Coordinates[0];
                    ratio[2] = Coordinates[2] / Coordinates[0];
                    break;
                case 1: // Y is biggest, all other ratios <= 1
                    ratio[1] = 1;
                    ratio[0] = Coordinates[0] / Coordinates[1];
                    ratio[2] = Coordinates[2] / Coordinates[1];
                    break;
                case 2: // Z is biggest, all other ratios <= 1
                    ratio[2] = 1;
                    ratio[1] = Coordinates[1] / Coordinates[2];
                    ratio[0] = Coordinates[0] / Coordinates[2];
                    break;
            }
            return ratio;
        }

        private int max(double[] Coordinates)
        {
            int max = 0;
            for (int i = 1; i < Coordinates.Length; i++)
            {
                if (Coordinates[i] > Coordinates[max])
                {
                    max = i;
                }
            }
            return max;
        }

        private double[] isrTime()
        {
            /*
            steps/s * ratio = steps/s[axis]
            e.g.:
                steps/s = 1000
                ratio = {1, 0.5, 0.01}

                X = 1000 steps/s
                Y = 500 steps/s
                Z = 10 steps/s
            
                X timer = 1000(ms) / 1000(steps/s) = 1ms delay
                Y timer = 1000(ms) / 5000(steps/s) = 2ms delay
                Z timer = 1000(ms) / 10(steps/s) = 100ms delay

             Timer[axis](1000 / steps/s[axis])
             Timer[axis].Elapse += axisElapse -> step;
             */
            double[] isrTimes = new double[3];


            isrTimes[0] = (1000 / (stepPerSecond[0] * ratio[0]));
            isrTimes[1] = (1000 / (stepPerSecond[1] * ratio[1]));
            isrTimes[2] = (1000 / (stepPerSecond[2] * ratio[2]));

            return isrTimes;
        }

        public bool ReadLimitSwitch(GPIOControl.LimitSwitch limitSwitch) {
            return gpioControl.ReadLimitSwitch(limitSwitch);
        }

        public long UsDelay(int microseconds, long StartTick)
        {
            return gpioControl.UsDelay(microseconds, StartTick);
        }

        public void DisableStepper()
        {
            gpioControl.DisableSteppers();
        }

        public bool[] ReadPin(int pin)
        {
            if(pin < 5)
            {
                return new[] {true, gpioControl.ReadPin(pin, true)};
            }
            return new[] {false, false};
        }

        public void EmergencyStop()
        {
            gpioControl.EmergencyStop();
        }

        public bool SetPin(int pin, bool state)
        {
            if(pin < 5)
            {
                gpioControl.SetPin(pin, state);
            } else
            {
                return false;
            }
            return true;
        }
    }
}
