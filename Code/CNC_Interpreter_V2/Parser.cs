using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CNC_Interpreter_V2
{
    internal class Parser
    {
        private Calculator _calculator = new Calculator(0.005);
        public List<Coordinate> Parse(double[] current, Value Input)
        {
            List<Coordinate> result = new List<Coordinate>();
            switch (Input.Command)
            {
                case "G0": // Linear move, spindel off
                    Console.WriteLine("G0 Linear Move Without Spindel");
                    Coordinate G0 = new Coordinate(Input.X, Input.Y, Input.Z, false);
                    G0.Print();
                    result.Append(G0);
                    return result;
                case "G1": // Linear move, spindel on
                    Console.WriteLine("G1 Linear Move With Spindel");
                    result.Append(new Coordinate(Input.X, Input.Y, Input.Z, true));
                    break;
                case "G2":
                    break;
                case "G3":
                    break;
                case "G4":
                    break;
                case "G5":
                    return _calculator.Bezier(current[0], current[1], Input);
                case "G6":
                    break;
                default:
                    break;
            }
            Console.WriteLine("Parser Moves:");
            for (int i = 0; i < result.Count; i++)
            {
                Console.WriteLine(i + ": " + result[i]);
            }
            return result;

        }
    }
}
