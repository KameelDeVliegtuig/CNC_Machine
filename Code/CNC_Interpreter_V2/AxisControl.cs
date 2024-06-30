using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        private double speed; // millimeter per second
        private int[] steps = { 80, 80, 400 }; // Steps per mm (default 80, 80, 400)
        private double[] stepPerSecond = new double[3];
        private bool[] done = { false, false, false };

        private int[] stepsToDo = new int[3];
        private int[] stepsDone = new int[] { 0, 0, 0 };
        private bool[] dir = new bool[] { POSITIVE, POSITIVE, POSITIVE }; // All increasing distance compared to 0

        private double[] ratio = new double[3];

        private int SpindelSpeed = 100; // 100%

        System.Timers.Timer TimerX = new System.Timers.Timer();
        System.Timers.Timer TimerY = new System.Timers.Timer();
        System.Timers.Timer TimerZ = new System.Timers.Timer();

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
                if (speed * steps[i] <= 1000 || (i == 2 && speed * steps[i] <= 5000))
                {
                    this.stepPerSecond[i] = speed * steps[i];
                }
                else
                {
                    if (i == 2)
                    {
                        this.stepPerSecond[i] = 5000;
                    }
                    else
                    {
                        this.stepPerSecond[i] = 1000;
                    }
                }
            }

        }

        public bool Move(Coordinate coordinate)
        {
            double[] moveLocation = new double[3];
            moveLocation[0] = coordinate.X;
            moveLocation[1] = coordinate.Y;
            moveLocation[2] = coordinate.Z;

            this.done = new[] { false, false, false };
            stepsDone = new int[] { 0, 0, 0 };
            for (int i = 0; i < moveLocation.Length; i++)
            {
                if (moveLocation[i] < 0)
                {
                    dir[i] = NEGATIVE;
                    stepsToDo[i] = (int)(moveLocation[i] * steps[i] * -1);
                    moveLocation[i] = moveLocation[i] * -1;
                }
                else
                {
                    dir[i] = POSITIVE;
                    stepsToDo[i] = (int)(moveLocation[i] * steps[i]);
                }
            }
            Console.WriteLine("Move Location: " + moveLocation[0] + " " + moveLocation[1] + " " + moveLocation[2]);
            Console.WriteLine("Steps to do: " + stepsToDo[0] + " " + stepsToDo[1] + " " + stepsToDo[2]);

            try
            {
                getRatio(moveLocation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            try
            {
                TimeSpan[] isrTimes = isrTime();
                //Console.WriteLine("ISR Times: " + isrTimes[0] + " " + isrTimes[1] + " " + isrTimes[2]);

                gpioControl.ControlSpindel(coordinate.Spindel ? SpindelSpeed : 0, true);

                long[] timeStamp = { Stopwatch.GetTimestamp(), Stopwatch.GetTimestamp(), Stopwatch.GetTimestamp() };
                bool[] toggleStage = { false, false, false };
                while (!done[0] || !done[1] || !done[2])
                {
                    TimeSpan[] ElapsedTime = { Stopwatch.GetElapsedTime(timeStamp[0]), Stopwatch.GetElapsedTime(timeStamp[1]), Stopwatch.GetElapsedTime(timeStamp[2]) };

                    for (int i = 0; i < 3; i++)
                    {
                        if ((coordinate[i] != 0 || coordinate[i] != -0) && done[i] == false)
                        {
                            if (ElapsedTime[i] > isrTimes[i])
                            {
                                timeStamp[i] = Stopwatch.GetTimestamp();
                                if (gpioControl.ToggleStep(dir[i], (StepperAxis)i) && toggleStage[i]) stepsDone[i]++;
                                toggleStage[i] = !toggleStage[i];

                                if (stepsDone[i] >= stepsToDo[i])
                                {
                                    done[i] = true;
                                }
                            }
                        }
                        else
                        {
                            done[i] = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
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

        private TimeSpan[] isrTime()
        {
            TimeSpan[] isrTimes = new TimeSpan[3];
            double[] times = new double[3];
            if (ratio[0] > 0)
            {
                isrTimes[0] = TimeSpan.FromMicroseconds((500 / (stepPerSecond[0] * ratio[0])) * 200);
            }
            if (ratio[1] > 0)
            {
                isrTimes[1] = TimeSpan.FromMicroseconds((500 / (stepPerSecond[1] * ratio[1])) * 200);
            }
            if (ratio[2] > 0)
            {
                isrTimes[2] = TimeSpan.FromMicroseconds((100 / (stepPerSecond[2] * ratio[2])) * 125); // Aangepaste tijd voor Z-as
            }
            
            
            
            return isrTimes;
        }


        public bool ReadLimitSwitch(GPIOControl.LimitSwitch limitSwitch)
        {
            return gpioControl.ReadLimitSwitch(limitSwitch);
        }

        public static void UsDelay(int microseconds, long StartTick)
        {
            while (Stopwatch.GetElapsedTime(StartTick).Microseconds < microseconds) continue;
        }

        public void DisableStepper()
        { 
            gpioControl.DisableSteppers();
        }

        public bool[] ReadPin(int pin)
        {
            if (pin < 5)
            {
                return new[] { true, gpioControl.ReadPin(pin, true) };
            }
            return new[] { false, false };
        }

        public void EmergencyStop()
        {
            gpioControl.EmergencyStop();
        }

        public bool SetPin(int pin, bool state)
        {
            if (pin < 5)
            {
                gpioControl.SetPin(pin, state);
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool ControlStep(bool dir, StepperAxis steppers)
        {
            return gpioControl.ControlStep(dir, steppers);
        }


    }
}
