using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CNC_Interpreter_V2
{
    internal class Parser
    {
        private Calculator _calculator = new Calculator(0.005);
        private string lastCommand = string.Empty;
        public List<Coordinate> Parse(double[] current, Value Input)
        {
            List<Coordinate> result = new List<Coordinate>();
            switch (Input.Command)
            {
                case "G0": // Linear move, spindel off
                    lastCommand = Input.Command;
                    if (Input.X != -0.0 || Input.Y != -0.0 || Input.Z != -0.0)
                    {
                        result.Add(new Coordinate(Input.X, Input.Y, Input.Z, false));
                    }
                    break;
                case "G1": // Linear move, spindel on
                    lastCommand = Input.Command;
                    if (Input.X != -0.0 || Input.Y != -0.0 || Input.Z != -0.0)
                    {
                        result.Add(new Coordinate(Input.X, Input.Y, Input.Z, true));
                    }
                    break;
                case "G2": // Clockwise arc
                case "G3": // CounterClockwise arc
                    double[] begin = new[] { 0.0, 0.0 };
                    double[] end = new double[2];
                    double[]? offset = null;
                    double? radius = null;
                    bool direction = Input.Command == "G2"; // if true CW, otherwise CCW

                    if (Input.X == -0.0 && Input.Y == -0.0)
                    {
                        end = new[] { 0.0, 0.0 };
                    }
                    else
                    {
                        end = new[] { Input.X, Input.Y };
                    }
                    if (Input.I != -0.0 && Input.J != -0.0)
                    {
                        offset = new[] { Input.I, Input.J };
                    }
                    if (Input.R != -0.0)
                    {
                        radius = Input.R;
                    }

                    try
                    {
                        return _calculator.Arc(begin, end, offset, radius, direction, Input.Workplane);
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e);
                        Debug.WriteLine(e);
                        break;
                    }
                case "G5":
                    return _calculator.Bezier(current[0], current[1], Input);
                default:
                    break;
            }
            return result;

        }
    }
}
