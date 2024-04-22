using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Parser
    {
        public List<Coordinate> Parse(Value Input)
        {
            switch (Input.Command)
            {
                case "G0":
                    break;
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
            return new List<Coordinate>();
        }
    }
}
