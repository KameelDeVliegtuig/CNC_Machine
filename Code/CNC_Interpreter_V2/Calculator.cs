using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Calculator
    {
        private double accuracy;
        public Calculator(double Accuracy)
        {
            accuracy = Accuracy;
        }
        public List<Coordinate> Bezier(double CurrentX, double CurrentY, Value CurveValues)
        {
            // Xstart, Ystart, P, Q, Xend, Yend must be set
            // I, J optional
            // Z must be 0 (not set)


            double[] point1 = new double[2];
            // Check for values
            if (!(CurveValues.I == 0.0) && !(CurveValues.J == 0.0))
            {
                point1[0] = CurveValues.I - CurrentX;
                point1[1] = CurveValues.J - CurrentY;
            }
            else
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
            double[] origin = { CurrentX, CurrentY };
            double[] point2 = { CurveValues.P - CurrentX, CurveValues.Q - CurrentY };
            double[] destination = { CurveValues.X - CurrentX, CurveValues.Y - CurrentY };
            for (double t = 0; t <= 1; t += 0.001)
            {
                double x = (1 - t) * ((1 - t) * ((1 - t) * origin[0] + t * point1[0]) + t * ((1 - t) * point1[0] + t * point2[0])) + t * ((1 - t) * ((1 - t) * point1[0] + t * point2[0]) + t * ((1 - t) * point2[0] + t * destination[0]));
                double y = (1 - t) * ((1 - t) * ((1 - t) * origin[1] + t * point1[1]) + t * ((1 - t) * point1[1] + t * point2[1])) + t * ((1 - t) * ((1 - t) * point1[1] + t * point2[1]) + t * ((1 - t) * point2[1] + t * destination[1]));

                if (linearDistance(previousX, x, previousY, y) >= accuracy)
                {
                    previousX = x;
                    previousY = y;
                    Debug.WriteLine("(" + x + ", " + y + "),");
                    result.Add(newCoordinate(move, CurveValues.Workplane));
                }
            }
            return result;
        }

        public List<Coordinate> Arc(double[] start, double[] end, double[]? offset, double? radius, bool clockwise, Settings.Workplanes workplane)
        {
            bool checkStart = (start[0] == -0.0 && start[1] == -0.0); // True if not correct
            bool checkEnd = (end[0] == -0.0 && end[1] == -0.0); // True if not correct

            if (checkStart || checkEnd || ((offset == null || offset[0] == -0.0 || offset[1] == -0.0) && (radius == null || radius == -0.0)))
            {
                Console.WriteLine("Arc: Not enough arguments");
            }

            double r;
            int clockwiseCorrector = 1;

            double mx = 0, my = 0;
            double previousX = 0, previousY = 0;

            double minX, minY, maxX, maxY;
            double dAB = linearDistance(start[0], end[0], start[1], end[1]);

            // Set range for arc
            if (start[0] > end[0])
            {
                minX = end[0];
                maxX = start[0];
            }
            else
            {
                minX = start[0];
                maxX = end[0];
            }

            if (start[1] > end[1])
            {
                minY = end[1];
                maxY = start[1];
            }
            else
            {
                minY = start[1];
                maxY = end[1];
            }


            if (!clockwise)
            {
                clockwiseCorrector = -1;
            }

            // Middlepoint
            if (offset != null)
            {
                mx = (start[0] + offset[0]);
                my = (start[1] + offset[1]);
            }
            else if (radius != null)
            {

                double midX = (start[0] + end[0]) / 2;
                double midY = (start[1] + end[1]) / 2;

                // Calculate the distance from the midpoint to the center of the circle
                double d = Math.Sqrt((double)radius * (double)radius - (dAB / 2) * (dAB / 2));

                // Calculate the slopes of the line segments connecting the points and the center of the circle
                double m1 = (end[1] - start[1]) / (end[0] - start[0]);
                double m2 = -1 / m1;

                // Calculate the x and y coordinates of the center of the circle
                mx = midX + (d * clockwiseCorrector) / Math.Sqrt(1 + m2 * m2);
                my = midY + m2 * (mx - midX);
            }
            else
            {
                Console.WriteLine("Arc: Both offset and radius are null");
            }

            // Radius
            if (radius == null)
            {
                r = linearDistance(mx, start[0], my, start[1]);
            }
            else
            {
                r = (double)radius;
            }

            if (dAB > 2 * r)
            {
                throw new Exception("Distance too big!");
            }

            // Coordinates
            List<Coordinate> coordinates = new List<Coordinate>();
            for (double i = 0; i <= (2 * Math.PI); i += 0.001)
            {
                double x = (r * Math.Cos(i * clockwiseCorrector)) + mx;// + (start[0] + end[0])/2;
                double y = (r * Math.Sin(i * clockwiseCorrector)) + my;// + (start[1] + end[1])/2;

                bool ifCW = (x >= minX && x <= maxX && y <= maxY && my >= maxY) || (x >= minX && x <= maxX && y >= minY && my <= maxY);
                bool ifCCW = (x <= maxX && x >= minX && y >= minY && maxY >= my) || (x <= maxX && x >= minX && y <= maxY && maxY <= my);

                if ((clockwise && ifCW) || (!clockwise && ifCCW))
                {
                    if (linearDistance(previousX, x, previousY, y) >= accuracy)
                    {
                        previousX = x;
                        previousY = y;
                        Coordinate newcoordinate = new Coordinate(x, y, 0, true);
                        coordinates.Add(newCoordinate(newcoordinate, workplane));
                        //Debug.WriteLine("(" + x + ", " + y + "),");
                    }
                }


            }
            //Debug.WriteLine("Radius: " + r + ", M(" + mx + "," + my + ")");

            return coordinates;
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
                    return new Coordinate(0, 0, 0, false);
            }
        }
    }
}
