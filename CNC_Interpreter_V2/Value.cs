using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Value
    {
        private string command;
        private double x;
        private double y;
        private double z;
        private double i;
        private double j;
        private double p;
        private double q;
        private double r;
        private bool s; // Spindel On/Off
        private Settings.workplanes workplane;
        private bool mm;

        public string Command {  get { return command; } set { if (value.GetType() == typeof(string)) { command = value; } } }
        public double X { get { return x; } set {  if(!Double.IsNaN(value)) { x = value; }; } }
        public double Y { get { return y; } set { if(!Double.IsNaN(value)) {  y = value; }; } }
        public double Z { get { return z; } set { if(!Double.IsNaN(value)) {  z = value; }; } }
        public double I { get { return i; } set { if(!Double.IsNaN(value)) {  i = value; }; } }
        public double J { get { return j; } set { if(!Double.IsNaN(value)) {  j = value; }; } }
        public double P { get { return p; } set { if(!Double.IsNaN(value)) {  p = value; }; } }
        public double Q { get { return q; } set { if(!Double.IsNaN(value)) {  q = value; }; } }
        public double R { get { return r; } set { if(!Double.IsNaN(value)) { r = value;  }; } }
        public bool S { get { return s; } set { if (value.GetType() == typeof(bool)) {  s = value; }; } }
        public Settings.workplanes Workplane { get { return workplane; } set { if (value.GetType() == typeof(Settings.workplanes)){ workplane = value; } } }
        public bool MM { get { return MM; } set {  MM = value; } }
    }
}
