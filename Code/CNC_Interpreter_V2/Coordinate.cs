using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Coordinate
    {
        private double x;
        private double y;
        private double z;
        private bool spindel;

        public Coordinate(double X, double Y, double Z, bool Spindel)
        {
            this.x = X;
            this.y = Y;
            this.z = Z;
            this.spindel = Spindel;
        }

        public double X { get { return x; } }
        public double Y { get { return y; } }
        public double Z { get { return z; } }
        public bool Spindel { get {  return spindel; } }
    }
}
