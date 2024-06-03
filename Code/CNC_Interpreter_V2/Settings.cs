using System;
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

        private Coordinate NOZZLE_PARK_POINT = new Coordinate(0, 0, 150, false);
        private const int NOZZLE_PARK_Z_RAISE_MIN = 50;

        private double x;
        private double y;
        private double z;
        private workplanes workplane;
        private double mmps = 2.0; // mm/s
        private double[] stepsPerMM = { 200, 200, 200 };
        private double[] mmpr = {80.0, 80.0, 400.0}; // mm/rotation [x,y,z]
        private bool mm = true; // Units default to mm
        private bool spindel = false; // Spindel default off
        private bool spindelDir = true; // True = CW, False = CCW
        private bool stepperEnable = false;
        // CurrentFile string??
        private int lineCounter;
        private bool locked;
        private int passcode;
        private short logging;
        private bool emergencyStop;
        private double[] spindelToProbe = { 0.0, 20.0, 21.5};

        public double X { get { return x; } set { if (value.GetType() == typeof(double)) { x = value; } } }
        public double Y { get { return y; } set { if (value.GetType() == typeof(double)) { y = value; } } }
        public double Z { get { return z; } set { if (value.GetType() == typeof(double)) { z = value; } } }
        public workplanes Workplane { get { return workplane; } set { if (Enum.IsDefined(value)) { workplane = value; } } }
        public double MMpS { get { return mmps; } }
        public double[] StepsPerMM { get { return stepsPerMM; } set { if (value.Length == 3) { stepsPerMM = value; } } }
        public double[] MMpR { get { return mmpr; } }
        public bool MM {  get { return mm; } set { MM = value; } }
        public bool Spindel {  get { return spindel; }  set { if (value.GetType() == typeof(bool) && !emergencyStop) { spindel = value; } } }
        public bool SpindelDir { get { return spindelDir; } set { spindelDir = value; } }
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

        public Coordinate ParkTool(double P)
        {
            Coordinate parkLocation = new Coordinate(0,0,0,false);
            if(P == 0)
            {
                parkLocation = NOZZLE_PARK_POINT;
                parkLocation.Z = (double)NOZZLE_PARK_Z_RAISE_MIN;
            }
            if(P == 1)
            {
                parkLocation = NOZZLE_PARK_POINT;
            }
            if (P == 2)
            {
                parkLocation.Z = NOZZLE_PARK_POINT.Z;
            }
            if (P == 3)
            {
                parkLocation.Z = (double)NOZZLE_PARK_Z_RAISE_MIN;
            }
            if (P == 4)
            {
                parkLocation.X = NOZZLE_PARK_POINT.X;
                parkLocation.Y = NOZZLE_PARK_POINT.Y;
            }
            return parkLocation;
        }
    }
}
