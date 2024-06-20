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

        public double this[int index]
        {
            get
            {
                return index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }

        public double X { get { return x; } set { if (!Double.IsNaN(value)) { this.x = value; }; } }
        public double Y { get { return y; } set { if (!Double.IsNaN(value)) { this.y = value; }; } }
        public double Z { get { return z; } set { if (!Double.IsNaN(value)) { this.z = value; }; } }
        public bool Spindel { get { return spindel; } }

        public void Print()
        {
            Console.WriteLine("X: " + this.x);
            Console.WriteLine("Y: " + this.y);
            Console.WriteLine("Z: " + this.z);
            Console.WriteLine("S: " + this.spindel);
        }
    }
}
