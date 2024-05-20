using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnitsNet;

namespace CNC_Interpreter_V2
{
    internal class AxisControl
    {
        private GPIOControl gpioControl = new GPIOControl();
        private double speed; // millimeter per second
        private int steps = 1280; // Steps per mm (default 1280 (80*16))
        private double stepPerSecond;
        private bool done = false;

        private int[] stepsToDo = new int[3];
        private int[] stepsDone = new int[] {0, 0, 0};

        private double[] ratio = new double[3];

        System.Timers.Timer TimerX = new System.Timers.Timer(1);
        System.Timers.Timer TimerY = new System.Timers.Timer(1);
        System.Timers.Timer TimerZ = new System.Timers.Timer(1);

        AxisControl(double Speed, int Steps)
        {
            this.speed = Speed;
            this.steps = Steps;
            // mm/s and steps/mm to steps/s
            // 1/2 *    3/1 =       3/2
            this.stepPerSecond = speed * steps;
        }

        public bool Move(double x, double y, double z)
        {
            this.done = false;
            stepsDone = new int[] {0,0,0};
            stepsToDo = new int[]
            {
                (int)(x * steps),
                (int)(y * steps),
                (int)(z * steps)
            };


            try
            {
                getRatio(new double[] { x, y, z });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

            try
            {
                double[] isrTimes = isrTime();
                TimerX = new System.Timers.Timer(isrTimes[0]);
                TimerY = new System.Timers.Timer(isrTimes[1]);
                TimerZ = new System.Timers.Timer(isrTimes[2]);

                TimerX.Enabled = true;
                TimerY.Enabled = true;
                TimerZ.Enabled = true;

                TimerX.Elapsed += TimerX_Elapsed;
                TimerY.Elapsed += TimerY_Elapsed;
                TimerZ.Elapsed += TimerZ_Elapsed;
            } catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }


            while (!done)
            {
                if(!(TimerX.Enabled || TimerY.Enabled || TimerZ.Enabled))
                {
                    done = true;
                }
            }
            return true;
        }

        private void TimerZ_Elapsed(object? sender, ElapsedEventArgs e)
        {
            stepsDone[0]++;
            if (stepsDone[0] >= stepsToDo[0])
            {
                TimerX.Stop();
                TimerX.Dispose();
            }
        }

        private void TimerY_Elapsed(object? sender, ElapsedEventArgs e)
        {
            stepsDone[1]++;
            if (stepsDone[1] >= stepsToDo[1])
            {
                TimerY.Stop();
                TimerY.Dispose();
            }
        }

        private void TimerX_Elapsed(object? sender, ElapsedEventArgs e)
        {
            // GPIO Step functie??


            stepsDone[2]++;
            if (stepsDone[2] >= stepsToDo[2])
            {
                TimerZ.Stop();
                TimerZ.Dispose();
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
            for(int i = 1; i < Coordinates.Length; i++)
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
                steps/s = 1280
                ratio = {1, 0.78.., 0.0078..}

                X = 1280 steps/s
                Y = 1000 steps/s
                Z = 10 steps/s
            
                X timer = 1000(ms) / 1280(steps/s) = 0,78..ms delay
                Y timer = 1000(ms) / 1000(steps/s) = 1ms delay
                Z timer = 1000(ms) / 10(steps/s) = 100ms delay

             Timer[axis](1000 / steps/s[axis])
             Timer[axis].Elapse += axisElapse -> step;
             */
            double[] isrTimes = new double[3];

            isrTimes[0] = (1000 / (stepPerSecond * ratio[0]));
            isrTimes[1] = (1000 / (stepPerSecond * ratio[1]));
            isrTimes[2] = (1000 / (stepPerSecond * ratio[2]));

            return isrTimes;
        }
    }
}
