using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Parser
    {
        public List<Coordinate> Parse(double[] current, Value Input)
        {
            List<Coordinate> result = new List<Coordinate>();
            switch (Input.Command)
            {
                case "G0":
                    break;
                    result.Append(new Coordinate(current[0] + Input.X, current[1] + Input.Y, current[2] + Input.Z, false));
                case "G1":
                    break;
                case "G2":
                    break;
                case "G3":
                    break;
                case "G4":
                    break;
                case "G5":
                    break;
                case "G6":
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
