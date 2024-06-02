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
        Delay CommonTimer = new Delay();

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
                Console.WriteLine("ISR Times: " + isrTimes[0] + " " + isrTimes[1] + " " + isrTimes[2]);

                // Spindel on or off depending on given variable
                gpioControl.ControlSpindel(coordinate.Spindel ? SpindelSpeed : 0, true);

                // Move axis
                long[] timeStamp = { Stopwatch.GetTimestamp(), Stopwatch.GetTimestamp(), Stopwatch.GetTimestamp() };
                while (done != new[] { true, true, true })
                {
                    TimeSpan[] ElapsedTime = { Stopwatch.GetElapsedTime(timeStamp[0]), Stopwatch.GetElapsedTime(timeStamp[1]), Stopwatch.GetElapsedTime(timeStamp[2]) };
                    if (coordinate.X != 0 && done[0] == false)
                    {
                        if (ElapsedTime[0].Microseconds % isrTimes[0] != ElapsedTime[0].Microseconds)
                        {
                            timeStamp[0] = Stopwatch.GetTimestamp();
                            Console.WriteLine("X");

                            if(gpioControl.ControlStep(dir[0], StepperAxis.X)) stepsDone[0]++;

                            if (stepsDone[0] >= stepsToDo[0])
                            {
                                done[0] = true;
                            }
                        }
                    }

                    if (coordinate.Y != 0 && done[1] == false)
                    {
                        if (ElapsedTime[1].Microseconds % isrTimes[1] != ElapsedTime[1].Microseconds)
                        {
                            timeStamp[1] = Stopwatch.GetTimestamp();
                            Console.WriteLine("Y");

                            if (gpioControl.ControlStep(dir[1], StepperAxis.Y)) stepsDone[1]++;

                            if (stepsDone[1] >= stepsToDo[1])
                            {
                                done[1] = true;
                            }
                        }
                    }

                    if (coordinate.Z != 0 && done[2] == false)
                    {
                        if (ElapsedTime[2].Microseconds % isrTimes[2] != ElapsedTime[2].Microseconds)
                        {
                            timeStamp[2] = Stopwatch.GetTimestamp();
                            Console.WriteLine("Z");

                            if (gpioControl.ControlStep(dir[2], StepperAxis.Z)) stepsDone[2]++;

                            if (stepsDone[2] >= stepsToDo[2])
                            {
                                done[2] = true;
                            }
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
            
                X timer = 500.000(us) / 1000(steps/s) = 500us delay
                Y timer = 500.000(us) / 5000(steps/s) = 1000us delay
                Z timer = 500.000(us) / 10(steps/s) = 50000us delay

             Timer[axis](1000 / steps/s[axis])
             Timer[axis].Elapse += axisElapse -> step;
             */
            double[] isrTimes = new double[3];


            //isrTimes[0] = (500 / (stepPerSecond[0] * ratio[0]));
            //isrTimes[1] = (500 / (stepPerSecond[1] * ratio[1]));
            //isrTimes[2] = (500 / (stepPerSecond[2] * ratio[2]));
            isrTimes[0] = 1;
            isrTimes[1] = 1;
            isrTimes[2] = 1;

            return isrTimes;
        }

        public bool ReadLimitSwitch(GPIOControl.LimitSwitch limitSwitch)
        {
            return gpioControl.ReadLimitSwitch(limitSwitch);
        }

        public async Task UsDelay(int microseconds, long StartTick)
        {
            await CommonTimer.UsDelay(microseconds, StartTick);
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
    }
}
