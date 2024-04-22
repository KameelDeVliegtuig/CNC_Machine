using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNC_Interpreter_V2
{
    internal class Interpreter
    {
        Parser parser = new Parser();
        Settings settings = new Settings(0.0, 0.0, 0.0);
        GPIOControl gpio = new GPIOControl();

        private List<Coordinate> moves = new List<Coordinate>();
        private string lastCommand;

        public List<Coordinate> Moves { get { return moves; } }

        public void Interpret(string GCODE)
        {
            string[] splitted = GCODE.Split(' ');
            Value value = createValue(splitted);
            switch (value.Command)
            {
                // G Codes
                case "G0":
                case "G1":
                //Debug.WriteLine("Linear Move");
                case "G2":
                case "G3":
                //Debug.WriteLine("Arc or Circle Move");
                case "G4":
                //Debug.WriteLine("Dwell");
                case "G5":
                //Debug.WriteLine("Bézier cubic spline");
                case "G6":
                //Debug.WriteLine("Direct Stepper Move");
                    Debug.WriteLine("To Parser to calculate coordinates");
                    try
                    {
                        moves.AddRange(parser.Parse(value));
                    } catch(Exception e) { 
                        Debug.WriteLine(e);
                    }
                    break;

                // WorkPlane Select
                case "G17": // XY
                    Debug.WriteLine("CNC Workspace Planes");
                    settings.Workplane = (Settings.workplanes)0;
                    Debug.WriteLine("Workplane: " + settings.Workplane);
                    break;
                case "G18": // ZX
                    Debug.WriteLine("CNC Workspace Planes");
                    settings.Workplane = (Settings.workplanes)1;
                    Debug.WriteLine("Workplane: " + settings.Workplane);
                    break;
                case "G19": // YZ
                    Debug.WriteLine("CNC Workspace Planes");
                    settings.Workplane = (Settings.workplanes)2;
                    Debug.WriteLine("Workplane: " + settings.Workplane);
                    break;
                case "G20":
                    Debug.WriteLine("Units inch");
                    settings.MM = false;
                    break;
                case "G21":
                    Debug.WriteLine("Units mm");
                    settings.MM = true;
                    break;
                case "G26":
                    Debug.WriteLine("Validation pattern");
                    break;
                case "G27":
                    Debug.WriteLine("Park Toolhead");
                    break;
                case "G28":
                    Debug.WriteLine("Auto Home");
                    break;
                case "G29": // Multiple options
                    // Always G28 before G29
                    Debug.WriteLine("Bed Leveling");
                    Interpret("G28");
                    break;
                //case "G34": // Checks if Z-rods are at the same position
                //    Debug.WriteLine("Z-axis Gantry Calibration");
                //    break;
                case "G42":
                    Debug.WriteLine("Move to mesh coordinate");
                    break;
                case "G53":
                    Debug.WriteLine("Move in Machine coordinates (fast move)");
                    break;
                case "G54": // Slot 1
                case "G55": // Slot 2
                case "G56": // Slot 3
                case "G57": // Slot 4
                case "G58": // Slot 5
                case "G59": // Slot 6
                case "G59.1": // Slot 7
                case "G59.2": // Slot 8
                case "G59.3": // Slot 9
                    // Use G92 to set offset in current workspace
                    Debug.WriteLine("Workspace Coordinate System");
                    break;
                case "G60":
                    Debug.WriteLine("Save Current position");

                    break;
                case "G61":
                    Debug.WriteLine("Return to saved position");
                    break;
                case "G90":
                    Debug.WriteLine("Absolute Positioning");
                    break;
                case "G91":
                    Debug.WriteLine("Relative Positioning");
                    break;
                case "G92": // Disable <0 values
                    Debug.WriteLine("Set Position");
                    break;

                // M codes
                case "M0":
                case "M1":
                    Debug.WriteLine("Unconditional STOP");
                    break;
                case "M3":
                    Debug.WriteLine("Turn on Spindel CW");
                    break;
                case "M4":
                    Debug.WriteLine("Turn on Spindel CCW");
                    break;
                case "M5":
                    Debug.WriteLine("Turn off Spindel");
                    break;
                case "M7":
                case "M8":
                case "M9":
                    Debug.WriteLine("Coolant Controls");
                    break;
                case "M16":
                    // Always return true (no printer check)
                    Debug.WriteLine("Device checker");
                    break;
                case "M18":
                case "M84":
                    Debug.WriteLine("Disable all steppers");
                    settings.StepperEnable = false;
                    break;

                // Only when using SD Card
                case "M20":
                    Debug.WriteLine("List Items SD card");
                    break;
                case "M21":
                    Debug.WriteLine("Initialize SD card");
                    break;
                case "M22":
                    Debug.WriteLine("Release SD card");
                    break;
                case "M23":
                    Debug.WriteLine("Select file on SD");
                    break;
                case "M24":
                    Debug.WriteLine("Start/Resume SD file");
                    break;
                case "M25":
                    Debug.WriteLine("Pause SD file");
                    break;
                case "M27":
                    Debug.WriteLine("Report SD Execution Status");
                    break;
                case "M28": // May be discontinued
                    Debug.WriteLine("Start Write to SD Card");
                    break;
                case "M29": // May be discontinued
                    Debug.WriteLine("Stop Write to SD Card");
                    break;
                case "M30":
                    Debug.WriteLine("Delete file from SD");
                    break;

                case "M31":
                    Debug.WriteLine("Print Time");
                    break;
                case "M32":
                    Debug.WriteLine("Begin from SD File");
                    break;
                // End of SD Card Section

                case "M42": // Analog or Digital pins
                    Debug.WriteLine("Set pin state");
                    gpio.SetPin((int)value.P, value.S);
                    break;
                case "M43":
                    // Also T option, to be checked in further functions
                    Debug.WriteLine("Debug / Toggle Pins");
                    gpio.ReadPin((int)value.P);
                    break;

                // LCD
                case "M73":
                    Debug.WriteLine("Mannually set progress");
                    break;
                case "M75":
                    Debug.WriteLine("Start Print Job Timer");
                    break;
                case "M76":
                    Debug.WriteLine("Pause Print Job Timer");
                    break;
                case "M77":
                    Debug.WriteLine("Stop Print Job Timer");
                    break;
                // End LCD

                case "M92":
                    Debug.WriteLine("Set Axis Steps per unit (mm/in)");

                    if (settings.MM)
                    {
                        //if(value.E)
                    }
                    break;
                case "M100":
                    GC.Collect();
                    //return Process.GetCurrentProcess().PrivateMemorySize64;
                    break;
                case "M102":
                    Debug.WriteLine("Configure Bed Distance Sensor");
                    break;
                case "M105":
                    Debug.WriteLine("Temperature Report");
                    break;
                case "M111":
                    Debug.WriteLine("Set debug level");
                    break;
                case "M112":
                    Debug.WriteLine("Full Shutdown");
                    break;
                case "M114":
                    Debug.WriteLine("Get Current Position");
                    break;
                case "M115":
                    Debug.WriteLine("Firmware Info");
                    break;
                case "M117":
                    Debug.WriteLine("Set LCD Message");
                    break;
                case "M118":
                    Debug.WriteLine("Serial Print");
                    break;
                case "M119":
                    Debug.WriteLine("Endstop States");
                    break;
                case "M149":
                    Debug.WriteLine("Set Temperature Units (C/F/K)");
                    break;
                case "M150":
                    Debug.WriteLine("Set LED(strip) RGB(W) Color");
                    break;

                case "M201":
                    Debug.WriteLine("Set MAX Acceleration");
                    break;
                case "M205":
                    Debug.WriteLine("Advanced axis Settings");
                    break;
                case "M206":
                    Debug.WriteLine("Home Offset");
                    break;
                case "M226":
                    Debug.WriteLine("Wait for pin state (0/1/-1)");
                    break;
                case "M256":
                    Debug.WriteLine("Set/Get LCD Brightness");
                    break;

                case "M300":
                    Debug.WriteLine("Play Tone");
                    break;
                case "M305":
                    Debug.WriteLine("Set MicroStepping");
                    break;
                case "M355":
                    Debug.WriteLine("Case Light Control");
                    break;

                case "M400":
                    Debug.WriteLine("Wait for moves to finish");
                    break;
                case "M401":
                    Debug.WriteLine("Deploy Bed Probe");
                    break;
                case "M402":
                    Debug.WriteLine("Stow Bed Probe");
                    break;
                case "M410":
                    Debug.WriteLine("Stop all stepper instantly");
                    break;
                case "M420":
                    Debug.WriteLine("Get/Set Bed Leveling State");
                    break;
                case "M428":
                    Debug.WriteLine("Set Home at Current Position");
                    break;

                case "M510":
                    Debug.WriteLine("Lock Machine");
                    break;
                case "M511":
                    Debug.WriteLine("Unlock Machine");
                    break;
                case "M512":
                    Debug.WriteLine("Set Passcode");
                    break;
                case "M524":
                    Debug.WriteLine("Abort Print in Progress");
                    break;
                case "M575":
                    Debug.WriteLine("Change Serial BaudRate");
                    break;

                case "M928":
                    Debug.WriteLine("Start Logging to SD");
                    break;
                case "M995":
                    Debug.WriteLine("Calibrate Touchscreen");
                    break;
                // Custom
                case "X0":
                    Debug.WriteLine("Selective Stop");
                    settings.EmergencyStop = true;
                    break;
                default:
                    Debug.WriteLine("Code not Found");
                    break;
            }
        }

        private Value createValue(string[] Input)
        {
            Value value = new Value();
            value.Workplane = settings.Workplane;
            if (Input == null)
            {
                return null;
            }
            if (Input[0][0] == 'G' || Input[0][0] == 'M' || Input[0][0] == 'X')
            {
                value.Command = Input[0];
                if (Input[0] == "G0" || Input[0] == "G1")
                {
                    lastCommand = Input[0];
                }
            }
            else
            {
                value.Command = lastCommand;
            }
            if (Input.Length > 2)
            {
                for (int i = 1; i < Input.Length; i++)
                {
                    switch (Input[i][0])
                    {

                        case 'E': // Extruder
                        case 'F': // FlowRate
                        case 'S': // Spindel
                                  // Combine all in case of wrong configuration
                            if (getDouble(Input[i]) > 0)
                            {
                                value.S = true;
                            }
                            else { value.S = false; }
                            Debug.WriteLine("S: " + value.S);
                            break;
                        case 'I':
                            value.I = getDouble(Input[i]);
                            Debug.WriteLine("I: " + value.I);
                            break;
                        case 'J':
                            value.J = getDouble(Input[i]);
                            Debug.WriteLine("J: " + value.J);
                            break;
                        case 'R':
                            value.R = getDouble(Input[i]);
                            Debug.WriteLine("R: " + value.R);
                            break;
                        case 'P':
                            value.P = getDouble(Input[i]);
                            Debug.WriteLine("P: " + value.P);
                            break;
                        case 'Q':
                            value.Q = getDouble(Input[i]);
                            Debug.WriteLine("Q: " + value.Q);
                            break;
                        case 'X':
                            value.X = getDouble(Input[i]);
                            Debug.WriteLine("X: " + value.X);
                            break;
                        case 'Y':
                            value.Y = getDouble(Input[i]);
                            Debug.WriteLine("Y: " + value.Y);
                            break;
                        case 'Z':
                            value.Z = getDouble(Input[i]);
                            Debug.WriteLine("Z: " + value.Z);
                            break;
                        default:
                            break;
                    }
                }
            }
            return value;
        }


        private double getDouble(string Input)
        {
            string number = new string("");
            for (int i = 1; i < Input.Length; i++)
            {
                number.Append(Input[i]);
            }
            return double.Parse(number.ToString());
        }
    }
}
