using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Calculator
    {
        private double accuracy;
        public Calculator(double Accuracy) {
            accuracy = Accuracy;
        }
        public List<Coordinate> Bezier(double CurrentX, double CurrentY, Value CurveValues) {
            // Xstart, Ystart, P, Q, Xend, Yend must be set
            // I, J optional
            // Z must be 0 (not set)


            double[] point1 = new double[2];
            // Check for values
            if (!(CurveValues.I == 0.0) && !(CurveValues.J == 0.0))
            {
                point1[0] = CurveValues.I - CurrentX;
                point1[1] = CurveValues.J - CurrentY;
            } else
            {
                point1[0] = CurveValues.X - CurrentX;
                point1[1] = CurveValues.Y - CurrentY;
            }
            if (CurveValues.P == 0.0 || CurveValues.Q == 0.0)
            {
                return new List<Coordinate>();
            }
            if (CurveValues.X == 0.0 || CurveValues.Y == 0.0)
            {
                return new List<Coordinate>();
            }
            List<Coordinate> result = new List<Coordinate>();
            double previousX = 0, previousY = 0;
            bool Spindel = bool.Parse(CurveValues.S.ToString());
            double[] origin = {CurrentX, CurrentY};
            double[] point2 = { CurveValues.P-CurrentX, CurveValues.Q-CurrentY };
            double[] destination = { CurveValues.X-CurrentX,  CurveValues.Y-CurrentY};
            for (double t = 0; t <= 1; t += 0.001)
            {
                double x = (1 - t) * ((1 - t) * ((1 - t) * origin[0] + t * point1[0]) + t * ((1 - t) * point1[0] + t * point2[0])) + t * ((1 - t) * ((1 - t) * point1[0] + t * point2[0]) + t * ((1 - t) * point2[0] + t * destination[0]));
                double y = (1 - t) * ((1 - t) * ((1 - t) * origin[1] + t * point1[1]) + t * ((1 - t) * point1[1] + t * point2[1])) + t * ((1 - t) * ((1 - t) * point1[1] + t * point2[1]) + t * ((1 - t) * point2[1] + t * destination[1]));

                if (linearDistance(previousX, x, previousY, y) >= accuracy)
                {
                    Coordinate move = new Coordinate(x, y, CurveValues.Z , Spindel);
                    previousX = x;
                    previousY = y;
                    Debug.WriteLine("(" + x + ", " + y + "),");
                    result.Add(newCoordinate(move, CurveValues.Workplane));
                }
            }
            return result;
        }


        private double linearDistance(double x1, double x2, double y1, double y2)
        {
            double deltaX = x2 - x1;
            double deltaY = y2 - y1;
            double distance = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
            return distance;
        }

        private Coordinate newCoordinate(Coordinate Coordinate, Settings.Workplanes Workplane)
        {
            switch (Workplane)
            {
                case Settings.Workplanes.XY: // (X, Y) -> (X, Y)
                    return new Coordinate(Coordinate.X, Coordinate.Y, Coordinate.Z, Coordinate.Spindel);
                case Settings.Workplanes.YZ: // (X, Y) -> (Y, Z)
                    return new Coordinate(Coordinate.Z, Coordinate.X, Coordinate.Y, Coordinate.Spindel);
                case Settings.Workplanes.ZX: // (X, Y) -> (Z, X)
                    return new Coordinate(Coordinate.Y, Coordinate.Z, Coordinate.X, Coordinate.Spindel);
                default:
                    return new Coordinate(0,0,0,false);
            }
        }
    }
}
