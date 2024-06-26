﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Value
    {
        private string command = "";
        private double x = -0.0;
        private double y = -0.0;
        private double z = -0.0;
        private double i = -0.0;
        private double j = -0.0;
        private double p = -0.0;
        private double q = -0.0;
        private double r = -0.0;
        private double s = -0.0; // Spindel
        private Settings.Workplanes workplane;
        private bool mm;
        private string? openText;

        public string Command { get { return command; } set { if (value.GetType() == typeof(string)) { command = value; } } }
        public double X { get { return x; } set { if (!Double.IsNaN(value)) { x = value; }; } }
        public double Y { get { return y; } set { if (!Double.IsNaN(value)) { y = value; }; } }
        public double Z { get { return z; } set { if (!Double.IsNaN(value)) { z = value; }; } }
        public double I { get { return i; } set { if (!Double.IsNaN(value)) { i = value; }; } }
        public double J { get { return j; } set { if (!Double.IsNaN(value)) { j = value; }; } }
        public double P { get { return p; } set { if (!Double.IsNaN(value)) { p = value; }; } }
        public double Q { get { return q; } set { if (!Double.IsNaN(value)) { q = value; }; } }
        public double R { get { return r; } set { if (!Double.IsNaN(value)) { r = value; }; } }
        public double S { get { return s; } set { if (value.GetType() == typeof(double)) { s = value; }; } }
        public Settings.Workplanes Workplane { get { return workplane; } set { if (value.GetType() == typeof(Settings.Workplanes)) { workplane = value; } } }
        public bool MM { get { return MM; } set { mm = value; } }
        public string? OpenText { get { return openText; } set { openText = value; } }

        public void Print()
        {
            Debug.WriteLine("Command: " + command);
            Debug.WriteLine("X: " + x);
            Debug.WriteLine("Y: " + y);
            Debug.WriteLine("Z: " + z);
            Debug.WriteLine("I: " + i);
            Debug.WriteLine("J: " + j);
            Debug.WriteLine("P: " + p);
            Debug.WriteLine("Q: " + q);
            Debug.WriteLine("R: " + r);
            Debug.WriteLine("S: " + s);
            Debug.WriteLine("Workplane: " + workplane);
            Debug.WriteLine("mm: " + mm);
        }
    }
}
