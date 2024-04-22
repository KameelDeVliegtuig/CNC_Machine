﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Settings
    {
        public enum workplanes
        {
            XY,
            ZX,
            YZ
        }

        private double x;
        private double y;
        private double z;
        private workplanes workplane;
        private double mmps = 2.0; // mm/s
        private double[] stepsPerMM = { 200, 200, 200 };
        private double[] mmpr = {80.0, 80.0, 400.0}; // mm/rotation [x,y,z]
        private bool mm = true; // Units default to mm
        private bool spindel = false; // Spindel default off
        private bool stepperEnable = false;
        // CurrentFile string??
        private int lineCounter;
        private bool locked;
        private int passcode;
        private short logging;
        private bool emergencyStop;
        private double[] spindelToProbe = { 1.0, 1.0, 0.0};

        public double X { get { return x; } }
        public double Y { get { return y; } }
        public double Z { get { return z; } }
        public workplanes Workplane { get { return workplane; } set { if (Enum.IsDefined(value)) { workplane = value; } } }
        public double MMpS { get { return mmps; } }
        public double[] StepsPerMM { get { return stepsPerMM; } set { if (value.Length == 3) { stepsPerMM = value; } } }
        public double[] MMpR { get { return mmpr; } }
        public bool MM {  get { return mm; } set { MM = value; } }
        public bool Spindel {  get { return spindel; }  set { if (value.GetType() == typeof(bool) && !emergencyStop) { spindel = value; } } }
        public bool StepperEnable { get { return stepperEnable; } set { if (value.GetType() == typeof(bool) && !emergencyStop) {  stepperEnable = value; } } }
        public int LineCounter { get { return lineCounter; } }
        public bool Locked { get { return locked; } }
        public short Logging { get { return logging; } set { if (value.GetType() == typeof(short)) { logging = value; } } }
        public bool EmergencyStop { get { return emergencyStop; } set { emergencyStop = value; } }
        public double[] SpindelToProbe {  get { return spindelToProbe; } }

        public Settings(double X, double Y, double Z)
        {
            this.x = X;
            this.y = Y;
            this.z = Z;
        }

        public void Lock(int Passcode)
        {
            if (!locked)
            {
                passcode = Passcode;
                locked = true;
            }
        }

        public bool Unlock(int Passcode)
        {
            if (passcode == Passcode)
            {
                locked = false;
                return true;
            }
            else return false;
        }

        public void SetOffset(double[] Offset)
        {
            if(Offset.Length != 3)
            {
                return;
            }
            for(int i = 0; i < Offset.Length; i++)
            {
                if (Double.IsNaN(Offset[i])) return;
            }
            spindelToProbe = Offset;
        }

    }
}
